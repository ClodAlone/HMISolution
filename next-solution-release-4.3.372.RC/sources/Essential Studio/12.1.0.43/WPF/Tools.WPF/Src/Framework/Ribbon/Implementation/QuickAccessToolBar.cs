// <copyright file="QuickAccessToolBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a QuickAccessToolBar control.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class QuickAccessToolBar : ItemsControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:QuickAccessToolBar Name="toolbar" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// QuickAccessToolBar class represents a control that displays horizontal row of the most commonly used commands in your application, with overflow support, also it supports ContextMenu clicking on it's dropdown button.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a QuickAccessToolBar in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:QuickAccessToolBar>
    /// <ribbon:RibbonButton ribbon:Ribbon.KeyTip="1" Command="ApplicationCommands.Close"/>
    /// <ribbon:RibbonButton ribbon:Ribbon.KeyTip="2" Command="ApplicationCommands.Save">
    /// </ribbon:QuickAccessToolBar>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a QuickAccessToolBar in C#.
    /// <code>    
    /// RibbonButton button1;
    /// RibbonButton button2;
    /// QuickAccessToolBar toolbar = new QuickAccessToolBar();
    /// toolbar.Items.Add(button1);
    /// toolbar.Items.Add(button2);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class QuickAccessToolBar : ItemsControl
    {
        #region Private members

        /// <summary>
        /// Represents the item wrapping
        /// </summary>
        private bool m_needItemWrap;

        /// <summary>
        /// Internal storage which encapsulates logic of adding, removing,
        /// preserving from items duplicating.
        /// </summary>
        internal InternalCommandManager m_internalCommandManager;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Ribbon instance.
        /// </summary>
        private Ribbon m_ribbon;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines whether QATItemsChangeEvent can be raised or not.
        /// </summary>
        private bool m_qATItemsChangeEventLocked;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Main ItemsControl element from visual tree.
        /// </summary>
        internal ItemsControl m_mainItemsControl;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// QAT menu custom ItemsControl element from visual tree.
        /// </summary>
        internal ItemsControl m_qatMenuItemsControl;

        /// <summary>
        /// QAT menu custom ItemsControl element from visual tree.
        /// </summary>
        internal ItemsControl m_qatMenuItemsOverFlowControl;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Popup ItemsControl element from visual tree.
        /// </summary>
        private ItemsControl m_popupItemsControl;

        /// <summary>
        /// QAT Menu item collection..
        /// </summary>
        private ObservableCollection<RibbonButton> m_itemCollection;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Button which opens popup.
        /// </summary>
        internal DropDownButton m_popupButton;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Button which indicates that toolbar overflow its items. 
        /// </summary>
        internal DropDownButton m_popupButtonOverflowed;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Default items which are initialized through XAML.
        /// </summary>
        internal List<UIElement> m_defaultItems;

        /// <summary>
        /// Button which indicates the QAT menu overflow button.
        /// </summary>
        internal DropDownButton m_popup_OverflowButton;

        private DropDownButton Qatdropdownbutton;

        SystemGesture m_systemGesture;
        #endregion

        #region	Initialization

        /// <summary>
        /// Initializes static members of the <see cref="QuickAccessToolBar"/> class.
        /// </summary>
        static QuickAccessToolBar()
        {
           // EnvironmentTest.ValidateLicense(typeof(QuickAccessToolBar));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(typeof(QuickAccessToolBar)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickAccessToolBar"/> class.
        /// </summary>
        public QuickAccessToolBar()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(QuickAccessToolBar));
            }
            m_internalCommandManager = new InternalCommandManager();
            m_internalCommandManager.QuickAccessToolBar = this;
            m_needItemWrap = true;
            m_itemCollection = new ObservableCollection<RibbonButton>();
            m_itemCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(M_itemCollection_CollectionChanged);
            WindowChrome.SetIsHitTestVisibleInChrome(this, true);
            this.Loaded += new RoutedEventHandler(QuickAccessToolBar_Loaded);            
        }      

        /// <summary>
        /// Handles the Loaded event of the QuickAccessToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void QuickAccessToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_defaultItems == null)
            {
                m_defaultItems = new List<UIElement>();
                foreach (object item in Items)
                {
                    if (item is UIElement)
                    {
                        m_defaultItems.Add(item as UIElement);
                    }
                }

                if (this.Ribbon !=null && !(this.Ribbon.IsQATOnceLoaded && this.AutoPersist == true))
                    m_internalCommandManager.InitializeDefaultItems(m_defaultItems);
                if (this.AutoPersist == true)
                    m_internalCommandManager.m_defaultAutoPersistItems = m_defaultItems;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the m_itemCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void M_itemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListCollectionView popupView = new ListCollectionView(m_itemCollection);

            if (m_qatMenuItemsControl != null)
            {
                m_qatMenuItemsControl.ItemsSource = popupView;
            }

            if (m_qatMenuItemsOverFlowControl != null)
            {
                m_qatMenuItemsOverFlowControl.ItemsSource = popupView;
            }
            if (e.NewItems != null && e.NewItems.Count>0)
            {
                RibbonCommandManager.QATMenuItem = (FrameworkElement)e.NewItems[0];
                RibbonButton button = (RibbonButton)e.NewItems[0];
                button.IsMenuItem = true;
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Adds the event handler.
        /// </summary>
        private void AddEventHandler()
        {
            Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;
            foreach (RibbonButton obj in m_itemCollection)
            {
                obj.Click -= new RoutedEventHandler(QATMenuItem_Click);
                obj.Click += new RoutedEventHandler(QATMenuItem_Click);
                obj.IsMenuItem = true;
                obj.IsSelected = CheckQATMenuItems(obj, syncItemColl);              
            }
        }

        /// <summary>
        /// Checks the QAT menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="syncItemColl">The sync item coll.</param>
        private bool CheckQATMenuItems(RibbonButton obj, Dictionary<string, FrameworkElement> syncItemColl)
        {
            string itemName = RibbonCommandManager.GetSynchronizedItem(obj);

            if (itemName != null && syncItemColl.ContainsKey(itemName))
            {
                for (int index = 0; index < Items.Count; index++)
                {
                    FrameworkElement qatitem = (FrameworkElement)Items[index];
                    if (itemName.Equals(RibbonCommandManager.GetSynchronizedItem(qatitem)))
                    {
                        return true;                        
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Handles the Click event of the QATMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void QATMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RibbonButton obj = (RibbonButton)sender;

            if (obj.IsSelected)
            {
                obj.IsSelected = false;
                RemoveFromQAT(obj);                              
            }
            else
            {
                 obj.IsSelected = true;
                AddToQAT(obj);
            }
        }

        /// <summary>
        /// Removes from QAT.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void RemoveFromQAT(FrameworkElement obj)
        {
            string itemName = RibbonCommandManager.GetSynchronizedItem(obj);
            Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;

            if (itemName != null && syncItemColl.ContainsKey(itemName))
            {
                for (int index = 0;  index < Items.Count; index++)
                {
                    FrameworkElement qatitem = (FrameworkElement)Items[index];
                    if (itemName.Equals(RibbonCommandManager.GetSynchronizedItem(qatitem)))                    
                    {
                        if (qatitem is ICommandSource)
                            obj.Tag = (qatitem as ICommandSource).Command;

                        InternalCommandManager.Remove(qatitem);
                        Items.RemoveAt(index);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks the QAT items command.
        /// </summary>
        internal void CheckQATItemsCommand()
        {
            foreach (var obj in this.QATMenuItems)
            {
                string itemName = RibbonCommandManager.GetSynchronizedItem(obj);
                 Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;
                 if (itemName != null && syncItemColl.ContainsKey(itemName))
                 {
                     FrameworkElement qatitem = syncItemColl[itemName];

                     if (qatitem is ICommandSource)
                         obj.Tag = (qatitem as ICommandSource).Command;
                 }
            }
        }

        /// <summary>
        /// Adds to QAT.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void AddToQAT(FrameworkElement obj)
        {
            string itemName = RibbonCommandManager.GetSynchronizedItem(obj);
            Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;

            if (itemName != null && syncItemColl.ContainsKey(itemName))
            {
                FrameworkElement item = syncItemColl[itemName];               
                NeedItemWrap = false;

                if (item is ButtonBase && (item as ButtonBase).Command == null)
                    (item as ButtonBase).Command = obj.Tag as ICommand;
                else if (item is SplitButton && (item as SplitButton).Command ==  null )
                    (item as SplitButton).Command = obj.Tag as ICommand;
                else if (item is SplitMenuButton && (item as SplitMenuButton).Command == null)
                    (item as SplitMenuButton).Command = obj.Tag as ICommand;               

                QuickAccessToolBarItem qatitem = new QuickAccessToolBarItem(item);
                
                try
                {
                    if (InternalCommandManager.CanAdd(qatitem))
                    {
                        InternalCommandManager.Add(qatitem);
                        if(Ribbon != null && Ribbon.QATItems != null)
                            Ribbon.QATItems.Add(qatitem.ClonedElement, obj);
                        Items.Add(qatitem.ClonedElement);
                    }
                }
                catch
                {
                }

                NeedItemWrap = true;
            }
        }

        /// <summary>
        /// Checks the uncheck from QAT menu.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        private void CheckUncheckFromQATMenu(FrameworkElement obj, bool isChecked)
        {
            string itemName = RibbonCommandManager.GetSynchronizedItem(obj);

            if (itemName == null)
                return;
            if (m_qatMenuItemsControl != null)
            {
                for (int index = 0; index < m_qatMenuItemsControl.Items.Count; index++)
                {
                    RibbonButton qatMenuItm = (m_qatMenuItemsControl.Items[index] as RibbonButton);
                    string qatMenuItemName = RibbonCommandManager.GetSynchronizedItem(qatMenuItm);
                    if (qatMenuItemName != null && itemName.Equals(qatMenuItemName))
                    {
                        qatMenuItm.IsSelected = isChecked;
                        break;
                    }

                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            FrameworkElement realSource = RibbonContextMenu.GetRealSource(this);
            if (realSource != null)
            {
                if (!realSource.IsEnabled)
                {
                    ContextMenuService.SetShowOnDisabled(realSource, true);
                    RibbonContextMenu.CreateContextMenu(realSource);
                    e.Handled = true;
                    return;
                }
            }

            base.OnMouseRightButtonUp(e);
        }

          #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch && m_systemGesture==SystemGesture.RightTap)
            {
                FrameworkElement realSource = RibbonContextMenu.GetRealSource(this);
                if (realSource != null)
                {
                    if (!realSource.IsEnabled)
                    {
                        ContextMenuService.SetShowOnDisabled(realSource, true);
                        RibbonContextMenu.CreateContextMenu(realSource);
                        e.Handled = true;
                        return;
                    }
                }

                base.OnTouchUp(e);
            }
        }
        #endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_systemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           
            m_mainItemsControl = (ItemsControl)GetTemplateChild("PART_Main");
            m_mainItemsControl.ItemsSource = Items;
            m_popupItemsControl = (ItemsControl)GetTemplateChild("PART_PopupItemsControl");
            m_popupButton = (DropDownButton)GetTemplateChild("PART_PopupButton");
            m_qatMenuItemsControl = (ItemsControl)Template.FindName("PART_QATMenu", this);
            m_qatMenuItemsOverFlowControl = (ItemsControl)Template.FindName("PART_QATMenuOverFlow", this);
            m_popupButtonOverflowed = (DropDownButton)GetTemplateChild("PART_PopupButtonOverflowed");
            m_popup_OverflowButton = (DropDownButton)GetTemplateChild("PART_OverflowButton");

            RibbonButton b = GetTemplateChild("PART_BelowButton") as RibbonButton;
            b.CommandTarget = Ribbon;
            b = GetTemplateChild("PART_AboveButton") as RibbonButton;
            b.CommandTarget = Ribbon;
            b = GetTemplateChild("PART_MinimizeButton") as RibbonButton;
            b.CommandTarget = Ribbon;
            b = GetTemplateChild("PART_MoreCommands") as RibbonButton;
            b.CommandTarget = Ribbon;

            if (VisualInitializeCompleete != null)
            {
                VisualInitializeCompleete(this, new EventArgs());
            }

            if (m_qatMenuItemsControl != null && m_qatMenuItemsOverFlowControl != null)
            {
                UpdateQATMenuItems();

            }
            checkVisibleDrodownButton();
           
            if(IsLoaded &&m_mainItemsControl!=null)
               m_mainItemsControl.Loaded += new RoutedEventHandler(m_mainItemsControl_Loaded);
        }

        void m_mainItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            m_mainItemsControl.Loaded -= new RoutedEventHandler(m_mainItemsControl_Loaded);
            QuickAccessToolBarPanel Qatpanel = VisualUtils.FindDescendant(m_mainItemsControl, typeof(QuickAccessToolBarPanel)) as QuickAccessToolBarPanel;
            if (Qatpanel != null)
                Qatpanel.InvalidateMeasure();
        }

        /// <summary>
        /// Updates the QAT menu items.
        /// </summary>
        private void UpdateQATMenuItems()
        {
            m_qatMenuItemsControl.ItemsSource = m_itemCollection;
            m_qatMenuItemsOverFlowControl.ItemsSource = m_itemCollection;
            AddEventHandler();            
        }

        /// <summary>
        /// Updates the state of the QAT items.
        /// </summary>
        public void UpdateQATItemsState()
        {
            if(m_qatMenuItemsControl!=null)
                m_qatMenuItemsControl.ItemsSource = m_itemCollection;
            if(m_qatMenuItemsOverFlowControl!=null)
                m_qatMenuItemsOverFlowControl.ItemsSource = m_itemCollection;

            ObservableCollection<RibbonButton> buttons = new ObservableCollection<RibbonButton>();

            foreach (RibbonButton obj in m_itemCollection)
            {
                obj.Click -= new RoutedEventHandler(QATMenuItem_Click);
                obj.Click += new RoutedEventHandler(QATMenuItem_Click);
                obj.IsMenuItem = true;

                if (!buttons.Contains(obj))
                    buttons.Add(obj);
               
            }

            foreach (RibbonButton obj in buttons)
            {
                if (obj.IsSelected)
                    AddToQAT(obj);
                else
                    RemoveFromQAT(obj);
            }

            UpdateItemsLayout();
        }

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="childName">Name of the child.</param>
        /// <returns>Template child</returns>
        protected internal DependencyObject GetChild(string childName)
        {
            return GetTemplateChild(childName);
        }

        /// <summary>
        /// Calls OnHasOverflowChildrenChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnHasOverflowChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBar instance = (QuickAccessToolBar)d;
            instance.OnHasOverflowChildrenChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// HasOverflowChildrenChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHasOverflowChildrenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasOverflowChildrenChanged != null)
            {
                HasOverflowChildrenChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHasGeometryChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnHasGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBar instance = (QuickAccessToolBar)d;
            instance.OnHasGeometryChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasGeometryChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHasGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasGeometryChanged != null)
            {
                HasGeometryChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGeometryBackgroundChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGeometryBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBar instance = (QuickAccessToolBar)d;
            instance.OnGeometryBackgroundChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// GeometryBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnGeometryBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GeometryBackgroundChanged != null)
            {
                GeometryBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGeometryStrokeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGeometryStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBar instance = (QuickAccessToolBar)d;
            instance.OnGeometryStrokeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// GeometryStrokeChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnGeometryStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GeometryStrokeChanged != null)
            {
                GeometryStrokeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnVisibleItemsCountChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnVisibleItemsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBar instance = (QuickAccessToolBar)d;
            instance.OnVisibleItemsCountChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// VisibleItemsCountChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnVisibleItemsCountChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateItemsLayout();

            if (VisibleItemsCountChanged != null)
            {
                VisibleItemsCountChanged(this, e);
            }
        }

        /// <summary>
        /// CollectionView filter. Filters items by index.
        /// </summary>
        /// <param name="item">Object from Items collection.</param>
        /// <returns>
        /// True, if index of item is less than VisibleItemsCount - 1;
        /// otherwise, false.
        /// </returns>
        protected bool FilterItems(object item)
        {
            return Items.IndexOf(item) <= VisibleItemsCount - 1;
        }

        /// <summary>
        /// CollectionView filter. Filters items by index.
        /// </summary>
        /// <param name="item">Object from Items collection.</param>
        /// <returns>
        /// FilterItems inversion.
        /// </returns>
        protected bool FilterItemsInvert(object item)
        {
            return Items.IndexOf(item) > VisibleItemsCount - 1;
        }

        /// <summary>
        /// Handles changes in Items property.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Ribbon != null && !m_qATItemsChangeEventLocked)
            {
                ArrayList oldItems = new ArrayList(Items);
                object orginalitem = null;
                QATAction action;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (m_needItemWrap)
                        {
                            QuickAccessToolBarItem item = new QuickAccessToolBarItem(e.NewItems[0] as UIElement);
                            InternalCommandManager.Add(item);
                        }

                        oldItems.Remove(e.NewItems[0]);
                        CheckUncheckFromQATMenu(e.NewItems[0] as FrameworkElement, true);

                        if (Ribbon.QATItems !=null && Ribbon.QATItems.Count > 0)
                        {
                            if (Ribbon.QATItems.ContainsKey(e.NewItems[0] as UIElement))
                                orginalitem = Ribbon.QATItems[e.NewItems[0] as UIElement];
                        }
                        this.OnQATItemAdded(e.NewItems[0] as UIElement,orginalitem,e.NewItems[0] as UIElement);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        Debug.WriteLine("Not supported action");
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (m_needItemWrap)
                        {
                            InternalCommandManager.Remove(e.OldItems[0] as UIElement);
                        }
                        Stack<string> stack = AutomaticKeys.m_keys;
                        AutomaticKeys.Pop();
                        int count = Items.Count;
                        foreach (string obj in stack)
                        {
                            if (count > 0)
                            {
                                Controls.Ribbon.SetKeyTip((Items[--count] as FrameworkElement), obj);
                            }
                        }

                        oldItems.Add(e.OldItems[0]);
                        //RibbonCommandManager.RemoveFromSyncCollection(RibbonCommandManager.GetSynchronizedItem(e.OldItems[0] as FrameworkElement));
                        CheckUncheckFromQATMenu(e.OldItems[0] as FrameworkElement, false);
                        this.OnQATItemRemoved(e.OldItems[0] as UIElement, e.OldItems[0] as UIElement, e.OldItems[0] as UIElement);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        Debug.WriteLine("Not supported action");
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        break;
                }
                IList newitems;
                if (e.Action == NotifyCollectionChangedAction.Add || e.Action==NotifyCollectionChangedAction.Remove)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        newitems = Items;
                        action = QATAction.Add;
                    }
                    else
                    {
                        newitems = Items;
                        action = QATAction.Remove;
                    }

                    Ribbon.FireQATItemsCollectionChanged(new QATItemsCollectionChangedEventArgs(oldItems, newitems,QATItemsContainer.QAT, action));
                }
            }

            if (m_qATItemsChangeEventLocked)
            {
                Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;
                foreach (RibbonButton obj in m_itemCollection)
                {                    
                    obj.IsSelected = CheckQATMenuItems(obj, syncItemColl);
                }
            }

            if (m_mainItemsControl != null && m_popupItemsControl != null)
            {
                UpdateItemsLayout();
            }

            base.OnItemsChanged(e);
        }

        internal void RemoveQATItemFireQATRemoved(object remove)
        {
            object org = null, clone = null;
            QuickAccessToolBarItem toolbaritem = InternalCommandManager.GetItemByClone(remove as UIElement);
            org = toolbaritem.SourceElement;
            clone = toolbaritem.ClonedElement;
            InternalCommandManager.Remove(remove as UIElement);
            Items.Remove(remove);
           //this.OnQATItemRemoved(remove as UIElement, org, clone);
        }

        /// <summary>
        /// Updates items layout.
        /// </summary>
        private void UpdateItemsLayout()
        {
            if (VisibleItemsCount != Items.Count)
            {
                HasOverflowChildren = true;
                if (m_popupButton != null)
                    m_popupButton.Visibility = System.Windows.Visibility.Collapsed;            
            }
            else
            {
                HasOverflowChildren = false;
                if (m_popupButton != null)
                    m_popupButton.Visibility = QATDropDownVisiblity;            
            }

            if (this.Items.Count > 0)
            {
                ListCollectionView mainView;

                if (this.ItemsSource == null && m_mainItemsControl!=null)
                {
                    mainView = new ListCollectionView(Items);
                    mainView.Filter = new Predicate<object>(FilterItems);
                    m_mainItemsControl.ItemsSource = mainView;
                }
                ListCollectionView popupView;
                if (this.ItemsSource == null && m_popupItemsControl!=null)
                {
                    popupView = new ListCollectionView(Items);
                    popupView.Filter = new Predicate<object>(FilterItemsInvert);
                    m_popupItemsControl.ItemsSource = popupView;
                }
            }
        }

        /// <summary>
        /// Locks the QAT items change. 
        /// </summary>
        protected internal void LockQATItemsChange()
        {
            m_qATItemsChangeEventLocked = true;
        }

        /// <summary>
        /// Unlocks the QAT items change. 
        /// </summary>
        protected internal void UnlockQATItemsChange()
        {
            m_qATItemsChangeEventLocked = false;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContentPresenter();
        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameworkElement;
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {            
            base.PrepareContainerForItemOverride(element, item);
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            m_mainItemsControl.ItemsSource = newValue;
            m_mainItemsControl.ItemTemplate = this.ItemTemplate;            
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.Ribbon != null && Ribbon.RibbonState == RibbonState.Adorner)
            {

                 Qatdropdownbutton= VisualUtils.FindAncestor(e.OriginalSource as FrameworkElement, typeof(DropDownButton)) as DropDownButton;
                 if (Qatdropdownbutton != null)
                 {
                     Ribbon.m_Qatdropdownclicked = true;
                     Qatdropdownbutton.MenuPopUp.Closed += MenuPopUp_Closed;
                 }

                Ribbon.HideAdorned();
                if (Ribbon.SelectedItem is RibbonTab && (Ribbon.SelectedItem as RibbonTab).m_tabButton != null)
                    (Ribbon.SelectedItem as RibbonTab).m_tabButton.IsChecked = false;
                Popup AdornerPopup = Ribbon.Template.FindName("Part_AdornerPopup", Ribbon) as Popup;
                if (AdornerPopup != null && AdornerPopup.IsOpen)
                    AdornerPopup.IsOpen = false;                
                base.OnPreviewMouseLeftButtonDown(e);

            }
            else
                base.OnPreviewMouseLeftButtonDown(e);
        }

         #if !SyncfusionFramework3_5
        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {           
            if (this.Ribbon != null && this.Ribbon.EnableTouch && m_systemGesture == SystemGesture.Tap)
            {
                if (Ribbon.RibbonState == RibbonState.Adorner)
                {

                    Qatdropdownbutton = VisualUtils.FindAncestor(e.OriginalSource as FrameworkElement, typeof(DropDownButton)) as DropDownButton;
                    if (Qatdropdownbutton != null)
                    {
                        Ribbon.m_Qatdropdownclicked = true;
                        Qatdropdownbutton.MenuPopUp.Closed += MenuPopUp_Closed;
                    }

                    Ribbon.HideAdorned();
                    if (Ribbon.SelectedItem is RibbonTab && (Ribbon.SelectedItem as RibbonTab).m_tabButton != null)
                        (Ribbon.SelectedItem as RibbonTab).m_tabButton.IsChecked = false;
                    Popup AdornerPopup = Ribbon.Template.FindName("Part_AdornerPopup", Ribbon) as Popup;
                    if (AdornerPopup != null && AdornerPopup.IsOpen)
                        AdornerPopup.IsOpen = false;                  

                }
                base.OnPreviewTouchDown(e);
            }
        }
        #endif

        void MenuPopUp_Closed(object sender, EventArgs e)
        {
            Popup menupopup = sender as Popup;

            if (Ribbon != null && menupopup != null && Qatdropdownbutton!=null && Qatdropdownbutton.MenuPopUp!=null)
            {
                Qatdropdownbutton.MenuPopUp.Closed -= MenuPopUp_Closed;     
                if (Ribbon.m_Qatdropdownclicked)
                    Ribbon.m_Qatdropdownclicked = false;
                if (Ribbon.RibbonState == RibbonState.Adorner)
                    Ribbon.RibbonState = RibbonState.Hide;
            }
        }
        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets a value indicating whether [save original state].
        /// </summary>
        /// <value><c>true</c> if [save original state]; otherwise, <c>false</c>.</value>
        [Description("Indicates whether to save state persisted on loading.")]
        public bool AutoPersist
        {
            get { return (bool)GetValue(AutoPersistProperty); }
            set { SetValue(AutoPersistProperty, value); }
        }   

        /// <summary>
        /// Gets or sets a value indicating whether [need item wrap].
        /// </summary>
        /// <value><c>true</c> if [need item wrap]; otherwise, <c>false</c>.</value>
        internal bool NeedItemWrap
        {
            get
            {
                return m_needItemWrap;
            }

            set
            {
                m_needItemWrap = value;
            }
        }

        /// <summary>
        /// Gets the internal command manager.
        /// </summary>
        /// <value>The internal command manager.</value>
        internal InternalCommandManager InternalCommandManager
        {
            get
            {
                return m_internalCommandManager;
            }
        }

        /// <summary>
        /// Gets or sets the ribbon.
        /// </summary>
        /// <value>The ribbon.</value>
        protected internal Ribbon Ribbon
        {
            get
            {
                return m_ribbon;
            }

            set
            {
                m_ribbon = value;
            }
        }

        /// <summary>
        /// Gets the default items.
        /// </summary>
        /// <value>The default items.</value>
        protected internal List<UIElement> DefaultItems
        {
            get
            {
                return m_defaultItems;
            }
        }

        /// <summary>
        /// Gets or sets the popup button.
        /// </summary>
        /// <value>The popup button.</value>
        internal DropDownButton PopupButton
        {
            get
            {
                return m_popupButton;
            }

            set
            {
                m_popupButton = value;
            }
        }

        /// <summary>
        /// Gets the QAT menu items.
        /// </summary>
        /// <value>The QAT menu items.</value>
        public ObservableCollection<RibbonButton> QATMenuItems
        {
            get
            {
                return m_itemCollection;
            }
            set
            {
                this.m_itemCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the popup button overflow.
        /// </summary>
        /// <value>The popup button overflow.</value>
        internal DropDownButton PopupButtonOverflow
        {
            get
            {
                return m_popupButtonOverflowed;
            }

            set
            {
                m_popupButtonOverflowed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has overflow children.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has overflow children; otherwise, <c>false</c>.
        /// </value>
        public bool HasOverflowChildren
        {
            get
            {
                return (bool)GetValue(HasOverflowChildrenProperty);
            }

            set
            {
                SetValue(HasOverflowChildrenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has geometry.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has geometry; otherwise, <c>false</c>.
        /// </value>
        public bool HasGeometry
        {
            get
            {
                return (bool)GetValue(HasGeometryProperty);
            }

            set
            {
                SetValue(HasGeometryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets background color or gradient for geometry.
        /// </summary>
        public Brush GeometryBackground
        {
            get
            {
                return (Brush)GetValue(GeometryBackgroundProperty);
            }

            set
            {
                SetValue(GeometryBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets  Stroke color or gradient for geometry.
        /// </summary>
        public Brush GeometryStroke
        {
            get
            {
                return (Brush)GetValue(GeometryStrokeProperty);
            }

            set
            {
                SetValue(GeometryStrokeProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the Geometry stroke thickness
        /// </summary>
        public double GeometryStrokeThickness
        {
            get 
            {
                return (double)GetValue(GeometryStrokeThicknessProperty); 
            }
            set
            { 
                SetValue(GeometryStrokeThicknessProperty, value); 
            }
        }
        

        /// <summary>
        /// Gets or sets the visible items count.
        /// </summary>
        /// <value>The visible items count.</value>
        protected internal int VisibleItemsCount
        {
            get
            {
                return (int)GetValue(VisibleItemsCountProperty);
            }

            set
            {
                SetValue(VisibleItemsCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the QAT drop down visiblity.
        /// </summary>
        /// <value>The QAT drop down visiblity.</value>
        public Visibility QATDropDownVisiblity
        {
            get
            {
                return (Visibility)GetValue(QATDropDownVisiblityProperty);
            }
            set
            {
                SetValue(QATDropDownVisiblityProperty, value);
            }
        }

        /// <summary>
        /// Determines whether [is QAT drop down visible changed] [the specified d].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsQATDropDownVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessToolBar instance = (QuickAccessToolBar)d;

            instance.IsQATDropDownVisibleChanged(e);
            
        }

        /// <summary>
        /// Determines whether [is QAT drop down visible changed] [the specified e].
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void IsQATDropDownVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_popupButton != null )
            {
                m_popupButton.Visibility = (Visibility)e.NewValue==Visibility.Visible && HasOverflowChildren ?
                    m_popupButton.Visibility :(Visibility)e.NewValue;
            }

            if (PopupButtonOverflow != null)
                PopupButtonOverflow.Visibility = (Visibility)e.NewValue;
        }

        /// <summary>
        /// Checks the visible drodown button.
        /// </summary>
        protected internal void checkVisibleDrodownButton()
        {
            if (m_popupButton != null )
            {
                m_popupButton.Visibility = QATDropDownVisiblity;

            }
        }

        /// <summary>
        /// Called when [QAT item removed].
        /// </summary>
        /// <param name="element">The element.</param>
        internal virtual void OnQATItemRemoved(UIElement element,object originalitem,object cloneditem)
        {
            UIElement sourceElement = null;
            sourceElement = element;

            if (this.QATItemRemoved != null)
                QATItemRemoved(this, new QATItemEventArgs(sourceElement,originalitem,cloneditem));
        }

        /// <summary>
        /// Called when [QAT item added].
        /// </summary>
        /// <param name="element">The element.</param>
        internal virtual void OnQATItemAdded(UIElement element,object originalitem,object cloneditem)
        {
            UIElement sourceElement = null;
            sourceElement = element;

            if (this.QATItemAdded != null)
                QATItemAdded(this, new QATItemEventArgs(sourceElement,originalitem,cloneditem));
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [QAT item removed].
        /// </summary>
        public event EventHandler QATItemRemoved;

        /// <summary>
        /// Occurs when [QAT item added].
        /// </summary>
        public event EventHandler QATItemAdded;

        /// <summary>
        /// Event that is raised when HasOverflowChildren property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback HasOverflowChildrenChanged;

        /// <summary>
        /// Event that is raised when HasGeometry property is changed.
        /// </summary>
        public event PropertyChangedCallback HasGeometryChanged;

        /// <summary>
        /// Event that is raised when GeometryBackground property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback GeometryBackgroundChanged;

        /// <summary>
        /// Event that is raised when GeometryStroke property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback GeometryStrokeChanged;

        /// <summary>
        /// Event that is raised when VisibleItemsCount property is
        /// changed.
        /// </summary>
        protected internal event PropertyChangedCallback VisibleItemsCountChanged;

        /// <summary>
        /// Raises when QAT visual initialization is completed.
        /// </summary>
        protected internal event EventHandler VisualInitializeCompleete;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty QATDropDownVisiblityProperty =
       DependencyProperty.Register("QATDropDownVisiblity", typeof(Visibility), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(IsQATDropDownVisibleChanged)));

        /// <summary>
        /// Identifies the <see cref="AutoPersist"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoPersistProperty =
            DependencyProperty.Register("AutoPersist", typeof(bool), typeof(QuickAccessToolBar), new UIPropertyMetadata(false));

        /// <summary>
        /// Defines when QAT has children in popup. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HasOverflowChildrenProperty =
            DependencyProperty.Register("HasOverflowChildren", typeof(bool), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasOverflowChildrenChanged)));

        /// <summary>
        /// Defines when QAT should be rendered with geometry. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HasGeometryProperty =
            DependencyProperty.Register("HasGeometry", typeof(bool), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHasGeometryChanged)));

        /// <summary>
        /// Defines background color or gradient for geometry. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryBackgroundProperty =
            DependencyProperty.Register("GeometryBackground", typeof(Brush), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGeometryBackgroundChanged)));

        /// <summary>
        /// Defines Stroke color or gradient for geometry. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryStrokeProperty =
            DependencyProperty.Register("GeometryStroke", typeof(Brush), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGeometryStrokeChanged)));

        /// <summary>
        /// Defines children count that are not contained in popup. This is a dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty VisibleItemsCountProperty =
            DependencyProperty.Register("VisibleItemsCount", typeof(int), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnVisibleItemsCountChanged)));

        /// <summary>
        /// Defines the Geometry stroke thickness
        /// </summary>
        public static readonly DependencyProperty GeometryStrokeThicknessProperty =
            DependencyProperty.Register("GeometryStrokeThickness", typeof(double), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion          
        
    }

    public class QATItemEventArgs : EventArgs
    {
        private object original=null , cloned=null;

        /// <summary>
        /// Represent the source element of QAT item.
        /// </summary>
        public UIElement SourceElement;

        /// <summary>
        /// 
        /// </summary>
        public object OriginalItem
        {
            get
            {
                return original;
            }
            set
            {
                original=value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object ClonedItem
        {
            get
            {
                return cloned;
            }
            set
            {
                cloned=value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QATItemEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public QATItemEventArgs(UIElement item,object _original,object _cloned)
        {
            this.SourceElement = item;
            OriginalItem=_original;
            ClonedItem=_cloned;
        }
    }
}