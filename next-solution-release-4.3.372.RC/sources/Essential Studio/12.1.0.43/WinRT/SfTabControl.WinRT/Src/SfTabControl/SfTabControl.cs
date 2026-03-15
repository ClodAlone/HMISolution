// <copyright file="SfTabControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using Windows.UI.Popups;
using System.Collections.Specialized;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Controls.Navigation
{
    /// <summary>
    /// Represents a control that contains multiple items that share the same space for
    /// content on the screen.
    /// </summary>
    /// <remarks>
    /// TabControl is a <see cref="N:Syncfusion.UI.Xaml.Primitives.Selector"/> which
    /// means it can contain a collection of objects of any type (such as string, image,
    /// or panel) and it is selectable.
    /// </remarks>
    /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
    /// Namespace</seealso>
    [ClassReference(IsReviewed = false)]
    public class SfTabControl : Syncfusion.UI.Xaml.Primitives.Selector,IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfTabControl()
        {
            DefaultStyleKey = typeof(SfTabControl);
            Loaded += SfTabControl_Loaded;
            LayoutUpdated += SfTabControl_LayoutUpdated;
            if (ItemsSource != null)
                ((INotifyCollectionChanged)ItemsSource).CollectionChanged += SfTabControl_CollectionChanged;
            IsEnabledChanged += SfTabControl_IsEnabledChanged;            
            Unloaded += SfTabControl_Unloaded;
        }

        void SfTabControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SfTabControl_Loaded;
            LayoutUpdated -= SfTabControl_LayoutUpdated;
            IsEnabledChanged -= SfTabControl_IsEnabledChanged;
            Unloaded -= SfTabControl_Unloaded;
            if (TabStripMenu != null)
            {
                TabStripMenu.Opened -= TabStripMenu_Opened;
                TabStripMenu.Closed -= TabStripMenu_Closed;
            }
        }

        void SfTabControl_LayoutUpdated(object sender, object e)
        {
            if (PinnedItems != null && PinnedItems.Count > 0 && pinnedpanel!=null && pinnedpanel.Children.Count == 0)
            {
                CreatePinnedPanelItems();
                pinnedpanel.InvalidateMeasure();
            }
            else if (pinnedpanel != null && pinnedpanel.Children.Count == 0)
            {
                pinnedpanel.Visibility = Visibility.Collapsed;
                if(pinnedItemsGrid != null)
                    pinnedItemsGrid.Visibility = Visibility.Collapsed;
                if(pinnedSeperator != null)
                    pinnedSeperator.Visibility = Visibility.Collapsed;
            }
            if (pinnedpanel != null)
            {
                foreach (SfTabItem item in pinnedpanel.Children)
                {
                    if (!item.IsSelected)
                        VisualStateManager.GoToState(item, "UnSelected", true);
                }
            }
        }

        void Content_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
			if(TabStripMenu != null && TabStripMenu.IsOpen && !(e.OriginalSource is RepeatButton))
                TabStripMenu.IsOpen = false;
        }

        void Content_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (TabStripMenu != null && TabStripMenu.IsOpen && !(e.OriginalSource is RepeatButton))
                TabStripMenu.IsOpen = false;
        }
        void SfTabControl_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeTabStripPlacement();
            Window.Current.Content.PointerCaptureLost += Content_PointerCaptureLost;
            Window.Current.Content.PointerPressed += Content_PointerPressed;
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        void SfTabControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
                VisualStateManager.GoToState(this, "Normal", true);
            else
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        void SfTabControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PinnedItems.Clear();
        }

        #endregion

        #region Variables

        private bool m_itemsChanged;

        internal bool internalremove;

        internal Grid mainGrid;

        internal Grid commonButtonGrid;

        internal ScrollViewer ScrollViewer;

        internal ItemsPresenter itemsPresenter;

        internal Grid pinnedItemsGrid;

        internal Rectangle pinnedSeperator;

        internal SfTabPanel pinnedpanel;

        internal ScrollViewer PinnedScrollViewer;

        internal Border pinnedCommonButtonBorder;

        internal Grid pinnedCommonButtonGrid;

        internal RepeatButton pinnedPreviousTabButton;

        internal RepeatButton pinnedNextTabButton;

        internal ContentPresenter contentPresenter;

        internal SfTabItem previousItem;

        internal RepeatButton previousTabButton;

        internal RepeatButton nextTabButton;
        
        internal Popup TabStripMenu;

        internal ListView TabStripMenuList;

        internal Border listBorder = new Border();

        internal RepeatButton commonCloseButton, tabStripMenuButton;

        internal Border commonCloseBorder,commonButtonGridBorder;

        internal CompositeTransform transform;

        internal ObservableCollection<object> PinnedItems;

		private bool isCloseButtonBoth;
        /// <summary>
        /// Represents an Event handler for handling TabClosed event.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public delegate void TabClosedEventHandler(object Sender, CloseTabEventArgs args);

        /// <summary>
        /// This event will be raised after the tab item is closed in TabControl.
        /// </summary>
        public event TabClosedEventHandler TabClosed;

        /// <summary>
        /// Represents an Event handler for handling NextTab/PreviousTab event.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public delegate void TabScrollEventHandler(object Sender, ScrollTabEventArgs args);

        /// <summary>
        /// This event will be raised after the tab item is navigated to Right/Bottom.
        /// </summary>
        public event TabScrollEventHandler NextTab;

        /// <summary>
        /// This event will be raised after the tab item is navigated to Left/Top.
        /// </summary>
        public event TabScrollEventHandler PreviousTab;

        /// <summary>
        /// Represents an Event handler for handling TabClosing event.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>e
        public delegate void CancelingEventHandler(object Sender, CancelingEventArgs args);

        /// <summary>
        /// This cancellable event will be raised before the tab item is closed in TabControl.
        /// </summary>
        public event CancelingEventHandler TabClosing;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof (DataTemplate), typeof (SfTabControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for the data used as header for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        /// <value>
        /// The default is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateSelectorProperty =
            DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(SfTabControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets Template Selector for the data used as header for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        /// <value>
        /// The default is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the placement option to place the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        /// <remarks>
        /// Tab strip can be placed around the control using <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.TabStripPlacement"/> property. It
        /// has the following options. Default option is Top.
        /// </remarks>
        /// <value>
        /// <para>Default value is <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Navigation.TabStripPlacement.Top"/>.</para>
        /// </value>
        [ClassReference(IsReviewed = false)]
        public TabStripPlacement TabStripPlacement
        {
            get { return (TabStripPlacement)GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TabStripPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty =
            DependencyProperty.Register("TabStripPlacement", typeof(TabStripPlacement), typeof(SfTabControl), new PropertyMetadata(TabStripPlacement.Top, new PropertyChangedCallback(OnTabStripPlacementChanged)));



        /// <summary>
        /// Gets or sets the data used to display the content of the selected <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>.
        /// </summary>
        /// <value>
        ///The Default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(SfTabControl), new PropertyMetadata(null));
        
      

        /// <summary>
        /// Gets or sets the collection of transitions that apply to the content area of
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        /// <value>
        /// <see cref="N:Windows.UI.Xaml.Media.Animation.">Transition</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public TransitionCollection ContentTransitions
        {
            get { return (TransitionCollection)GetValue(ContentTransitionsProperty); }
            set { SetValue(ContentTransitionsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentTransitionsProperty =
            DependencyProperty.Register("ContentTransitions", typeof(TransitionCollection), typeof(SfTabControl), new PropertyMetadata(null));

        
        internal Object SelectedContent
        {
            get { return (Object)GetValue(SelectedContentProperty); }
            set { SetValue(SelectedContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty =
            DependencyProperty.Register("SelectedContent", typeof(Object), typeof(SfTabControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Template Selector for the data used to display the content of the selected <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>.
        /// </summary>
        /// <value>
        ///The Default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(SfTabControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value of the TabScrollButtonVisibility dependency property.
        /// </summary>
        public TabScrollButtonVisibility TabScrollButtonVisibility
        {
            get { return (TabScrollButtonVisibility)GetValue(TabScrollButtonVisibilityProperty); }
            set { SetValue(TabScrollButtonVisibilityProperty, value); }
        }
        /// <summary>
        ///  Represents the TabScrollButtonVisibilityProperty
        /// </summary>
        public static readonly DependencyProperty TabScrollButtonVisibilityProperty =
            DependencyProperty.Register("TabScrollButtonVisibility", typeof(TabScrollButtonVisibility), typeof(SfTabControl), new PropertyMetadata(TabScrollButtonVisibility.Collapsed, new PropertyChangedCallback(OnTabScrollButtonVisibilityChanged)));
        
        /// <summary>
        /// Gets or sets the value of the PinnedTabScrollButtonVisibility dependency property.
        /// </summary>
        public TabScrollButtonVisibility PinnedTabScrollButtonVisibility
        {
            get { return (TabScrollButtonVisibility)GetValue(PinnedTabScrollButtonVisibilityProperty); }
            set { SetValue(PinnedTabScrollButtonVisibilityProperty, value); }
        }
        /// <summary>
        ///  Represents the PinnedTabScrollButtonVisibilityProperty
        /// </summary>
        public static readonly DependencyProperty PinnedTabScrollButtonVisibilityProperty =
            DependencyProperty.Register("PinnedTabScrollButtonVisibility", typeof(TabScrollButtonVisibility), typeof(SfTabControl), new PropertyMetadata(TabScrollButtonVisibility.Collapsed, new PropertyChangedCallback(OnPinnedTabScrollButtonVisibilityChanged)));
        
        /// <summary>
        /// Gets or sets the value of the CloseButtonType dependency property.
        /// </summary>
        public CloseButtonType CloseButtonType
        {
            get{ return (CloseButtonType)GetValue(CloseButtonTypeProperty);}
            set{SetValue(CloseButtonTypeProperty, value);}
        }
        /// <summary>
        ///  Represents the CloseButtonTypeProperty
        /// </summary>
        public static readonly DependencyProperty CloseButtonTypeProperty =
            DependencyProperty.Register("CloseButtonType", typeof(CloseButtonType), typeof(SfTabControl), new PropertyMetadata(CloseButtonType.Hide, new PropertyChangedCallback(OnCloseButtonTypeChanged)));


        /// <summary>
        /// Gets or sets the value for the SelectionStyle
        /// </summary>
        public SelectionStyle SelectionStyle
        {
            get { return (SelectionStyle)GetValue(SelectionStyleProperty); }
            set { SetValue(SelectionStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionStyleProperty =
            DependencyProperty.Register("SelectionStyle", typeof(SelectionStyle), typeof(SfTabControl), new PropertyMetadata(SelectionStyle.HeaderText, new PropertyChangedCallback(OnSelectionStyleChanged)));

        /// <summary>
        /// Gets or sets a value to display the TabStripMenu with Tab Item headers for switching tab items on selection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool ShowTabstripMenu
        {
            get { return (bool)GetValue(ShowTabstripMenuProperty); }
            set { SetValue(ShowTabstripMenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowTabstripMenu.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowTabstripMenuProperty =
            DependencyProperty.Register("ShowTabstripMenu", typeof(bool), typeof(SfTabControl), new PropertyMetadata(false,new PropertyChangedCallback(OnShowTabStripMenuChanged)));
        
        /// <summary>
        /// Gets or sets the Menu items for TabstripMenu
        /// </summary>
        public List<object> TabstripMenuItems
        {
            get { return (List<object>)GetValue(TabstripMenuItemsProperty); }
            set { SetValue(TabstripMenuItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TabstripItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabstripMenuItemsProperty =
            DependencyProperty.Register("TabstripMenuItems", typeof(List<object>), typeof(SfTabControl), new PropertyMetadata(null));

        /// <summary>
        /// Used to set the template for the TabstripMenuItems
        /// </summary>
        public DataTemplate TabstripMenuItemTemplate
        {
            get { return (DataTemplate)GetValue(TabstripMenuItemTemplateProperty); }
            set { SetValue(TabstripMenuItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TabstripMenuItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabstripMenuItemTemplateProperty =
            DependencyProperty.Register("TabstripMenuItemTemplate", typeof(DataTemplate), typeof(SfTabControl), new PropertyMetadata(null));

        

        /// <summary>
        /// Gets or sets the tabcontrol horizontal template
        /// </summary>
        public ControlTemplate TabControlHorizontalTemplate
        {
            get { return (ControlTemplate)GetValue(TabControlHorizontalTemplateProperty); }
            set { SetValue(TabControlHorizontalTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TabControlHorizontalTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabControlHorizontalTemplateProperty =
            DependencyProperty.Register("TabControlHorizontalTemplate", typeof(ControlTemplate), typeof(SfTabControl), new PropertyMetadata(null, OnTabControlHorizontalTemplateChanged));


        /// <summary>
        /// Gets or sets the tabcontrol Vertical template
        /// </summary>
        public ControlTemplate TabControlVerticalTemplate
        {
            get { return (ControlTemplate)GetValue(TabControlVerticalTemplateProperty); }
            set { SetValue(TabControlVerticalTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TabControlVerticalTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabControlVerticalTemplateProperty =
            DependencyProperty.Register("TabControlVerticalTemplate", typeof(ControlTemplate), typeof(SfTabControl), new PropertyMetadata(null, OnTabControlVerticalTemplateChanged));   


        #endregion

        #region Helper Methods

        internal bool IsPinnedItemsPanelHasSelectedItems()
        { 
            if(pinnedpanel!=null && pinnedpanel.Children.Count>0)
            {
                foreach(SfTabItem item in pinnedpanel.Children)
                    if (item.IsSelected)
                        return true;
            }
            return false;
        }
        public void Dispose()
        {
            #region TabItems Dispose
            SfTabItem item;
            int index = 0;
            foreach (var vitem in Items)
            {
                if (!(vitem is SfTabItem))
                    item = ItemContainerGenerator.ContainerFromItem(vitem) as SfTabItem;
                else
                    item = vitem as SfTabItem;
                if (item != null)
                {
                    item.Dispose();
                    if (item.Visibility == Visibility.Collapsed
                    && pinnedpanel != null && pinnedpanel.Children.Count > 0 && index < pinnedpanel.Children.Count)
                    {
                        item = pinnedpanel.Children[index] as SfTabItem;
                        if (item!= null)
                            item.Dispose();
                    }
                }
                index++;
            }
            #endregion

            #region TabPanel Dispose

            SfTabPanel panel = GetTabPanel();
            if(panel != null)
                panel.Dispose();

            panel = GetPinnedTabPanel();
            if(panel != null)
                panel.Dispose();

            #endregion

            Loaded -= SfTabControl_Loaded;
            LayoutUpdated -= SfTabControl_LayoutUpdated;

            if (ItemsSource != null)
                ((INotifyCollectionChanged)ItemsSource).CollectionChanged -= SfTabControl_CollectionChanged;

            if (Window.Current.Content != null)
            {
                Window.Current.Content.PointerCaptureLost -= Content_PointerCaptureLost;
                Window.Current.Content.PointerPressed -= Content_PointerPressed;
            }
            if (ScrollViewer != null)
            {
                ScrollViewer.ViewChanged -= ScrollViewer_ViewChanged;
            }

            if (PinnedScrollViewer != null)
            {
                PinnedScrollViewer.ViewChanged -= ScrollViewer_ViewChanged;
            }
            if (TabStripMenuList != null)
                TabStripMenuList.SelectionChanged -= TabStripMenuList_SelectionChanged;
            if (listBorder != null)
            {
                listBorder.Loaded -= listBorder_Loaded;
                TabStripMenu.LostFocus -= TabStripMenu_LostFocus;
            }
            if (commonCloseButton != null)
            {
                commonCloseButton.PointerMoved -= commonCloseButton_PointerMoved;
                commonCloseButton.PointerExited -= commonCloseButton_PointerExited;
                commonCloseButton.Click -= commonCloseButton_Click;
                commonCloseButton.PointerCaptureLost -= commonCloseButton_PointerCaptureLost;
            }

            if (tabStripMenuButton != null)
            {
                tabStripMenuButton.Click -= tabStripMenuButton_Click;
                tabStripMenuButton.PointerMoved -= tabStripMenuButton_PointerMoved;
                tabStripMenuButton.PointerExited -= tabStripMenuButton_PointerExited;
                tabStripMenuButton.PointerCaptureLost -= tabStripMenuButton_PointerCaptureLost;
            }

            if (previousTabButton != null)
            {
                previousTabButton.PointerMoved -= previousTabButton_PointerMoved;
                previousTabButton.PointerExited -= previousTabButton_PointerExited;
                previousTabButton.Click -= previousTabButton_Click;
                previousTabButton.PointerCaptureLost -= previousTabButton_PointerCaptureLost;
            }

            if (nextTabButton != null)
            {
                nextTabButton.PointerMoved -= nextTabButton_PointerMoved;
                nextTabButton.PointerExited -= nextTabButton_PointerExited;
                nextTabButton.Click -= nextTabButton_Click;
                nextTabButton.PointerCaptureLost -= nextTabButton_PointerCaptureLost;
            }

            if (pinnedPreviousTabButton != null)
            {
                pinnedPreviousTabButton.PointerMoved -= previousTabButton_PointerMoved;
                pinnedPreviousTabButton.PointerExited -= previousTabButton_PointerExited;
                pinnedPreviousTabButton.Click -= previousTabButton_Click;
                pinnedPreviousTabButton.PointerCaptureLost -= previousTabButton_PointerCaptureLost;
            }

            if (pinnedNextTabButton != null)
            {
                pinnedNextTabButton.PointerMoved -= nextTabButton_PointerMoved;
                pinnedNextTabButton.PointerExited -= nextTabButton_PointerExited;
                pinnedNextTabButton.Click -= nextTabButton_Click;
                pinnedNextTabButton.PointerCaptureLost -= nextTabButton_PointerCaptureLost;
            }
        }

        internal bool ClosingTab(SfTabItem tabItem)
        {
            var args = new CancelingEventArgs(tabItem);
            if (TabClosing != null)
            {
                TabClosing(this, args);                
            }
            return args.Cancel;
        }

        internal void ClosedTab(SfTabItem tabItem)
        {
            var closeArgs = new CloseTabEventArgs(tabItem);
            if (TabClosed != null)
            {
                TabClosed(this, closeArgs);
            }
        }

        private void ChangeTabStripPlacement()
        {
            if (TabStripMenu != null && TabStripMenu.IsOpen)
                TabStripMenu.IsOpen = false;
            if ((mainGrid != null && itemsPresenter != null && contentPresenter != null|| this.Visibility==Visibility.Collapsed))
            {
                SfTabPanel panel=GetTabPanel();
                if (TabStripPlacement == TabStripPlacement.Left)
                {
                    Template = TabControlVerticalTemplate != null ? TabControlVerticalTemplate : Template;
                    VisualStateManager.GoToState(this, "Left", true);
                    CreatePinnedPanelItems();
                }
                else if (TabStripPlacement == TabStripPlacement.Right)
                {
                    Template = TabControlVerticalTemplate != null ? TabControlVerticalTemplate : Template;
                    VisualStateManager.GoToState(this, "Right", true);
                    CreatePinnedPanelItems();
                }
                else if (TabStripPlacement == TabStripPlacement.Bottom)
                {
                    Template = TabControlHorizontalTemplate != null ? TabControlHorizontalTemplate : Template;
                    VisualStateManager.GoToState(this, "Bottom", true);
                    CreatePinnedPanelItems();
                }
                else if (TabStripPlacement == TabStripPlacement.Top)
                {
                    Template = TabControlHorizontalTemplate != null ? TabControlHorizontalTemplate : Template;
                    VisualStateManager.GoToState(this, "Top", true);
                    CreatePinnedPanelItems();
                }

                if (panel != null)
                {
                    ValidateScrollBarVisibility();
                    CheckNavigationButtonVisibility(panel.m_scrollInfo.NeedScrollButtonsShow, panel.ActualWidth);
                }
                m_itemsChanged = true;
            }
        }

        private void CreatePinnedPanelItems()
        {
            if (PinnedItems != null && PinnedItems.Count > 0 && pinnedpanel != null && pinnedpanel.Children.Count == 0)
            {
                foreach (object tabitem in PinnedItems)
                {
                    SfTabItem newItem = new SfTabItem();
                    if (tabitem is SfTabItem)
                    {
                        (Items[PinnedItems.IndexOf(tabitem)] as SfTabItem).Cloning(newItem);
                        pinnedpanel.Children.Add(newItem);
                    }
                    else
                    {
                        if (ItemContainerGenerator.ContainerFromItem(tabitem) is SfTabItem)
                            (ItemContainerGenerator.ContainerFromItem(tabitem) as SfTabItem).Cloning(newItem);
                        pinnedpanel.Children.Add(newItem);
                    }
                    if (newItem.IsSelected)
                    {
                        if (SelectionStyle == SelectionStyle.HeaderText)
                        {
                            VisualStateManager.GoToState(newItem, "Normal", true);
                        }
                        else
                        {
                            VisualStateManager.GoToState(newItem, "Selected", true);
                        }
                    }
                    else
                        VisualStateManager.GoToState(newItem, "UnSelected", true);
                }
                pinnedpanel.Visibility = Visibility.Visible;
            }
            UpdateLayout();
        }

        private Point ShowTabStripPopupMenu(double left, double top)
        {
            double _left, _top;
            Point location;
            if (TabStripPlacement == TabStripPlacement.Left)
            {
                listBorder.MaxHeight = this.ActualHeight - 4;
                _left = left + tabStripMenuButton.ActualWidth;
                _top = top - (listBorder.ActualHeight - tabStripMenuButton.ActualHeight);
            }
            else if (TabStripPlacement == TabStripPlacement.Top)
            {
                listBorder.MaxHeight = this.ActualHeight - (tabStripMenuButton.ActualHeight - commonButtonGridBorder.Margin.Top) - BorderThickness.Top;
                _left = left - (listBorder.ActualWidth - tabStripMenuButton.ActualWidth);
                _top = top + tabStripMenuButton.ActualHeight + 2 - BorderThickness.Top;
            }
            else if (TabStripPlacement == TabStripPlacement.Right)
            {
                listBorder.MaxHeight = this.ActualHeight - 4;
                _left = left - listBorder.ActualWidth;
                _top = top - (listBorder.ActualHeight - tabStripMenuButton.ActualHeight);
            }
            else
            {
                listBorder.MaxHeight = this.ActualHeight - (tabStripMenuButton.ActualHeight + commonButtonGridBorder.Margin.Top) - BorderThickness.Top;
                _left = left - (listBorder.ActualWidth - tabStripMenuButton.ActualWidth);
                _top = top - listBorder.ActualHeight - 2 - BorderThickness.Top;
            }
            location.X = _left;
            location.Y = _top;
            return location;
        }

        void listBorder_Loaded(object sender, RoutedEventArgs e)
        {
            var ttv = (this.tabStripMenuButton).TransformToVisual(this);
            Point point = ttv.TransformPoint(new Point(0, 0));
            Point position = ShowTabStripPopupMenu(point.X,point.Y);
            TabStripMenu.HorizontalOffset = position.X;
            TabStripMenu.VerticalOffset = position.Y;
        }
        
        #endregion

        #region Override Methods

        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/> SfTabControl control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            ScrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            if (ScrollViewer != null)
            {
                ScrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            }
            PinnedScrollViewer = GetTemplateChild("PART_PinnedScrollViewer") as ScrollViewer;

            if (PinnedScrollViewer != null)
            {
                PinnedScrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            }
            mainGrid = GetTemplateChild("MainGrid") as Grid;
            itemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
            pinnedpanel=GetTemplateChild("PinnedPanel") as SfTabPanel;
            pinnedItemsGrid = GetTemplateChild("PART_PinnedItemsGrid") as Grid;
            pinnedSeperator = GetTemplateChild("PART_Seperator") as Rectangle;
            contentPresenter = GetTemplateChild("PART_Content") as ContentPresenter;
            commonButtonGrid = GetTemplateChild("CommonButtonGrid") as Grid;
            commonButtonGridBorder = GetTemplateChild("CommonButtonBorder") as Border;
            pinnedCommonButtonGrid = GetTemplateChild("PinnedCommonButtonGrid") as Grid;
            TabStripMenu = GetTemplateChild("PART_TabStripMenuPopup") as Popup;
            TabStripMenuList = GetTemplateChild("Part_ListView") as ListView;
            if(TabStripMenuList!=null)
                TabStripMenuList.SelectionChanged += TabStripMenuList_SelectionChanged;
            
            listBorder = GetTemplateChild("Part_ListBorder") as Border;
            if (listBorder != null)
            {
                listBorder.Loaded += listBorder_Loaded;
                TabStripMenu.LostFocus+=TabStripMenu_LostFocus;
            }
           
            commonCloseButton = GetTemplateChild("CommonCloseButton") as RepeatButton;
            if (commonCloseButton != null)
            {
                commonCloseButton.PointerMoved+=commonCloseButton_PointerMoved;
                commonCloseButton.PointerExited+=commonCloseButton_PointerExited;
                commonCloseButton.Click += commonCloseButton_Click;
                commonCloseButton.PointerCaptureLost += commonCloseButton_PointerCaptureLost;
            }
            commonCloseBorder = GetTemplateChild("CommonButtonBorder") as Border;
            pinnedCommonButtonBorder = GetTemplateChild("PinnedCommonButtonBorder") as Border;
            CheckCommonCloseButtonVisibilityOnLoad(commonCloseButton);

            tabStripMenuButton = GetTemplateChild("TabstripMenuButton") as RepeatButton;
            
            if (tabStripMenuButton != null)
            {
                if (ShowTabstripMenu && Items.Count>0)
                {
                    tabStripMenuButton.Visibility = Visibility.Visible;
                }
                else
                {
                    tabStripMenuButton.Visibility = Visibility.Collapsed;
                }
                if (transform == null)
                {
                    transform = new CompositeTransform();
                    tabStripMenuButton.RenderTransform = transform;
                }
                tabStripMenuButton.Click += tabStripMenuButton_Click;
                tabStripMenuButton.PointerMoved += tabStripMenuButton_PointerMoved;
                tabStripMenuButton.PointerExited += tabStripMenuButton_PointerExited;
                tabStripMenuButton.PointerCaptureLost += tabStripMenuButton_PointerCaptureLost;
            }

            previousTabButton = GetTemplateChild("PreviousTabButton") as RepeatButton;
            if (previousTabButton != null)
            {
                previousTabButton.PointerMoved += previousTabButton_PointerMoved;
                previousTabButton.PointerExited += previousTabButton_PointerExited;
                previousTabButton.Click += previousTabButton_Click;
                previousTabButton.PointerCaptureLost += previousTabButton_PointerCaptureLost;
            }

            nextTabButton = GetTemplateChild("NextTabButton") as RepeatButton;
            if (nextTabButton != null)
            {
                nextTabButton.PointerMoved += nextTabButton_PointerMoved;
                nextTabButton.PointerExited += nextTabButton_PointerExited;
                nextTabButton.Click += nextTabButton_Click;
                nextTabButton.PointerCaptureLost += nextTabButton_PointerCaptureLost;
            }

            pinnedPreviousTabButton = GetTemplateChild("PinnedPreviousTabButton") as RepeatButton;
            if (pinnedPreviousTabButton != null)
            {
                pinnedPreviousTabButton.PointerMoved += previousTabButton_PointerMoved;
                pinnedPreviousTabButton.PointerExited += previousTabButton_PointerExited;
                pinnedPreviousTabButton.Click += previousTabButton_Click;
                pinnedPreviousTabButton.PointerCaptureLost += previousTabButton_PointerCaptureLost;
            }

            pinnedNextTabButton = GetTemplateChild("PinnedNextTabButton") as RepeatButton;
            if (pinnedNextTabButton != null)
            {
                pinnedNextTabButton.PointerMoved += nextTabButton_PointerMoved;
                pinnedNextTabButton.PointerExited += nextTabButton_PointerExited;
                pinnedNextTabButton.Click += nextTabButton_Click;
                pinnedNextTabButton.PointerCaptureLost += nextTabButton_PointerCaptureLost;
            }

            ChangeTabStripPlacement();
            base.OnApplyTemplate();
        }

        void TabStripMenu_Closed(object sender, object e)
        {
            if (TabStripMenu != null)
            {
                TabStripMenu.Opened -= TabStripMenu_Opened;
                TabStripMenu.Closed -= TabStripMenu_Closed;
            }
        }

        void TabStripMenu_Opened(object sender, object e)
        {
            if(TabStripMenuList!=null)
            {
                foreach (object listobj in TabStripMenuList.Items)
                {
                    ListViewItem listViewItem=null;
                    if(TabStripMenuList.ItemContainerGenerator!=null)
                        listViewItem = TabStripMenuList.ItemContainerGenerator.ContainerFromItem(listobj) as ListViewItem;
                    if (listViewItem != null && !listViewItem.IsSelected)
                    {
                        VisualStateManager.GoToState(listViewItem, "Unselected", true);
                    }
                }
            }
        }

        void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate)
            {
                SfTabPanel tabpanel;
                if ((sender as ScrollViewer).Name.Equals("PART_PinnedScrollViewer"))
                {
                    tabpanel = GetPinnedTabPanel();
                    tabpanel.ScrollOffset = TabStripPlacement == TabStripPlacement.Top ||
                                            TabStripPlacement == TabStripPlacement.Bottom
                                                ? PinnedScrollViewer.HorizontalOffset
                                                : PinnedScrollViewer.VerticalOffset;
                }
                else
                {
                    tabpanel = GetTabPanel();
                    tabpanel.ScrollOffset = TabStripPlacement == TabStripPlacement.Top ||
                                            TabStripPlacement == TabStripPlacement.Bottom
                                                ? ScrollViewer.HorizontalOffset
                                                : ScrollViewer.VerticalOffset;
                }
            }
        }

        /// <summary>
        /// Invoked when the pointer is pressed on the control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (TabStripMenu != null && TabStripMenu.IsOpen)
                TabStripMenu.IsOpen = false;
            base.OnPointerPressed(e);
        }

        void TabStripMenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SfTabItem tempitem = null;
            SfTabItem selectedItem = null;
            ListView selectedIteminView = sender as ListView;
            foreach (var item in Items)
            {
                if (selectedIteminView.SelectedIndex != -1)
                    tempitem = ItemContainerGenerator.ContainerFromIndex(selectedIteminView.SelectedIndex) as SfTabItem;

                if (tempitem != null && tempitem.Header.Equals(selectedIteminView.SelectedItem))
                {
                    selectedItem = tempitem;
                    break;
                }
            }
            if (selectedItem != null)
            {
                if (pinnedpanel != null)
                {
                    foreach (SfTabItem item in pinnedpanel.Children)
                    {
                        if ((ItemsSource != null && item.DataContext == selectedItem.DataContext) || (item == selectedItem))
                        {
                            item.IsSelected = true;
                        }
                        else
                        {
                            item.IsSelected = false;
                            item.pinnableButton.Opacity = 0;
                        }
                    }
                }
                selectedItem.IsSelected = true;
            }
            TabStripMenu.IsOpen = false;
        }
         
        /// <summary>
        /// Occurs when the Pointer is pressed on the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        void TabStripMenu_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TabStripMenu != null && TabStripMenu.IsOpen)
                TabStripMenu.IsOpen = false;
        }

        void commonCloseButton_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                SfTabItem selectedItem = null;
                if (ItemsSource == null)
                {
                    selectedItem = (SelectedItem as SfTabItem);
                }
                else
                {
                    selectedItem = this.ItemContainerGenerator.ContainerFromItem(SelectedItem) as SfTabItem;
                }
                if (selectedItem != null)
                    selectedItem.CloseTab();
                if (this.Items.Count == 0)
                    this.SelectedContent = null;
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
            else
            {
                if (pinnedpanel != null)
                {
                    foreach (var tabitem in pinnedpanel.Children)
                    {
                        if ((tabitem as SfTabItem).IsSelected)
                            (tabitem as SfTabItem).CloseTab();
                    }
                }
                else
                    VisualStateManager.GoToState(this, "UnSelected", true);
            }
        }

        void tabStripMenuButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "TabStripPressed", true); 
        }

        void tabStripMenuButton_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (TabStripMenu != null)
            {
                TabStripMenu.Opened += TabStripMenu_Opened;
                TabStripMenu.Closed += TabStripMenu_Closed;
            }            
            var ttv = (sender as UIElement).TransformToVisual(this);
            Point point = ttv.TransformPoint(new Point(0, 0));
            if (m_itemsChanged || TabstripMenuItems != null)
            {
                PopulateItems();
                m_itemsChanged = false;
            }
            if (TabStripMenuList != null && (TabstripMenuItems == null || (TabstripMenuItems != null && TabstripMenuItems.Count == 0)) && Items.Count != 0)
            {
                if (SelectedItem != null)
                {
                    if (ItemsSource != null)
                        TabStripMenuList.SelectedIndex = Items.IndexOf(SelectedItem);
                    else
                        TabStripMenuList.SelectedItem = (SelectedItem as SfTabItem).Header;
                    VisualStateManager.GoToState(this, "TabStripPointerOver", true);
                }
                else
                {
                    if (pinnedpanel != null && pinnedpanel.Children.Count > 0)
                    {
                        TabStripMenuList.SelectedIndex = pinnedpanel.Children.IndexOf(pinnedpanel.Children.OfType<SfTabItem>().Where(item => item.IsSelected == true).FirstOrDefault());
                    }
                }
            }
            Point position= ShowTabStripPopupMenu(point.X, point.Y);
            TabStripMenu.HorizontalOffset = position.X;
            TabStripMenu.VerticalOffset = position.Y;
            if (TabStripMenu != null && TabStripMenu.IsOpen)
                TabStripMenu.IsOpen = false;
            else
                TabStripMenu.IsOpen = true;
            VisualStateManager.GoToState(this, "TabStripUnSelected", true);   
        }

        void tabStripMenuButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "TabStripUnSelected", true);    
        }

        void tabStripMenuButton_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "TabStripPointerOver", true);
        }

        internal void PopulateItems()
        {
            if (TabStripMenuList != null)
            {
                if (TabstripMenuItemTemplate == null)
                    TabStripMenuList.ItemTemplate = ItemTemplate;
                if (TabstripMenuItems == null || (TabstripMenuItems != null && TabstripMenuItems.Count == 0))
                {
                    if (ItemsSource != null && ItemTemplate != null)
                        TabStripMenuList.ItemsSource = Items;
                    else if (ItemsSource != null && ItemTemplate == null)
                    {
                        List<object> items = new List<object>();
                        SfTabItem titem = null;
                        foreach (var tabItem in Items)
                        {
                            titem = ItemContainerGenerator.ContainerFromItem(tabItem) as SfTabItem;
                            items.Add(titem.Header);
                        }
                        TabStripMenuList.ItemsSource = items;
                    }
                    else
                    {
                        List<object> items = new List<object>();
                        foreach (SfTabItem tabItem in Items)
                            items.Add(tabItem.Header);
                        TabStripMenuList.ItemsSource = items;
                    }
                }
                else
                    TabStripMenuList.ItemsSource = TabstripMenuItems;
            }
        }

        /// <summary>
        /// Checks the close button visibility on load.
        /// </summary>
        /// <param name="commonclosebutton">The closebutton.</param>
        internal void CheckCommonCloseButtonVisibilityOnLoad(RepeatButton commonclosebutton)
        {
            if (commonclosebutton != null)
            {
                if (this != null && (this.CloseButtonType == CloseButtonType.Both || this.CloseButtonType == CloseButtonType.Common) && Items.Count != 0)
                {
                    commonclosebutton.Visibility = Visibility.Visible;
                }
                else
                {
                    commonclosebutton.Visibility = Visibility.Collapsed;
                }
                commonclosebutton.IsEnabled = true;
            }
        }

        void previousTabButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RepeatButton).Name.Equals("PinnedPreviousTabButton"))
                VisualStateManager.GoToState(this, "PinnedPreviousTabPressed", true);
            else
                VisualStateManager.GoToState(this, "PreviousTabPressed", true);
        }

        void nextTabButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RepeatButton).Name.Equals("PinnedNextTabButton"))
                VisualStateManager.GoToState(this, "PinnedNextTabPressed", true);
            else
                VisualStateManager.GoToState(this, "NextTabPressed", true);
        }

        void commonCloseButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", true);
            if (TabStripMenu != null && TabStripMenu.IsOpen)
                TabStripMenu.IsOpen = false;
        }

        void commonCloseButton_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        void commonCloseButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "UnSelected", true);
        }


        private void previousTabButton_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SfTabPanel panel;
            if ((sender as RepeatButton).Name.Equals("PinnedPreviousTabButton"))
            {
                panel = GetPinnedTabPanel();
                panel.ScrollToPrevTab();
                VisualStateManager.GoToState(this, "PinnedPreviousTabPointerOver", true);
            }
            else
            {
                panel = GetTabPanel();
                panel.ScrollToPrevTab();
                VisualStateManager.GoToState(this, "PreviousTabPointerOver", true);
                if (PreviousTab != null)
                {
                    SfTabItem item =
                        ItemContainerGenerator.ContainerFromIndex(panel.m_scrollInfo.LastTrimmedTabIndex) as SfTabItem;
                    ScrollTabEventArgs args = new ScrollTabEventArgs(item);
                    PreviousTab(this, args);
                }
            }
        }

        private void nextTabButton_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SfTabPanel panel;
            if ((sender as RepeatButton).Name.Equals("PinnedNextTabButton"))
            {
                panel = GetPinnedTabPanel();
                panel.ScrollToNextTab();
                VisualStateManager.GoToState(this, "PinnedNextTabPointerOver", true);
            }
            else
            {
                panel = GetTabPanel();
                panel.ScrollToNextTab();
                VisualStateManager.GoToState(this, "NextTabPointerOver", true);
                if (NextTab != null)
                {
                    SfTabItem item =
                        ItemContainerGenerator.ContainerFromIndex(panel.m_scrollInfo.LastTrimmedTabIndex) as SfTabItem;
                    ScrollTabEventArgs args = new ScrollTabEventArgs(item);
                    NextTab(this, args);
                }
            }
        }

        internal SfTabPanel GetTabPanel()
        {
            SfTabPanel panel = itemsPresenter != null ? GetVisualChild<SfTabPanel>(itemsPresenter) : null;
            return panel;
        }

        internal SfTabPanel GetPinnedTabPanel()
        {
            SfTabPanel panel = GetVisualChild<SfTabPanel>(this);
            return panel;
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                object v = (object)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v as DependencyObject);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        internal void CheckPinnedNavigationButtonVisibility(bool NeedScrollButtonsShow, double panelwidth)
        {
            switch (PinnedTabScrollButtonVisibility)
            {
                case TabScrollButtonVisibility.Auto:
                    if (pinnedNextTabButton != null && pinnedPreviousTabButton != null)
                    {
                        pinnedPreviousTabButton.Visibility = pinnedNextTabButton.Visibility = NeedScrollButtonsShow ? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;

                case TabScrollButtonVisibility.Collapsed:
                    if (pinnedNextTabButton != null && pinnedPreviousTabButton != null)
                    {
                        pinnedPreviousTabButton.Visibility = pinnedNextTabButton.Visibility = Visibility.Collapsed;
                    }
                    break;

                case TabScrollButtonVisibility.Visible:
                    if (pinnedPreviousTabButton != null && pinnedNextTabButton != null)
                    {
                        pinnedPreviousTabButton.Visibility = pinnedNextTabButton.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    break;
            }
            CheckPinnedCommonButtonGridAlignment(panelwidth);
        }

        internal void CheckNavigationButtonVisibility(bool NeedScrollButtonsShow, double panelwidth)
        {
            switch (TabScrollButtonVisibility)
            {
                case TabScrollButtonVisibility.Auto:
                    if (nextTabButton != null && previousTabButton != null)
                    {
                        previousTabButton.Visibility = nextTabButton.Visibility = NeedScrollButtonsShow ? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;

                case TabScrollButtonVisibility.Collapsed:
                    if (nextTabButton != null && previousTabButton != null)
                    {
                        previousTabButton.Visibility = nextTabButton.Visibility = Visibility.Collapsed;     
                    }
                    break;

                case TabScrollButtonVisibility.Visible:
                    if (nextTabButton != null && previousTabButton != null && Items.Count > 0)
                    {
                        previousTabButton.Visibility = nextTabButton.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    break;
            }
            isCloseButtonBoth = (this.CloseButtonType == CloseButtonType.Both);
            CheckCommonButtonGridAlignment(panelwidth);

            if (pinnedpanel.Children.Count > 0)
            {
                pinnedpanel.Visibility = Visibility.Visible;
                pinnedItemsGrid.Visibility = Visibility.Visible;
                pinnedSeperator.Visibility = Visibility.Visible;
            }
            ValidateScrollBarVisibility();
        }

        internal int ValidateDefinitionCount(bool navigationbuttonVisible, bool closebuttonVisible, bool tabstripmenubuttonVisible)
        {
            return (navigationbuttonVisible ? 2 : 0) + (closebuttonVisible ? 1 : 0) + (tabstripmenubuttonVisible ? 1 : 0);
        }

        internal void ValidateScrollBarVisibility()
        {
            if (previousTabButton != null && previousTabButton.Visibility == Visibility.Visible 
                && nextTabButton != null && nextTabButton.Visibility == Visibility.Visible)
            {
                if(!ScrollViewer.GetHorizontalScrollBarVisibility(this).Equals(ScrollBarVisibility.Hidden))
                    ScrollViewer.SetHorizontalScrollBarVisibility(this,ScrollBarVisibility.Hidden);
                if (!ScrollViewer.GetVerticalScrollBarVisibility(this).Equals(ScrollBarVisibility.Hidden))
                    ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Hidden);
            }
        }

        internal void CheckPinnedCommonButtonGridAlignment(double width)
        {
            bool navigationbuttonVisible = (pinnedPreviousTabButton.Visibility == Visibility.Visible && pinnedNextTabButton.Visibility == Visibility.Visible);
            pinnedCommonButtonBorder.Visibility = (!navigationbuttonVisible) ? Visibility.Collapsed : Visibility.Visible;

            int definitioncount = ValidateDefinitionCount(navigationbuttonVisible, false, false);
            if (definitioncount > 0 && definitioncount != pinnedCommonButtonGrid.ColumnDefinitions.Count)
            {
                GridLength gridlength = (TabStripPlacement == TabStripPlacement.Top ||
                                        TabStripPlacement == TabStripPlacement.Bottom)
                                            ? new GridLength(0.0, GridUnitType.Star)
                                            : new GridLength(width / definitioncount);
                pinnedCommonButtonGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < definitioncount; i++)
                {
                    pinnedCommonButtonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridlength });
                }
            }

            int index = TabStripPlacement == TabStripPlacement.Right ? definitioncount - 1 : 0;
            if (navigationbuttonVisible)
            {
                if (TabStripPlacement == TabStripPlacement.Right)
                {
                    Grid.SetColumn(pinnedNextTabButton, index--);
                    Grid.SetColumn(pinnedPreviousTabButton, index--);
                }
                else
                {
                    Grid.SetColumn(pinnedPreviousTabButton, index++);
                    Grid.SetColumn(pinnedNextTabButton, index++);
                }
            }
        }

        internal void CheckCommonButtonGridAlignment(double width)
        {
            bool navigationbuttonVisible = (previousTabButton.Visibility == Visibility.Visible && nextTabButton.Visibility == Visibility.Visible);
            CheckCommonCloseButtonVisibilityOnLoad(commonCloseButton);
            bool closebuttonVisible = commonCloseButton.Visibility == Visibility.Visible;
            bool tabstripmenubuttonVisible = tabStripMenuButton.Visibility == Visibility.Visible;
            commonCloseBorder.Visibility = ((!navigationbuttonVisible && !tabstripmenubuttonVisible && !closebuttonVisible)||(Items.Count == 0 && commonButtonGrid.Visibility==Visibility.Collapsed)) ? Visibility.Collapsed : Visibility.Visible;

            double navigationWidth = nextTabButton.Width + previousTabButton.Width + (nextTabButton.Margin.Left * 3);
            double closeButtonWidth = navigationWidth + commonCloseButton.Width + (commonCloseButton.Margin.Left * 2);
            double tabStripMenuButtonWidth = closeButtonWidth + tabStripMenuButton.Width + (tabStripMenuButton.Margin.Left * 2);
            int definitioncount = ValidateDefinitionCount(navigationbuttonVisible, closebuttonVisible,tabstripmenubuttonVisible);
            if (definitioncount > 0 && (definitioncount != commonButtonGrid.ColumnDefinitions.Count || isCloseButtonBoth))
            {
                if (navigationbuttonVisible && (TabStripPlacement == TabStripPlacement.Left || TabStripPlacement == TabStripPlacement.Right))
                {
                    if (width < navigationWidth)
                        width = navigationWidth;
                }
                if (closebuttonVisible && (TabStripPlacement == TabStripPlacement.Left || TabStripPlacement == TabStripPlacement.Right))
                {
                    if (width < closeButtonWidth)
                        width = closeButtonWidth;
                }
                if (tabstripmenubuttonVisible && (TabStripPlacement == TabStripPlacement.Left || TabStripPlacement == TabStripPlacement.Right))
                {
                    if (width < tabStripMenuButtonWidth)
                        width = tabStripMenuButtonWidth;
                }

                GridLength gridlength = (TabStripPlacement == TabStripPlacement.Top ||
                                        TabStripPlacement == TabStripPlacement.Bottom)
                                            ? new GridLength(0.0, GridUnitType.Star)
                                            : new GridLength(width/definitioncount); 

                if (width <= definitioncount && (TabStripPlacement == TabStripPlacement.Left || TabStripPlacement == TabStripPlacement.Right))
                    gridlength = new GridLength(0.0, GridUnitType.Star);
                if (commonButtonGrid.ColumnDefinitions.Count > 0 && commonButtonGrid.ColumnDefinitions[0].Width != gridlength&&(TabStripPlacement==TabStripPlacement.Left||TabStripPlacement==TabStripPlacement.Right))
                {
                    commonButtonGrid.ColumnDefinitions.Clear();
                    for (int i = 0; i < definitioncount; i++)
                    {
                        commonButtonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width, GridUnitType.Star) });
                    }
                }
                    isCloseButtonBoth = false;
            }

            int index = TabStripPlacement == TabStripPlacement.Right ? definitioncount - 1 : 0;
            if (navigationbuttonVisible)
            {
                if (TabStripPlacement == TabStripPlacement.Right)
                {
                    Grid.SetColumn(nextTabButton, index--);
                    Grid.SetColumn(previousTabButton, index--);
                }
                else
                {
                    Grid.SetColumn(previousTabButton, index++);
                    Grid.SetColumn(nextTabButton, index++);
                }
            }
            if (closebuttonVisible)
            {
                Grid.SetColumn(commonCloseButton, TabStripPlacement == TabStripPlacement.Right ? index-- : index++);
            }
            if (tabstripmenubuttonVisible)
            {
                Grid.SetColumn(tabStripMenuButton, TabStripPlacement == TabStripPlacement.Right ? index-- : index++);
            }
        }

        void previousTabButton_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if ((sender as RepeatButton).Name.Equals("PinnedPreviousTabButton"))
                VisualStateManager.GoToState(this, "PinnedPreviousTabPointerOver", true);
            else
                VisualStateManager.GoToState(this, "PreviousTabPointerOver", true);
        }

        void previousTabButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if ((sender as RepeatButton).Name.Equals("PinnedPreviousTabButton"))
                VisualStateManager.GoToState(this, "PinnedPreviousTabUnSelected", true);
            else
                VisualStateManager.GoToState(this, "PreviousTabUnSelected", true);
        }

        void nextTabButton_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if ((sender as RepeatButton).Name.Equals("PinnedNextTabButton"))
                VisualStateManager.GoToState(this, "PinnedNextTabPointerOver", true);
            else
                VisualStateManager.GoToState(this, "NextTabPointerOver", true);
        }

        void nextTabButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if ((sender as RepeatButton).Name.Equals("PinnedNextTabButton"))
                VisualStateManager.GoToState(this, "PinnedNextTabUnSelected", true);
            else
                VisualStateManager.GoToState(this, "NextTabUnSelected", true);
        }

        /// <summary>
        /// Occurs when the selected Tab item <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/> is changed.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectionChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SelectedItem != null)
            {
                SfTabItem selectedItem = null;
                if (ItemsSource == null)
                {
                    selectedItem = (SelectedItem as SfTabItem);
                }
                else
                {
                    selectedItem = this.ItemContainerGenerator.ContainerFromItem(SelectedItem) as SfTabItem;
                }

                if (args.OldValue != null)
                {
                    if (ItemsSource == null)
                    {
                        previousItem = args.OldValue as SfTabItem;
                    }
                    else
                    {
                        previousItem = this.ItemContainerGenerator.ContainerFromItem(args.OldValue) as SfTabItem;
                    }
                    if (previousItem != null)
                    {
                        previousItem.IsSelected = false;
                        if(previousItem.pinnableButton!=null && previousItem.ShowPinnableButton)
                            previousItem.pinnableButton.Opacity=0;
                    }
                }
                if (pinnedpanel != null)
                {
                    foreach (SfTabItem item in pinnedpanel.Children)
                    {
                        SfTabItem actualItem = null;
                        int index = pinnedpanel.Children.IndexOf(item);
                        if (index != -1)
                        {
                            actualItem = ItemContainerGenerator.ContainerFromIndex(index) as SfTabItem;
                            if (actualItem != null && (actualItem.SelectedBackground as SolidColorBrush).Color != (item.SelectedBackground as SolidColorBrush).Color)
                                item.SelectedBackground = actualItem.SelectedBackground;
                            if (actualItem != null && (actualItem.SelectedForeground as SolidColorBrush).Color != (item.SelectedForeground as SolidColorBrush).Color)
                                item.SelectedForeground = actualItem.SelectedForeground;
                        }
                        bool canExecute = args.OldValue != null && previousItem != null 
                                              ? (ItemsSource != null && previousItem != null && item.DataContext == previousItem.DataContext) || (item == previousItem)
                                              : selectedItem != null && !pinnedpanel.Children.Contains(selectedItem);
                        if (canExecute || (ItemsSource == null && Items.IndexOf(args.NewValue as SfTabItem) != pinnedpanel.Children.IndexOf(item)))
                        {
                            item.IsSelected = false;
                            if (item.pinnableButton != null)
                                item.pinnableButton.Opacity = 0;
                        }
                    }
                }
                if (selectedItem != null)
                {
                    if (previousItem != null && previousItem.IsSelected)
                    {
                        previousItem.IsSelected = false;
                        if (previousItem.pinnableButton != null && previousItem.ShowPinnableButton)
                            previousItem.pinnableButton.Opacity = 0;
                    }
                    SelectedContent = selectedItem.Content;
                    selectedItem.IsSelected = true;
                    if (pinnedpanel != null && pinnedpanel.Children.Count > 0 && selectedItem.Visibility == Visibility.Collapsed
                        && SelectedIndex >= 0 && SelectedIndex < pinnedpanel.Children.Count)
                    {
                        (pinnedpanel.Children[SelectedIndex] as SfTabItem).IsSelected = true;
                    }
                }

                if (ContentTemplateSelector != null)
                {
                    this.ContentTemplate = ContentTemplateSelector.SelectTemplate(SelectedItem, contentPresenter);
                }
            }
            foreach (object item in Items)
            {
                SfTabItem tabitem = item is SfTabItem ? item as SfTabItem : ItemContainerGenerator.ContainerFromIndex(Items.IndexOf(item)) as SfTabItem;
                if (tabitem != null && Items.IndexOf(item) != SelectedIndex && tabitem.IsSelected)
                {
                    tabitem.IsSelected = false;
                }
            }
            if (TabStripMenu != null && TabStripMenu.IsOpen)
                TabStripMenu.IsOpen = false;
            base.OnSelectionChanged(args);
        }

        /// <summary>
        /// Calls OnTabScrollButtonVisibilityChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabScrollButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabControl instance = (SfTabControl)d;
            SfTabPanel panel = instance.GetTabPanel();
            if (instance!=null && panel != null)
            {
                bool needscrollbuttonshow = panel.m_scrollInfo.NeedScrollButtonsShow;
                if (((instance.TabStripPlacement==TabStripPlacement.Left ||instance.TabStripPlacement==TabStripPlacement.Right) &&
                    panel.m_scrollInfo.AllTrimmedHeight < panel.m_scrollInfo.DesiredHeight / instance.Items.Count)
                    || (((instance.TabStripPlacement == TabStripPlacement.Top || instance.TabStripPlacement == TabStripPlacement.Bottom) &&
                    panel.m_scrollInfo.AllTrimmedWidth < panel.m_scrollInfo.DesiredWidth / instance.Items.Count)))
                    needscrollbuttonshow = false;
                instance.CheckNavigationButtonVisibility(needscrollbuttonshow, panel.ActualWidth);
                instance.ValidateScrollBarVisibility(); 
            }
        }

        /// <summary>
        /// Calls OnPinnedTabScrollButtonVisibilityChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPinnedTabScrollButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabControl instance = (SfTabControl)d;
            SfTabPanel panel = instance.GetPinnedTabPanel();
            if (panel != null)
            {
                bool needscrollbuttonshow = panel.m_scrollInfo.NeedScrollButtonsShow;
                instance.CheckPinnedNavigationButtonVisibility(needscrollbuttonshow, panel.ActualWidth);
            }
        }

        private static void OnShowTabStripMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabControl instance = (SfTabControl) d;
            if (instance.tabStripMenuButton != null && instance.ShowTabstripMenu && instance.Items.Count > 0)
                instance.tabStripMenuButton.Visibility = Visibility.Visible;
            else if(instance.tabStripMenuButton != null)
                instance.tabStripMenuButton.Visibility = Visibility.Collapsed;
            SfTabPanel panel = instance.GetTabPanel();
            if (panel != null)
                instance.CheckCommonButtonGridAlignment(panel.ActualWidth);
        }

        /// <summary>
        /// Calls OnCloseButtonTypeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCloseButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabControl instance = (SfTabControl) d;
            SfTabItem item = null;

            #region CloseButtonVisibilityChecking

            int index = 0;
            foreach (var vitem in instance.Items)
            {
                if (!(vitem is SfTabItem))
                    item = instance.ItemContainerGenerator.ContainerFromItem(vitem) as SfTabItem;
                else
                    item = vitem as SfTabItem;
                if (item != null && item.CanClose)
                {
                    Windows.UI.Xaml.Controls.Primitives.RepeatButton closebutton =
                        item.closeButton as Windows.UI.Xaml.Controls.Primitives.RepeatButton;
                    Windows.UI.Xaml.Controls.Primitives.RepeatButton commonclosebutton =
                        instance.commonCloseButton as Windows.UI.Xaml.Controls.Primitives.RepeatButton;
                    if (closebutton != null)
                    {
                        item.CheckCloseButtonVisibilityOnLoad(closebutton);
                    }

                    if (item.Visibility == Visibility.Collapsed 
                    && instance.pinnedpanel != null && instance.pinnedpanel.Children.Count > 0 && index < instance.pinnedpanel.Children.Count)
                    {
                        item = instance.pinnedpanel.Children[index] as SfTabItem;
                        if(item.closeButton != null)
                            item.CheckCloseButtonVisibilityOnLoad(item.closeButton);
                    }

                    if (commonclosebutton != null && instance.Items.Count != 0)
                        instance.CheckCommonCloseButtonVisibilityOnLoad(commonclosebutton);
                }
                index++;
            }

            #endregion
            
            SfTabPanel panel = instance.GetTabPanel();
            if (panel != null)
            {
                instance.isCloseButtonBoth = (instance.CloseButtonType==CloseButtonType.Both);
                instance.CheckCommonButtonGridAlignment(panel.ActualWidth);
            }
        }

        private static void OnTabControlVerticalTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabControl tab = (SfTabControl)d;
            if (tab != null && (tab.TabStripPlacement == TabStripPlacement.Left || tab.TabStripPlacement == TabStripPlacement.Right) &&
                e.NewValue != null && e.OldValue!=null && !e.OldValue.Equals(e.NewValue))
                tab.Template = e.NewValue as ControlTemplate;
        }

        private static void OnTabControlHorizontalTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabControl tab = (SfTabControl)d;
            if(tab!=null && (tab.TabStripPlacement==TabStripPlacement.Top||tab.TabStripPlacement==TabStripPlacement.Bottom) &&
                e.NewValue!=null && e.OldValue!=null && !e.OldValue.Equals(e.NewValue))
                tab.Template = e.NewValue as ControlTemplate;
        }

        private static void OnSelectionStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfTabControl tab = (SfTabControl)d;
            SfTabItem item = null;
            foreach (var vitem in tab.Items)
            {
                if (!(vitem is SfTabItem))
                    item = tab.ItemContainerGenerator.ContainerFromItem(vitem) as SfTabItem;
                else
                    item = vitem as SfTabItem;
                if (item != null)
                {
                    if (item.IsSelected)
                    {
                        if (tab.SelectionStyle == SelectionStyle.HeaderText)
                            VisualStateManager.GoToState(item, "Normal", true);
                        else
                            VisualStateManager.GoToState(item, "Selected", true);
                    }
                }
            }
            if (tab.PinnedItems != null && tab.pinnedpanel!=null)
            {
                foreach (var tabitem in tab.pinnedpanel.Children)
                {
                    if (!(tabitem is SfTabItem))
                        item = tab.ItemContainerGenerator.ContainerFromItem(tabitem) as SfTabItem;
                    else
                        item = tabitem as SfTabItem;
                    if (item != null)
                    {
                        SfTabItem actualItem=null;
                        if (tab.ItemsSource != null)
                            actualItem = tab.ItemContainerGenerator.ContainerFromIndex(tab.pinnedpanel.Children.IndexOf(item)) as SfTabItem;
                        else
                            actualItem=tab.Items[tab.pinnedpanel.Children.IndexOf(item)] as SfTabItem;
                        if (tab.SelectionStyle == SelectionStyle.HeaderText)
                        {
                            if ((item.SelectedForeground as SolidColorBrush).Color != (actualItem.SelectedForeground as SolidColorBrush).Color)
                                item.SelectedForeground = actualItem.SelectedForeground;
                        }
                        else
                        {
                            if ((item.SelectedBackground as SolidColorBrush).Color != (actualItem.SelectedBackground as SolidColorBrush).Color)
                                item.SelectedBackground = actualItem.SelectedBackground;
                        }
                        if (item.IsSelected)
                        {
                            if (tab.SelectionStyle == SelectionStyle.HeaderText)
                                VisualStateManager.GoToState(item, "Normal", true);
                            else
                                VisualStateManager.GoToState(item, "Selected", true);
                        }
                    }
                }
            }
        }
      
        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SfTabItem;
        }

        /// <summary>
        /// Checks if the item is a <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// </summary>
        /// <returns>Dependency Object</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SfTabItem();
        }
        
        /// <summary>
        /// Arranges the container for overrided items
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            SfTabItem tabItem = element as SfTabItem;
            if (tabItem != null)
            {
                tabItem.parentTabControl = this;
            }
            SfTabItem selectedItem = null;
            if (SelectedItem != null && Items.Contains(SelectedItem))
            {
                if (ItemsSource == null)
                {
                    selectedItem = (SelectedItem as SfTabItem);
                }
                else
                {
                    selectedItem = this.ItemContainerGenerator.ContainerFromItem(SelectedItem) as SfTabItem;
                }
                if (selectedItem != null)
                {
                    SelectedContent = selectedItem.Content;
                    if (!selectedItem.IsSelected)
                        selectedItem.IsSelected = true;
                }
            }
            else
            {
                SelectedItem = null;
                SelectedContent = null;
            }
            if (item is SfTabItem)
            {
                base.PrepareContainerForItemOverride(tabItem, item);
            }
            else
            {
                if (tabItem != null)
                {
                    if (!String.IsNullOrEmpty(DisplayMemberPath))
                    {
                        if (ItemTemplate == null && ItemTemplateSelector == null)
                        {
                            PropertyInfo info = item.GetType().GetRuntimeProperty(DisplayMemberPath);
                            if (info != null)
                            {
                                var header = info.GetValue(item);
                                tabItem.Header = header;
                            }
                        }
                    }
                    else
                    {
                        tabItem.Header = item;
                    }

                    if (tabItem.ContentTemplate == null)
                    {
                        tabItem.SetBinding(
                            ContentControl.ContentTemplateProperty,
                            new Binding()
                            {
                                Source = ContentTemplate,
                                Mode = BindingMode.OneWay
                            });
                    }

                    // potentially set headertemplate if accordionItem did not specify one explicitly
                    if (tabItem.HeaderTemplate == null)
                    {
                        if (ItemTemplate != null)
                            tabItem.SetBinding(SfTabItem.HeaderTemplateProperty,
                                    new Binding()
                                    {
                                        Source = ItemTemplate,
                                        Mode = BindingMode.OneWay
                                    });
                        else
                            tabItem.SetBinding(
                                SfTabItem.HeaderTemplateProperty,
                                new Binding()
                                {
                                    Source = HeaderTemplate,
                                    Mode = BindingMode.OneWay
                                });
                    }

                    if (tabItem.HeaderTemplateSelector == null)
                    {
                        if (tabItem.HeaderTemplate == null)
                        {
                            if (HeaderTemplateSelector != null)
                            {
                                tabItem.HeaderTemplate =
                                    HeaderTemplateSelector.SelectTemplate(item, tabItem) as DataTemplate;
                            }
                            else if(ItemTemplateSelector!=null)
                            {
                                tabItem.HeaderTemplate = ItemTemplateSelector.SelectTemplate(item, tabItem) as DataTemplate;
                                HeaderTemplateSelector = ItemTemplateSelector;
                            }

                        }
                    }

                    tabItem.Content = item;
                    tabItem.ContentTemplateSelector = ContentTemplateSelector;
                }
                base.PrepareContainerForItemOverride(tabItem, tabItem);
            }
        }

        /// <summary>
        /// Called whenever item is added or removed 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(object e)
        {
            if (PinnedItems != null && !internalremove)
            {
                object tabitem = null;
                System.Collections.IList list = ItemsSource as System.Collections.IList;
                foreach (object item in PinnedItems)
                {
                    if ((ItemsSource!=null && list.Contains(item)) ||(item is SfTabItem && !this.Items.Contains(item)))
                        tabitem = item;
                }
                if (tabitem != null)
                    PinnedItems.Remove(tabitem);
            }
            m_itemsChanged = true;
            if (Items.Count == 0 && ShowTabstripMenu && tabStripMenuButton != null && TabstripMenuItems == null)
                tabStripMenuButton.Visibility = Visibility.Collapsed;

            if (SelectedItem != null && !Items.Contains(SelectedItem) && !IsPinnedItemsPanelHasSelectedItems())
            {
                if (pinnedpanel != null && pinnedpanel.Children.Count > 0 && pinnedpanel.Children.Count < Items.Count)
                    SelectedIndex = pinnedpanel.Children.Count - 1;
                else
                {
                    if (Items.Count > 0)
                    {
                        if (SelectedIndex - 1 > 0)
                            SelectedItem = this.Items[SelectedIndex - 1];
                        else
                            SelectedItem = this.Items[0];
                    }
                }
            }
            else if (SelectedItem != null && Items.Contains(SelectedItem))
            {
                int selectedindex = Items.IndexOf(SelectedItem);
                if (selectedindex >= 0 && selectedindex < Items.Count && !selectedindex.Equals(SelectedIndex))
                    SelectedIndex = selectedindex;
            }

            if (Items.Count == 0)
            {
                SelectedContent = null;
                if (commonButtonGrid != null)
                    commonButtonGrid.Visibility = Visibility.Collapsed;
                if (pinnedCommonButtonGrid != null)
                    pinnedCommonButtonGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                if(commonButtonGrid!=null)
                    commonButtonGrid.Visibility = Visibility.Visible;
                if(pinnedCommonButtonGrid!=null)
                    pinnedCommonButtonGrid.Visibility = Visibility.Visible;
                if (ShowTabstripMenu && tabStripMenuButton != null && tabStripMenuButton.Visibility == Visibility.Collapsed)
                    tabStripMenuButton.Visibility = Visibility.Visible;
                if (TabScrollButtonVisibility == TabScrollButtonVisibility.Visible && previousTabButton.Visibility == Visibility.Collapsed && nextTabButton.Visibility == Visibility.Collapsed)
                    previousTabButton.Visibility = nextTabButton.Visibility = Visibility.Visible;
            }
            base.OnItemsChanged(e);
        }

        #endregion

        #region Callback Methods

        private static void OnTabStripPlacementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfTabControl tab = (SfTabControl)obj;
            tab.OnTabStripPlacementChanged(args);
        }

        /// <summary>
        /// Invoked when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.TabStripPlacement"/> TabStripPlacement is changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnTabStripPlacementChanged(DependencyPropertyChangedEventArgs args)
        {
            if(!args.OldValue.Equals(args.NewValue))
                ChangeTabStripPlacement();
            AdjustScrollViewer();
            SfTabPanel panel = GetTabPanel();
            if (panel != null)
            {
                ValidateScrollBarVisibility();
                CheckNavigationButtonVisibility(panel.m_scrollInfo.NeedScrollButtonsShow, panel.ActualWidth);
            }
        }

        internal void AdjustScrollViewer()
        {
            bool horizontalplacement = TabStripPlacement == TabStripPlacement.Left ||
                                       TabStripPlacement == TabStripPlacement.Right
                                           ? false
                                           : true;
            if (PinnedScrollViewer != null)
            {
                if (horizontalplacement)
                {
                    PinnedScrollViewer.ScrollToHorizontalOffset(0);
                    if (PinnedScrollViewer.ViewportWidth > 0)
                    {
                        double completewidth = (PinnedScrollViewer.ViewportWidth +
                            ScrollViewer.ViewportWidth);
                        if (commonButtonGrid != null)
                            completewidth -= pinnedCommonButtonGrid.ActualWidth;
                        PinnedScrollViewer.MaxWidth = completewidth - completewidth / 1.5;
                    }
                }
                else
                {
                    if (PinnedScrollViewer.ViewportHeight > 0)
                    {
                        PinnedScrollViewer.ScrollToVerticalOffset(0);
                        double completeheight = (PinnedScrollViewer.ViewportHeight +
                                                 ScrollViewer.ViewportHeight);
                        if (commonButtonGrid != null)
                            completeheight -= pinnedCommonButtonGrid.ActualHeight;
                        PinnedScrollViewer.MaxHeight = completeheight - completeheight / 1.5;
                    }
                }
            }
        }

        #endregion
    }


    /// <summary>
    /// Represents a class for defining the TabClosed event arguments
    /// </summary>
    public class CloseTabEventArgs : RoutedEventArgs
    {
        private SfTabItem targetTabItem;
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="_targetTabItem"></param>
        public CloseTabEventArgs(SfTabItem _targetTabItem)
        {
            targetTabItem = _targetTabItem;
        }
        /// <summary>
        /// It holds the TabItem to be closed
        /// </summary>
        public SfTabItem TargetTabItem
        {
            get { return targetTabItem; }
            set { targetTabItem = value; }
        }
    }

    /// <summary>
    /// Represents a class for defining the NextTab/PreviousTab event arguments
    /// </summary>
    public class ScrollTabEventArgs : RoutedEventArgs
    {
        private SfTabItem targetTabItem;
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="_targetTabItem"></param>
        public ScrollTabEventArgs(SfTabItem _targetTabItem)
        {
            targetTabItem = _targetTabItem;
        }
        /// <summary>
        /// It holds the TabItem navigated
        /// </summary>
        public SfTabItem TargetTabItem
        {
            get { return targetTabItem; }
            set { targetTabItem = value; }
        }
    }

    /// <summary>
    /// Represents a class for defining the TabClosed event arguments
    /// </summary>
    public class CancelingEventArgs : RoutedEventArgs
    {
        private SfTabItem targetTabItem;
        private bool cancel;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="_targetTabItem"></param>
        public CancelingEventArgs(SfTabItem _targetTabItem)
        {
            targetTabItem = _targetTabItem;
        }
        /// <summary>
        /// It holds the TabItem to be closed
        /// </summary>
        public SfTabItem TargetTabItem
        {
            get { return targetTabItem; }
            set { targetTabItem = value; }
        }

        /// <summary>
        /// Gets and sets a value if the tab item can be closed or not.
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }
}
