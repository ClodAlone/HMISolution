// <copyright file="RibbonContextMenu.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a pop-up menu that enables a control to expose
    /// functionality that is specific to the context of the control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonContextMenu : ContextMenu, IDisposable
    {
        #region Private Members


        /// <summary>
        /// last captured element
        /// </summary>
        private static FrameworkElement m_lastCapturedElement;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="RibbonContextMenu"/> class.
        /// </summary>
        static RibbonContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextMenu), new FrameworkPropertyMetadata(typeof(RibbonContextMenu)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonContextMenu"/> class.
        /// </summary>
        public RibbonContextMenu()
        {
            if (!(Mouse.Captured is ContextMenu) && Mouse.Captured != null)
            {
                m_lastCapturedElement = Mouse.Captured as FrameworkElement;
            }
            this.Unloaded -= new RoutedEventHandler(RibbonContextMenu_Unloaded);
            this.Unloaded += new RoutedEventHandler(RibbonContextMenu_Unloaded);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnPreviewMouseDownOutsideCapturedElement);
        }

        void RibbonContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the last captured element.
        /// </summary>
        /// <value>The last captured element.</value>
        internal static FrameworkElement LastCapturedElement
        {
            get
            {
                return m_lastCapturedElement;
            }
        }

        #endregion

        /// <summary>
        /// Add custom context menu item in existing Ribobn context menu. This is a attached property.
        /// </summary>
        public static readonly DependencyProperty CustomContextMenuItemsProperty =
         DependencyProperty.RegisterAttached("CustomContextMenuItems", typeof(RibbonCustomContextMenuItems), typeof(RibbonContextMenu), new FrameworkPropertyMetadata(new RibbonCustomContextMenuItems()));

        /// <summary>
        /// Add custom context menu item in existing Ribobn context menu. This is a attached property.
        /// </summary>
        public static readonly DependencyProperty IsCustomContextMenuItemsOnTopProperty =
         DependencyProperty.RegisterAttached("IsCustomContextMenuItemsOnTop", typeof(bool), typeof(RibbonContextMenu), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets the custom context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static RibbonCustomContextMenuItems GetCustomContextMenuItems(DependencyObject obj)
        {
            return (RibbonCustomContextMenuItems)obj.GetValue(CustomContextMenuItemsProperty);
        }

        /// <summary>
        /// Sets the custom context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetCustomContextMenuItems(DependencyObject obj, RibbonCustomContextMenuItems value)
        {
            obj.SetValue(CustomContextMenuItemsProperty, value);
        }


        /// <summary>
        /// Gets the is custom context menu items on top.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsCustomContextMenuItemsOnTop(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCustomContextMenuItemsOnTopProperty);
        }

        /// <summary>
        /// Sets the is custom context menu items on top.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsCustomContextMenuItemsOnTop(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCustomContextMenuItemsOnTopProperty, value);
        }

        // work around.

        #region localfields

        /// <summary>
        /// Represents the RibbonMenuitem number zero
        /// </summary>
        RibbonMenuItem itemzero = null;

        /// <summary>
        /// Represents the RibbonMenuitem number one
        /// </summary>
        RibbonMenuItem itemone = null;

        /// <summary>
        /// Represents the seperator number two
        /// </summary>
        Separator itemtwo = null;

        /// <summary>
        /// Represents the RibbonMenuitem number three
        /// </summary>
        RibbonMenuItem itemthree = null;

        /// <summary>
        /// Represents the RibbonMenuitem number four
        /// </summary>
        RibbonMenuItem itemfour = null;

        /// <summary>
        /// Represents the RibbonMenuitem number five
        /// </summary>
        RibbonMenuItem itemfive = null;

        /// <summary>
        /// Represents the RibbonMenuitem number six
        /// </summary>
        Separator itemsix = null;

        /// <summary>
        /// Represents the RibbonMenuitem number seven
        /// </summary>
        RibbonMenuItem itemseven = null;

        /// <summary>
        /// Represents the ribbon
        /// </summary>
        Ribbon ribbon = null;

        /// <summary>
        /// Represents the visual root framework element
        /// </summary>
        FrameworkElement visualRoot = null;


        #endregion

        #region Implementation

        /// <summary>
        /// Overrides OnOpened method to perform custom action of menu
        /// elements.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnOpened(RoutedEventArgs e)
        {
            if (this.Items.Count <= 0)
            {
                this.ItemsSource = null;
                ResourceDictionary rd = new ResourceDictionary() { Source = new Uri("/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute) };
                ItemsControl newitems = rd["RibbonContextMenuItems"] as ItemsControl;
                this.ItemsSource = newitems.ItemsSource;
            }
            FrameworkElement m_target;
            UpdateObjectReferences();
            bool showFullMenu = true;
            m_target = (e.Source as RibbonContextMenu).PlacementTarget as FrameworkElement;
            if (ribbon == null)
            {
                ribbon = FindRibbon(m_target);
            }
            if (visualRoot == null)
            {
                visualRoot = FindVisualRoot(m_target);
            }

            if (ribbon != null)
            {
                itemseven.IsChecked = ribbon.RibbonState != RibbonState.Normal ? true : false;
                if (ribbon.QuickAccessToolBar != null)
                {
                    if (ribbon.QuickAccessToolBar.HasGeometry)
                    {
                        itemfour.Visibility = Visibility.Collapsed;
                        itemfive.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        itemfour.Visibility = Visibility.Visible;
                        itemfive.Visibility = Visibility.Collapsed;
                    }
                }
            }

            if (!(m_target is Ribbon) && !(m_target is RibbonTab) && !(m_target is QuickAccessToolBar) && !(m_target is ButtonPanel) && showFullMenu)
            {
                if ((m_target as FrameworkElement).Parent is QuickAccessToolBar)
                {
                    itemzero.Visibility = Visibility.Visible;
                    itemzero.CommandTarget = PlacementTarget;
                    itemtwo.Visibility = Visibility.Visible;
                    itemone.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bool isAlreadyInQAT = false;
                    if (ribbon != null && ribbon.QuickAccessToolBar != null)
                    {
                        foreach (var item in ribbon.QuickAccessToolBar.Items)
                        {
                            object obj = Ribbon.GetRibbonQATCommandTag(item as DependencyObject);
                            if(obj != null)
                            {
                            if (obj.Equals(Ribbon.GetRibbonQATCommandTag(m_target)))
                            {
                                isAlreadyInQAT = true;
                                break;
                            }
                            }
                           
                        }
                    }
                    if (ribbon != null && ribbon.QuickAccessToolBar != null && Ribbon.GetIsQATItem(m_target) && ribbon.QuickAccessToolBar.InternalCommandManager.CanAdd(m_target) && !isAlreadyInQAT)
                    {
                        itemone.IsEnabled = true;
                    }
                    else
                    {
                        itemone.IsEnabled = false;
                    }

                    itemone.Visibility = Visibility.Visible;
                    itemone.CommandTarget = PlacementTarget;
                    itemtwo.Visibility = Visibility.Visible;
                    itemzero.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                itemzero.Visibility = Visibility.Collapsed;
                itemone.Visibility = Visibility.Collapsed;
                itemtwo.Visibility = Visibility.Collapsed;
            }

            //Shared.DictionaryList themeDictionary = SkinStorage.GetVisualStylesList(this);
            //string skin = SkinStorage.GetVisualStyle(this);

            //if (themeDictionary != null)
            //{
            //    foreach (DependencyObject item in Items)
            //    {
            //        if (SkinStorage.GetVisualStylesList(item) == null)
            //        {
            //            SkinStorage.SetVisualStylesList(item, themeDictionary);
            //        }

            //        SkinStorage.SetVisualStyle(item, skin);
            //    }
            //}

            if (itemzero != null && itemone != null && itemtwo != null)
            {
                if (itemzero.Visibility == System.Windows.Visibility.Collapsed && itemone.Visibility == System.Windows.Visibility.Collapsed)
                {
                    itemtwo.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            bool hasNoItems = false;
            if (itemthree != null && itemfour != null && itemfive != null && itemsix != null)
            {
                if (itemthree.Visibility == System.Windows.Visibility.Collapsed && itemfour.Visibility == System.Windows.Visibility.Collapsed && itemfive.Visibility == System.Windows.Visibility.Collapsed)
                {                 
                    itemsix.Visibility = System.Windows.Visibility.Collapsed;
                    hasNoItems = true;
                }
            }

            if (itemseven != null && itemsix != null)
            {
                if (itemseven.Visibility == System.Windows.Visibility.Collapsed)
                {
                    itemsix.Visibility = Visibility.Collapsed;
                    if (hasNoItems)
                    {
                        if (itemtwo != null)
                            itemtwo.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
            
            UpdateMenuItemsFlowDirection(visualRoot);

            base.OnOpened(e);

        }

        /// <summary>
        /// Removes the object reference.
        /// </summary>
        private void RemoveObjectReference()
        {
            itemzero = null;
            itemone = null;
            itemtwo = null;
            itemthree = null;
            itemfour = null;
            itemfive = null;
            itemsix = null;
            itemseven = null;
        }

        /// <summary>
        /// Updates the object references.
        /// </summary>
        private void UpdateObjectReferences()
        {

            foreach (object obj in this.ItemsSource)
            {
                if (obj is RibbonMenuItem)
                {
                    RibbonMenuItem temp = obj as RibbonMenuItem;
                    if (temp.Tag != null)
                    {
                        if (temp.Tag.Equals("Remove"))
                        {
                            itemzero = obj as RibbonMenuItem;
                        }
                        else if (temp.Tag.Equals("Item_One"))
                        {
                            itemone = obj as RibbonMenuItem;
                        }
                        else if (temp.Tag.Equals("Item_Three"))
                        {
                            itemthree = obj as RibbonMenuItem;
                        }
                        else if (temp.Tag.Equals("Item_Four"))
                        {
                            itemfour = obj as RibbonMenuItem;
                        }
                        else if (temp.Tag.Equals("Item_Five"))
                        {
                            itemfive = obj as RibbonMenuItem;
                        }
                        else if (temp.Tag.Equals("Item_Seven"))
                        {
                            itemseven = obj as RibbonMenuItem;
                        }
                    }
                }
                if (obj is Separator)
                {
                    Separator temp = obj as Separator;

                    if (temp.Tag != null)
                    {
                        if (temp.Tag.Equals("Item_Two"))
                        {
                            itemtwo = obj as Separator;
                        }
                        else if (temp.Tag.Equals("Item_Six"))
                        {
                            itemsix = obj as Separator;
                        }
                    }

                }

            }

        }

        /// <summary>
        /// Sets flow direction of the visual root to the menu item.
        /// </summary>
        /// <param name="visualRoot">visual root</param>
        private void UpdateMenuItemsFlowDirection(FrameworkElement visualRoot)
        {
            for (int i = 0, cnt = Items.Count; i < cnt; i++)
            {
                RibbonMenuItem item = Items[i] as RibbonMenuItem;
                if (item != null && i == cnt - 1)
                {
                    if (this.ribbon.RibbonState == RibbonState.Adorner || this.ribbon.RibbonState == RibbonState.Normal)
                    {
                        (item.Icon as UIElement).Visibility = System.Windows.Visibility.Hidden;
                    }
                    else if (this.ribbon.RibbonState == RibbonState.Hide)
                    {
                        (item.Icon as UIElement).Visibility = System.Windows.Visibility.Visible;
                    }
                }

                if (item != null)
                {
                    item.FlowDirection = visualRoot.FlowDirection;
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.ContextMenu.Closed"/> event occurs.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ContextMenu.Closed"/> event.</param>
        protected override void OnClosed(RoutedEventArgs e)
        {
            //if (this.StaysOpen)
            //    return;
            Ribbon ribbon = null;

            if (PlacementTarget is Ribbon)
            {
                ribbon = PlacementTarget as Ribbon;
            }
            else
            {
                ribbon = VisualUtils.FindAncestor(PlacementTarget, typeof(Ribbon)) as Ribbon;
            }
            if (ribbon == null)
            {
                ribbon = FindRibbon(PlacementTarget as FrameworkElement);
            }
            
            ContextMenuEventArgs args = null;

            // Ribbon contextmenu opening event
            if (ribbon != null)
            {
                args = (ContextMenuEventArgs)FormatterServices.GetUninitializedObject(typeof(ContextMenuEventArgs));
                args.RoutedEvent = Ribbon.RibbonContextMenuClosingEvent;
                args.Source = e.Source;
                ribbon.FireRibbonContextMenuClosing(args);
            }

            if (ribbon!=null && ribbon.ContextMenu !=null && ribbon.ContextMenu.IsOpen == true)
                ribbon.ContextMenu.IsOpen = false;
            base.OnClosed(e);
            (PlacementTarget as FrameworkElement).ContextMenu = null;
            RemoveObjectReference();
            //Mouse.Capture(null);
        }

        /// <summary>
        /// Called when a <see cref="E:System.Windows.ContentElement.KeyDown"/> event is raised by an object inside the <see cref="T:System.Windows.Controls.ContextMenu"/>.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.KeyDown"/> event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (Keyboard.FocusedElement != null && Keyboard.FocusedElement.GetType() == typeof(RibbonContextMenu))
                {
                    if (PlacementTarget != null)
                    {
                        PlacementTarget = null;
                    }
                    return;
                }
                else
                {
                    Keyboard.Focus(m_lastCapturedElement);
                    if (Mouse.Captured == null)
                    {
                        Mouse.Capture(m_lastCapturedElement);
                    }
                }

                this.IsOpen = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when [preview mouse down outside captured element].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            if (m_lastCapturedElement != null && m_lastCapturedElement.IsMouseOver == false)
            {
                if (m_lastCapturedElement is RibbonItemsControl)
                {
                    (m_lastCapturedElement as RibbonItemsControl).IsDropDownOpen = false;
                }
                else
                {
                    Mouse.Capture(m_lastCapturedElement, CaptureMode.SubTree);
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (m_lastCapturedElement != null && m_lastCapturedElement.IsMouseOver)
                {
                    Mouse.Capture(m_lastCapturedElement, CaptureMode.SubTree);
                    Keyboard.Focus(m_lastCapturedElement);
                }
            }
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <param name="source">The source.</param>
        internal static void CreateContextMenu(FrameworkElement source)
        {
            Ribbon ribbon = null;

            if (source is Ribbon)
            {
                ribbon = source as Ribbon;
            }
            else
            {
                ribbon = VisualUtils.FindAncestor(source, typeof(Ribbon)) as Ribbon;
            }

            if (ribbon == null)
            {
                ribbon= FindRibbon(source);
            }

            RibbonContextMenu contextMenu = new RibbonContextMenu();
                ContextMenuEventArgs args = null;
                if (ribbon != null)
                {
                    args = (ContextMenuEventArgs)FormatterServices.GetUninitializedObject(typeof(ContextMenuEventArgs));
                    args.RoutedEvent = Ribbon.RibbonContextMenuOpeningEvent;
                    args.Source = contextMenu;
                    ribbon.FireRibbonContextMenuOpening(args);
                }
              if (args != null && !args.Handled && args.Source != null)
                {
                    source.ContextMenu = args.Source as ContextMenu;
                    source.ContextMenu.PlacementTarget = source;
                    if (source.ContextMenu.IsOpen == false)
                        source.ContextMenu.IsOpen = true;
              }
              if (source.ContextMenu != null)
              {
                  foreach (var item in source.ContextMenu.Items)
                  {
                      if (item is RibbonMenuItem)
                      {
                          //if (!SkinStorage.GetEnableTouch(item as RibbonMenuItem))
                          //{
                          SkinStorage.SetEnableTouch(item as RibbonMenuItem, SkinStorage.GetEnableTouch(source.ContextMenu));
                          //}
                      }
                      if (item is Separator)
                      {
                          SkinStorage.SetEnableTouch(item as Separator, SkinStorage.GetEnableTouch(source.ContextMenu));
                      }
                  }
              }
              if (source.ContextMenu != null)
              {
                  IList collectionview = source.ContextMenu.ItemsSource as IList;

                  RibbonCustomContextMenuItems customMenuItems = RibbonContextMenu.GetCustomContextMenuItems(source) as RibbonCustomContextMenuItems;

                  int i = 0;

                  foreach (object obj in customMenuItems)
                  {

                      if (!collectionview.Contains(obj))
                      {
                          if (!RibbonContextMenu.GetIsCustomContextMenuItemsOnTop(ribbon))
                          {
                              collectionview.Add(obj);
                          }
                          else
                          {
                              collectionview.Insert(i, obj);
                              i++;
                          }
                      }

                  }
              }   //source.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// Finds the ribbon.
        /// </summary>
        /// <param name="m_target">The m_target.</param>
        /// <returns></returns>
        private static Ribbon FindRibbon(FrameworkElement m_target)
        {

            Ribbon ribbon = null;

            FrameworkElement visualRoot = VisualUtils.FindRootVisual(m_target as Visual) as FrameworkElement;

            Visual logicalParent = null;

            if (VisualUtils.RootPopupType == visualRoot.GetType())
            {
                visualRoot = visualRoot.Parent as FrameworkElement;
                while (visualRoot != null && !(visualRoot is Ribbon) && !(visualRoot is ApplicationMenu))
                {
                    if (visualRoot.Parent != null)
                    {
                        visualRoot = visualRoot.Parent as FrameworkElement;
                    }
                    else
                    {
                        visualRoot = visualRoot.TemplatedParent as FrameworkElement;
                    }
                }

                if (visualRoot != null)
                {
                    if (visualRoot is Ribbon)
                    {
                        ribbon = visualRoot as Ribbon;
                    }
                    else
                    {
                        ribbon = VisualUtils.FindAncestor(visualRoot as Visual, typeof(Ribbon)) as Ribbon;
                    }
                }
            }

            if (ribbon == null)
            {
                if (visualRoot == null)
                {
                    visualRoot = m_target as FrameworkElement;
                }

                if (m_target is Ribbon)
                {
                    ribbon = m_target as Ribbon;
                }
                else
                {
                    ribbon = VisualUtils.FindAncestor(m_target, typeof(Ribbon)) as Ribbon;
                }

                if (ribbon == null)
                {
                    ribbon = VisualUtils.FindSomeParent(m_target as FrameworkElement, typeof(Ribbon)) as Ribbon;

                    if (ribbon == null)
                    {
                        Visual newLogicalParent = LogicalTreeHelper.GetParent(m_target) as Visual;

                        while (newLogicalParent != null && !(newLogicalParent is Ribbon))
                        {
                            logicalParent = newLogicalParent;
                            newLogicalParent = LogicalTreeHelper.GetParent(logicalParent) as Visual;
                        }

                        if (logicalParent != null)
                        {
                            ribbon = VisualUtils.FindAncestor(logicalParent, typeof(Ribbon)) as Ribbon;
                        }
                    }
                }
            }
            return ribbon;
        }


        /// <summary>
        /// Finds the visual root.
        /// </summary>
        /// <param name="m_target">The m_target.</param>
        /// <returns></returns>
        private static FrameworkElement FindVisualRoot(FrameworkElement m_target)
        {

            Ribbon ribbon = null;

            FrameworkElement visualRoot = VisualUtils.FindRootVisual(m_target as Visual) as FrameworkElement;

            Visual logicalParent = null;

            if (VisualUtils.RootPopupType == visualRoot.GetType())
            {
                visualRoot = visualRoot.Parent as FrameworkElement;
                while (visualRoot != null && !(visualRoot is Ribbon) && !(visualRoot is ApplicationMenu))
                {
                    if (visualRoot.Parent != null)
                    {
                        visualRoot = visualRoot.Parent as FrameworkElement;
                    }
                    else
                    {
                        visualRoot = visualRoot.TemplatedParent as FrameworkElement;
                    }
                }

                if (visualRoot != null)
                {
                    if (visualRoot is Ribbon)
                    {
                        ribbon = visualRoot as Ribbon;
                    }
                    else
                    {
                        ribbon = VisualUtils.FindAncestor(visualRoot as Visual, typeof(Ribbon)) as Ribbon;
                    }
                }
            }

            if (ribbon == null)
            {
                if (visualRoot == null)
                {
                    visualRoot = m_target as FrameworkElement;
                }

                if (m_target is Ribbon)
                {
                    ribbon = m_target as Ribbon;
                }
                else
                {
                    ribbon = VisualUtils.FindAncestor(m_target, typeof(Ribbon)) as Ribbon;
                }

                if (ribbon == null)
                {
                    ribbon = VisualUtils.FindSomeParent(m_target as FrameworkElement, typeof(Ribbon)) as Ribbon;

                    if (ribbon == null)
                    {
                        Visual newLogicalParent = LogicalTreeHelper.GetParent(m_target) as Visual;

                        while (newLogicalParent != null && !(newLogicalParent is Ribbon))
                        {
                            logicalParent = newLogicalParent;
                            newLogicalParent = LogicalTreeHelper.GetParent(logicalParent) as Visual;
                        }

                        if (logicalParent != null)
                        {
                            ribbon = VisualUtils.FindAncestor(logicalParent, typeof(Ribbon)) as Ribbon;
                        }
                    }
                }
            }
            return visualRoot;
        }

        /// <summary>
        /// Gets the real source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>real source</returns>
        internal static FrameworkElement GetRealSource(ItemsControl source)
        {
            Point point = Mouse.GetPosition(source);
            FrameworkElement realSource = ScreenUtils.GetElementFromPoint(source, source.PointToScreen(point));
            return realSource;
        }

        #endregion

        public void Dispose()
        {
            this.ribbon.ribbon_contextmenu = this;
            this.ribbon = null;
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(this, OnPreviewMouseDownOutsideCapturedElement);
            
            this.Unloaded -= new RoutedEventHandler(RibbonContextMenu_Unloaded);
        }
    }


    /// <summary>
    /// Collection class that holds custom context menu items.
    /// </summary>
    public class RibbonCustomContextMenuItems : ObservableCollection<UIElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonCustomContextMenuItems"/> class.
        /// </summary>
        public RibbonCustomContextMenuItems()
        { }
    }
}
