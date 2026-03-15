// <copyright file="CustomContextMenu.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Represents a pop-up menu that enables a control to expose functionality that
    /// is specific to the context of the control. By default contains menu items that allow moving the element 
    /// to different dock states.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/vista.aero.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/TransparentStyle.xaml")]  
    public class CustomContextMenu : ContextMenu
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string FloatingItemName = "PART_FloatingMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items.
        /// </summary>
        private const string DockableItemName = "PART_DockableMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items.
        /// </summary>
        private const string TabbedItemName = "PART_TabbedMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items.
        /// </summary>
        private const string AutoHideItemName = "PART_AutoHideMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string HideItemName = "PART_HideMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MaximizeItemName = "PART_MaximizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string MinimizeItemName = "PART_MinimizeMenuItem";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant holds the name of one of the items. 
        /// </summary>
        private const string RestoreItemName = "PART_RetoreMenuItem";

        /////// <property name="flag" value="Finished" />
        /////// <summary>
        /////// This constant holds the content of one of the items. 
        /////// </summary>
        ////private const string FloatingHeaderContent = "Floating";

        /////// <property name="flag" value="Finished" />
        /////// <summary>
        /////// This constant holds the content of one of the items.
        /////// </summary>
        ////private const string DockableHeaderContent = "Dockable";

        /////// <property name="flag" value="Finished" />
        /////// <summary>
        /////// This constant holds the content of one of the items.
        /////// </summary>
        ////private const string TabbedHeaderContent = "Tabbed";

        /////// <property name="flag" value="Finished" />
        /////// <summary>
        /////// This constant holds the content of one of the items.
        /////// </summary>
        ////private const string AutoHideHeaderContent = "Auto Hide";

        /////// <property name="flag" value="Finished" />
        /////// <summary>
        /////// This constant holds the content of one of the items. 
        /////// </summary>
        ////private const string HideHeaderContent = "Hide";

        /// <summary>
        /// Means that menu items cannot contain keyboard focus.
        /// </summary>
        private const bool MenuItemsFocusable = false;
        #endregion

        #region Private member

        /// <summary>
        /// Represent object reference of Resource wrapper class.
        /// </summary>
        ResourceWrapper wrapper = new ResourceWrapper();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for floating.
        /// </summary>
        private CustomMenuItem m_FloatItem = new CustomMenuItem();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for docked.
        /// </summary>
        private  CustomMenuItem m_DockItem = new CustomMenuItem();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for tabbed.
        /// </summary>
        private CustomMenuItem m_TabbedItem = new CustomMenuItem();

        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for auto-hide.
        /// </summary>
        private CustomMenuItem m_AutoHideItem = new CustomMenuItem();

        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for maximize.
        /// </summary>
        private CustomMenuItem m_maximizeItem = new CustomMenuItem();

        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for minimize.
        /// </summary>
        private CustomMenuItem m_minimizeItem = new CustomMenuItem();

        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for restore.
        /// </summary>
        private CustomMenuItem m_restoreItem = new CustomMenuItem();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This variable is a reference to ContextMenu item which is
        /// responsible for hide.
        /// </summary>
        private CustomMenuItem m_HideItem = new CustomMenuItem();

        /// <summary>
        /// This varaible is a reference to language dictionary.
        /// </summary>
        private ResourceDictionary langDictionary;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="CustomContextMenu"/> class.
        /// </summary>
        static CustomContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomContextMenu), new FrameworkPropertyMetadata(typeof(CustomContextMenu)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomContextMenu"/> class.
        /// </summary>
        public CustomContextMenu()
        {
            langDictionary = new ResourceDictionary();
            langDictionary.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Themes/LangDictionary.xaml", UriKind.Relative);
            this.Unloaded += new RoutedEventHandler(CustomContextMenu_Unloaded);
            
            
        }

       

        /// <summary>
        /// Handles the Unloaded event of the CustomContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void CustomContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (m_FloatItem != null)
            {
                m_FloatItem.Items.Clear();
               // m_FloatItem = null;
            }
        }

        /// <summary>
        /// Mins the memory.
        /// </summary>
        public void MinMemory()
        {
            m_DockItem = null;
            m_FloatItem = null;
            m_HideItem = null;
            m_TabbedItem = null;
            langDictionary = null;
        }
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the item which is responsible
        /// for hidden is enabled. 
        /// </summary>
        public bool IsEnabledHiddenMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledHiddenMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledHiddenMenuItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled maximize menu item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled maximize menu item; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabledMaximizeMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledMaximizeMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledMaximizeMenuItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled minimize menu item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled minimize menu item; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabledMinimizeMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledMinimizeMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledMinimizeMenuItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled restore menu item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled restore menu item; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabledRestoreMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledRestoreMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledRestoreMenuItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item which is responsible
        /// for floating is enabled. 
        /// </summary>
        public bool IsEnabledFloatingMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledFloatingMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledFloatingMenuItemProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets a value indicating whether the item which is responsible
        /// for dockable is enabled. 
        /// </summary>
        public bool IsEnabledDockableMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledDockableMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledDockableMenuItemProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets a value indicating whether the item which is responsible
        /// for dockable is enabled. 
        /// </summary>
        public bool IsEnabledTabbedMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledTabbedMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledTabbedMenuItemProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets a value indicating whether the item which is responsible
        /// for auto-hide is enabled. 
        /// </summary>
        public bool IsEnabledAutoHideMenuItem
        {
            get
            {
                return (bool)GetValue(IsEnabledAutoHideMenuItemProperty);
            }

            set
            {
                SetValue(IsEnabledAutoHideMenuItemProperty, value);
            }
        }

        /// <summary>
        /// Gets the target element this instance of CustomContextMenu belongs to.
        /// </summary>
        public FrameworkElement TargetElement
        {
            get
            {
                return GetTargetElement();
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Determines whether the specified item is, or is eligible to be, its own item container.
        /// </summary>
        /// <param name="item">The item to check whether it is an item container.</param>
        /// <returns>
        /// true if the item is a <see cref="System.Windows.Controls.MenuItem"/> or a <see cref="System.Windows.Controls.Separator"/> otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CustomMenuItem;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Creates or identifies the element used to display the
        /// specified item.
        /// </summary>
        /// <returns>
        /// A TabItem.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomMenuItem();
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises the Initialized event. This method is invoked
        /// whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event
        /// data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            InitalDefaultValueForItem();
            ReSetCustomMenuItem(null);
            SetBindingToItems();
        }

        protected override void OnClosed(RoutedEventArgs e)
        {
            base.OnClosed(e);
            //this.Items.Clear();
        }
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Called when the Opened event occurs.
        /// </summary>
        /// <param name="e">The event data for the Opened event.</param>
        internal Visibility CustomContextMenuvisibility = Visibility.Visible; 
        protected override void OnOpened(RoutedEventArgs e)
        {
            base.OnOpened(e);
            if (this.TemplatedParent is NativeFloatWindow)
            {
                FrameworkElement element = (this.TemplatedParent as NativeFloatWindow).PrimaryElement;
                if (element != null)
                {
                    CustomMenuItemCollection itemCollection = DockingManager.GetCustomMenuItems(element);
                    if (DockingManager.GetCollapseDefaultContextMenuItemsInFloat(element) || ((this.TemplatedParent as NativeFloatWindow).DockingManager != null && (this.TemplatedParent as NativeFloatWindow).DockingManager.CollapseDefaultContextMenuItems))
                    {
                        CustomContextMenuvisibility = Visibility.Collapsed;
                    }
                    ReSetCustomMenuItem(itemCollection);
                    switch (DockingManager.GetState(element))
                    {
                        case DockState.Dock:
                            CustomMenuItemCollection dockitemcollection = DockingManager.GetDockWindowContextMenuItems(element);
                            if (dockitemcollection != null && dockitemcollection.Count > 0)
                            {
                                ReSetCustomMenuItem(dockitemcollection);
                            }
                            break;
                        case DockState.Float:
                            CustomMenuItemCollection floatitemcollection = DockingManager.GetFloatWindowContextMenuItems(element);
                            if (floatitemcollection != null && floatitemcollection.Count > 0)
                            {
                                ReSetCustomMenuItem(floatitemcollection);
                            }
                            break;
                    }
                }
            }
            else
            {
                if (null != DockingManager.GetInternalDataContext(this))
                {
                    FrameworkElement element = GetTargetElement();
                    CustomMenuItemCollection itemCollection = DockingManager.GetCustomMenuItems(element);
                    if ((DockingManager.GetCollapseDefaultContextMenuItemsInFloat(element)) || ((this.TemplatedParent as FloatWindow) != null && (this.TemplatedParent as FloatWindow).DockingManager != null && (this.TemplatedParent as FloatWindow).DockingManager.CollapseDefaultContextMenuItems)) 
                    {
                        CustomContextMenuvisibility = Visibility.Collapsed;
                    }
                    ReSetCustomMenuItem(itemCollection);
                    switch (DockingManager.GetState(element))
                    {
                        case DockState.Dock:
                            CustomMenuItemCollection dockitemcollection = DockingManager.GetDockWindowContextMenuItems(element);
                            if (dockitemcollection != null && dockitemcollection.Count > 0)
                            {
                                ReSetCustomMenuItem(dockitemcollection);
                            }
                            break;
                        case DockState.Float:
                            CustomMenuItemCollection floatitemcollection = DockingManager.GetFloatWindowContextMenuItems(element);
                            if (floatitemcollection != null && floatitemcollection.Count > 0)
                            {
                                ReSetCustomMenuItem(floatitemcollection);
                            }
                            break;
                    }
                }
            }
          
            //ClearValue(SkinStorage.VisualStyleProperty);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method initializes default value for item.
        /// </summary>
        private void InitalDefaultValueForItem()
        {
            m_FloatItem.Name = FloatingItemName;
            ////m_FloatItem.Header = FloatingHeaderContent;
            m_FloatItem.Header = wrapper.Floating; //langDictionary["Floating"];
            m_FloatItem.Focusable = MenuItemsFocusable;
            

            m_DockItem.Name = DockableItemName;
            ////m_DockItem.Header = DockableHeaderContent;
            m_DockItem.Header = wrapper.Dockable;  //langDictionary["Dockable"];
            m_DockItem.Focusable = MenuItemsFocusable;
           

            m_TabbedItem.Name = TabbedItemName;
            ////m_TabbedItem.Header = TabbedHeaderContent;
            m_TabbedItem.Header = wrapper.Tabbed; //langDictionary["Tabbed"];
            m_TabbedItem.Focusable = MenuItemsFocusable;
            

            m_AutoHideItem.Name = AutoHideItemName;
            ////m_AutoHideItem.Header = AutoHideHeaderContent;
            m_AutoHideItem.Header = wrapper.AutoHide; //langDictionary["AutoHide"];
            m_AutoHideItem.Focusable = MenuItemsFocusable;

            m_HideItem.Name = HideItemName;
            ////m_HideItem.Header = HideHeaderContent;
            m_HideItem.Header = wrapper.Hide; //langDictionary["Hide"];
            m_HideItem.Focusable = MenuItemsFocusable;

            m_maximizeItem.Name = MaximizeItemName;
            m_maximizeItem.Header = wrapper.Maximize;
            m_maximizeItem.Focusable = MenuItemsFocusable;

            m_minimizeItem.Name = MinimizeItemName;
            m_minimizeItem.Header = wrapper.Minimize;
            m_minimizeItem.Focusable = MenuItemsFocusable;

            m_restoreItem.Name = RestoreItemName;
            m_restoreItem.Header = wrapper.Restore;
            m_restoreItem.Focusable = MenuItemsFocusable;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method sets binding for some items.
        /// </summary>
        private void SetBindingToItems()
        {
            BindingUtils.SetBinding(m_AutoHideItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledAutoHideMenuItemProperty);
            BindingUtils.SetBinding(m_HideItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledHiddenMenuItemProperty);
            BindingUtils.SetBinding(m_DockItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledDockableMenuItemProperty);
            BindingUtils.SetBinding(m_FloatItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledFloatingMenuItemProperty);
            BindingUtils.SetBinding(m_TabbedItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledTabbedMenuItemProperty);
            BindingUtils.SetBinding(m_maximizeItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledMaximizeMenuItemProperty);
            BindingUtils.SetBinding(m_minimizeItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledMinimizeMenuItemProperty);
            BindingUtils.SetBinding(m_restoreItem, this, UIElement.IsEnabledProperty, CustomContextMenu.IsEnabledRestoreMenuItemProperty);

            FrameworkElement element = GetTargetElement();
           
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method resets items in instance.
        /// </summary>
        /// <param name="customItemCollection">Items collection which
        /// should be reset.</param>
        private void ReSetCustomMenuItem(CustomMenuItemCollection customItemCollection)
        {
            BeginInit();

         
            Style menuitemstyle = null;
            string skin = SkinStorage.GetVisualStyle(this);
            ResourceDictionary rd = new ResourceDictionary();

            if (this.TemplatedParent is NativeFloatWindow)
            {
                if ((this.TemplatedParent as NativeFloatWindow).DockingManager != null)
                {
                    if ((this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate != null)
                    {
                        m_FloatItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                        m_DockItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                        m_TabbedItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                        m_AutoHideItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                        m_HideItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                        m_restoreItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                        m_maximizeItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                        m_minimizeItem.HeaderTemplate = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockedElementContextMenuItemTemplate;
                    }
                    menuitemstyle = (this.TemplatedParent as NativeFloatWindow).DockingManager.DockWindowContextMenuItemStyle;
                }
            }
            else
            {
                if (this.GetTargetElement() != null)
                {
                    DockingManager dockManager = (this.GetTargetElement() as FrameworkElement).Parent as DockingManager;
                    if (dockManager != null)
                        menuitemstyle = dockManager.DockWindowContextMenuItemStyle;
                }
            }
            Items.Clear();
            Items.Add(m_FloatItem);
            Items.Add(m_DockItem);
            Items.Add(m_TabbedItem);
            Items.Add(m_AutoHideItem);
            Items.Add(m_HideItem);
            Items.Add(m_restoreItem);
            Items.Add(m_minimizeItem);
            Items.Add(m_maximizeItem);
            if (Items != null)
            {
                foreach (CustomMenuItem menuitem in Items)
                {
                    menuitem.Visibility = CustomContextMenuvisibility;
                }
            }
            bool CanAllowCustomMenuItems = true;

            FrameworkElement element = GetTargetElement();
            if (this.TemplatedParent is NativeFloatWindow && element==null)
                element = (this.TemplatedParent as NativeFloatWindow).PrimaryElement;
            if (element != null && (element as DockingManager)!=null)
            {
                DockedElementTabbedHost host = VisualUtils.FindAncestor(element, typeof(DockedElementTabbedHost)) as DockedElementTabbedHost;
                if (host != null)
                {
                    if (host.DockingManager != null)
                    {
                        if (host.DockingManager.m_custommenuitems == null)
                        {
                            CanAllowCustomMenuItems = false;
                        }
                        else
                        {
                            if (host.DockingManager.m_custommenuitems.Count > 0)
                            {
                                customItemCollection = host.DockingManager.m_custommenuitems;
                            }
                        }
                    }
                }
            }

            DockingManager manager = null;

            if(element!=null)
            {
                //bool m_updatevisibility = false;
                manager = VisualUtils.FindAncestor(element, typeof(DockingManager)) as DockingManager;
                if (manager != null)
                {
                    UpdateVisiblity(manager.CollapseDefaultContextMenuItems || DockingManager.GetCollapseDefaultContextMenuItemsInDock(element as DependencyObject), manager);
                }
                else
                {
                    manager = DockingManager.ResolveManager(element);
                    if (manager != null)
                    {
                        UpdateVisiblity(manager.CollapseDefaultContextMenuItems || DockingManager.GetCollapseDefaultContextMenuItemsInFloat(element as DependencyObject), manager);
                    }
                }
            }

           

            if (null != customItemCollection&&CanAllowCustomMenuItems)
            {
                int customItemCount = customItemCollection.Count;

                if (menuitemstyle != null)
                {
                    m_AutoHideItem.Style = menuitemstyle;
                    m_DockItem.Style = menuitemstyle;
                    m_FloatItem.Style = menuitemstyle;
                    m_HideItem.Style = menuitemstyle;
                    m_maximizeItem.Style = menuitemstyle;
                    m_minimizeItem.Style = menuitemstyle;
                    m_restoreItem.Style = menuitemstyle;
                    m_TabbedItem.Style = menuitemstyle;
                }

                if (0 != customItemCount)
                {
                    CustomContextMenu parent = customItemCollection[0].Parent as CustomContextMenu;

                    for (int i = 0; i < customItemCount; ++i)
                    {
                        CustomMenuItem item = customItemCollection[i] as CustomMenuItem;

                        if (null != parent)
                        {
                            parent.RemoveLogicalChild(item);
                        }
                        if (customItemCollection[i].Parent != null && customItemCollection[i].Parent is ContextMenu)
                        {
                            ContextMenu parent1 = customItemCollection[i].Parent as ContextMenu;
                            parent1.Items.Remove(item);
                        }

                        item.Focusable = MenuItemsFocusable;
                        if (menuitemstyle != null)
                        {
                            item.Style = menuitemstyle;
                            if (item.Items.Count > 0)
                            {
                                CheckInternalItems(item, menuitemstyle);
                            }
                            item.UpdateLayout();
                        }
                        Items.Add(item);
                    }
                }
            }
            foreach (CustomMenuItem menuitem in Items)            
               {
                if (menuitem.Visibility == Visibility.Visible)        
                {                
                  Visibility = Visibility.Visible;      
                    break;              
                  }               
                 else     
                 Visibility = Visibility.Collapsed;           
               }

            EndInit();
        }

        /// <summary>
        /// Checks the internal items.
        /// </summary>
        /// <param name="menuitem">The menuitem.</param>
        /// <param name="menuitemstyle">The menuitemstyle.</param>
        internal void CheckInternalItems(CustomMenuItem menuitem,Style menuitemstyle)
        {
            for (int i = 0; i < menuitem.Items.Count; i++)
            {
                CustomMenuItem menuchilditem = menuitem.Items[i] as CustomMenuItem;
                menuchilditem.Style = menuitemstyle;
                if (menuchilditem.Items.Count > 0)
                {
                    CheckInternalItems(menuchilditem, menuitemstyle);
                }
                else
                {
                    menuchilditem.Style = menuitemstyle;
                    menuchilditem.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Updates the visiblity.
        /// </summary>
        /// <param name="isvisible">if set to <c>true</c> [isvisible]</param>
        private void UpdateVisiblity(bool isvisible, DockingManager owner)
        {
            FrameworkElement element = GetTargetElement();
            if (this.TemplatedParent is NativeFloatWindow && element == null)
                element = (this.TemplatedParent as NativeFloatWindow).PrimaryElement;
            SetVisiblity(m_AutoHideItem, isvisible);
            SetVisiblity(m_DockItem, isvisible);
            SetVisiblity(m_FloatItem, isvisible);
            SetVisiblity(m_HideItem, isvisible);
            SetVisiblity(m_TabbedItem, isvisible);
            SetVisiblity(m_restoreItem, isvisible);
            SetVisiblity(m_maximizeItem, isvisible);
            SetVisiblity(m_minimizeItem, isvisible);

            if (!isvisible)
            {
                SetVisiblity(m_AutoHideItem, !DockingManager.GetShowAutoHiddenMenuItem(element));
                SetVisiblity(m_DockItem, !DockingManager.GetShowDockableMenuItem(element));
                SetVisiblity(m_FloatItem, !DockingManager.GetShowFloatingMenuItem(element));
                SetVisiblity(m_HideItem, !DockingManager.GetShowHiddenMenuItem(element));
                SetVisiblity(m_TabbedItem, !DockingManager.GetShowTabbedMenuItem(element));
                SetVisiblity(m_restoreItem, !DockingManager.GetShowRestoreMenuItem(element));
                SetVisiblity(m_maximizeItem, !DockingManager.GetShowMaximizedMenuItem(element));
                SetVisiblity(m_minimizeItem, !DockingManager.GetShowMinimizeMenuItem(element));
            }

            if (!owner.MaximizeButtonEnabled)
            {
                SetVisiblity(m_restoreItem, true);
                SetVisiblity(m_maximizeItem, true);
            }

            if (!owner.MinimizeButtonEnabled)
            {
                SetVisiblity(m_minimizeItem, true);
            }

            if (!owner.UseDocumentContainer || !(DockingManager.GetCanDocument(element)))
            {
                    IsEnabledTabbedMenuItem = false;
            }
            else if(owner.UseDocumentContainer)
            {
                if(DockingManager.GetCanDocument(element))                  
                        IsEnabledTabbedMenuItem = true;
            }

            if ((DockingManager.GetCanDock(element)))
            {
                IsEnabledDockableMenuItem = true;
            }
            else 
            {                
                    IsEnabledDockableMenuItem = false;
            }

            if (DockingManager.GetDockWindowState(element as DependencyObject) == WindowState.Maximized)
            {
                if (IsEnabledMaximizeMenuItem)
                {
                    IsEnabledMaximizeMenuItem = false;
                }
                IsEnabledRestoreMenuItem = true;
            }
            else
            {
                IsEnabledRestoreMenuItem = false;
            }
        }

        private void SetVisiblity(CustomMenuItem item, bool visiblity)
        {
            if (visiblity)
            {
                item.Visibility = Visibility.Collapsed;
            }
            else
            {
                if(item!=null)
                item.Visibility=Visibility.Visible;
            }
        }
        /// <summary>
        /// Gets the target element.
        /// </summary>
        /// <returns>return targetElement.</returns>
        private FrameworkElement GetTargetElement()
        {
            FrameworkElement targetElement;
            FrameworkElement internalDataContext = DockingManager.GetInternalDataContext(this);

            if (DockingManager.ResolveManager(this) != null)
            {
                DockingManager docmanager = DockingManager.ResolveManager(this);
                if (docmanager.DockedElementContextMenuItemTemplate != null)
                {
                    m_FloatItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                    m_DockItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                    m_TabbedItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                    m_AutoHideItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                    m_HideItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                    m_restoreItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                    m_maximizeItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                    m_minimizeItem.HeaderTemplate = docmanager.DockedElementContextMenuItemTemplate;
                }
            }

            if (internalDataContext is DockedElementTabbedHost)
            {
                DockedElementTabbedHost host = (DockedElementTabbedHost)internalDataContext;
                targetElement = host.InternalDataContext;
            }
            else
            {
                targetElement = internalDataContext;
            }

            return targetElement;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        #endregion

        #region Dependency Property

        /// <summary>
        /// Identifies the <see cref="IsEnabledHiddenMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledHiddenMenuItemProperty =
            DependencyProperty.Register("IsEnabledHiddenMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsEnabledMaximizeMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledMaximizeMenuItemProperty =
            DependencyProperty.Register("IsEnabledMaximizeMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsEnabledMinimizeMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledMinimizeMenuItemProperty =
            DependencyProperty.Register("IsEnabledMinimizeMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsEnabledRestoreMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledRestoreMenuItemProperty =
            DependencyProperty.Register("IsEnabledRestoreMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsEnabledFloatingMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledFloatingMenuItemProperty =
            DependencyProperty.Register("IsEnabledFloatingMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsEnabledDockableMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledDockableMenuItemProperty =
            DependencyProperty.Register("IsEnabledDockableMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsEnabledAutoHideMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledAutoHideMenuItemProperty =
            DependencyProperty.Register("IsEnabledAutoHideMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsEnabledTabbedMenuItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledTabbedMenuItemProperty =
            DependencyProperty.Register("IsEnabledTabbedMenuItem", typeof(bool), typeof(CustomContextMenu), new UIPropertyMetadata(true));

        #endregion
    }
}