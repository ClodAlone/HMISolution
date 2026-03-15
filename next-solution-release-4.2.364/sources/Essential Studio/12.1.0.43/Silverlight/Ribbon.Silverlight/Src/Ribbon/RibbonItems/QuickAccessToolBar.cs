#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/RibbonControls/RibbonQuickAccessToolBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Office2007Black;component/RibbonControls/RibbonQuickAccessToolBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/RibbonControls/RibbonQuickAccessToolBar.xaml")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
       Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/RibbonControls/RibbonQuickAccessToolBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Office2010Black;component/RibbonControls/RibbonQuickAccessToolBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/RibbonControls/RibbonQuickAccessToolBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Blend;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
     Type = typeof(QuickAccessToolBar), XamlResource = "/Syncfusion.Theming.Metro;component/Ribbon.xaml")]
    public class QuickAccessToolBar : ItemsControl,IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickAccessToolBar"/> class.
        /// </summary>
        public QuickAccessToolBar()
        {
            this.DefaultStyleKey = typeof(QuickAccessToolBar);
            this.Unloaded += new RoutedEventHandler(QuickAccessToolBar_Unloaded);            
            qATMenuItems = new ObservableCollection<RibbonButton>();
            qATMenuItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(qATMenuItems_CollectionChanged);
        }

        void QuickAccessToolBar_Unloaded(object sender, RoutedEventArgs e)
        {
           
        }

        /// <summary>
        /// Handles the CollectionChanged event of the qATMenuItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void qATMenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_parentRibbon == null) _parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;

            if (_parentRibbon != null)
            {
                RibbonButton item = e.NewItems[0] as RibbonButton;
                if (item != null)
                {
                    AddRibbonButtonInSyncItem(item);
                }
            }
        }

        internal static int DefaultQATItemsCount = 10;
        internal static QATState CurrentQATState = QATState.AboveRibbon;

        internal Grid PART_OverFlowToggleButton;
        internal ToggleButton PART_ToggleButton;
        internal RibbonDropDown PART_RibbonDropDown;
        internal RibbonDropDown PART_OverflowRibbonDropDown;
        internal Grid PART_OverFlowTopContainer;
        internal Grid PART_OverFlowContainer;
        internal Grid PART_NormalContainer;
        internal StackPanel PART_OverflowItemsControl;
        internal QATDropDownControl PART_QATDropDownControl;
        internal Ribbon _parentRibbon;
        private bool isLoaded = false;
        internal ContextMenuAdv QATContextMenu;

        ObservableCollection<RibbonButton> qATMenuItems;
        /// <summary>
        /// Gets the QAT menu items.
        /// </summary>
        /// <value>The QAT menu items.</value>
        public ObservableCollection<RibbonButton> QATMenuItems
        {
            get
            {
                return qATMenuItems;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {

            if (this.PART_QATDropDownControl != null)
            {
                this.PART_QATDropDownControl.OnMinimizeRibbonClick -= new EventHandler(PART_QATDropDownControl_OnMinimizeRibbonClick);
                this.PART_QATDropDownControl.OnMoreCommandsClick -= new EventHandler(PART_QATDropDownControl_OnMoreCommandsClick);
                this.PART_QATDropDownControl.OnShowBelowtheRibbonClick -= new EventHandler(PART_QATDropDownControl_OnShowBelowtheRibbonClick);
            }

            this.PART_QATDropDownControl = this.GetTemplateChild("PART_QATDropDownControl") as QATDropDownControl;

            this.QATContextMenu = this.GetTemplateChild("PART_QATContextMenu") as ContextMenuAdv;

            this.PART_RibbonDropDown = this.GetTemplateChild("PART_RibbonDropDown") as RibbonDropDown;

            this.PART_OverflowRibbonDropDown = this.GetTemplateChild("PART_OverflowRibbonDropDown") as RibbonDropDown;

            this.PART_ToggleButton = this.GetTemplateChild("PART_ToggleButton") as ToggleButton;

            this.PART_OverFlowToggleButton = this.GetTemplateChild("PART_OverFlowToggleButton") as Grid;

            this.PART_OverFlowTopContainer = this.GetTemplateChild("PART_OverFlowTopContainer") as Grid;

            this.PART_OverFlowContainer = this.GetTemplateChild("PART_OverFlowContainer") as Grid;

            this.PART_NormalContainer = this.GetTemplateChild("PART_NormalContainer") as Grid;

            this.PART_OverflowItemsControl = this.GetTemplateChild("PART_OverflowItemsControl") as StackPanel;

            this.SubscribeClickEvents();
            if (this.OverFlowItems != null)
            {
                OverFlowItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OverFlowItems_CollectionChanged);
            }
            if (this.PART_OverflowRibbonDropDown != null)
            {
                PART_OverflowRibbonDropDown.IsOpenChanged += new EventHandler(PART_OverflowRibbonDropDown_IsOpenChanged);
            }
            this.Loaded += new RoutedEventHandler(QuickAccessToolBar_Loaded);
            isLoaded = false;
            this.UpdateQATMenuItems();
            this.ShowHidePathGeometry();
        }

        /// <summary>
        /// Updates the QAT menu items.
        /// </summary>
        private void UpdateQATMenuItems()
        {
            if (_parentRibbon == null) _parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;

            if (_parentRibbon != null)
            {
                foreach (RibbonButton item in this.QATMenuItems)
                {
                    if (item != null)
                    {
                        AddRibbonButtonInSyncItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the ribbon button in sync item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void AddRibbonButtonInSyncItem(RibbonButton item)
        {
            if (item == null) return;
            if (this.CheckforExistence(item)) return;
            var cmdpvdr = new RibbonCommandProvider(item.Label);
            if (item.SmallIcon != null)
                cmdpvdr.SmallIcon = item.SmallIcon;
            else if (item.LargeIcon != null)
                cmdpvdr.SmallIcon = item.LargeIcon;
            cmdpvdr.GroupName = "Synchronized Items";
            //cmdpvdr.Host = item;
            cmdpvdr.Command = item.Command;
            cmdpvdr.CommandParameter = item.CommandParameter;
            _parentRibbon.SynchronizedCommands.Add(cmdpvdr);
        }

        /// <summary>
        /// Handles the IsOpenChanged event of the PART_OverflowRibbonDropDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PART_OverflowRibbonDropDown_IsOpenChanged(object sender, EventArgs e)
        {
            if (PART_OverflowRibbonDropDown != null)
            {
                if (PART_OverflowRibbonDropDown.IsOpen)
                {
                    VisualStateManager.GoToState(this, "Pressed", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
        }

        /// <summary>
        /// Handles the Loaded event of the QuickAccessToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void QuickAccessToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                InitializeSynchronizedItems();
                isLoaded = true;
            }          
            this.Unloaded +=new RoutedEventHandler(QuickAccessToolBar_Unloaded);
        }

        /// <summary>
        /// Initializes the synchronized items.
        /// </summary>
        internal void InitializeSynchronizedItems()
        {
            _parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (_parentRibbon != null && PART_QATDropDownControl != null && PART_QATDropDownControl._dropdown != null)
            {
                int index = 0;
                RibbonMenuGroup _container = PART_QATDropDownControl._dropdown.Content as RibbonMenuGroup;
                foreach (var item in _parentRibbon.SynchronizedCommands)
                {
                    RibbonCommandProvider _command = item as RibbonCommandProvider;
                    RibbonMenuItem _item = new RibbonMenuItem() { Header = _command.Label, _provider = _command };
                    _item.IsCheckable = true;
                    _item.IsCheckedChanged += new PropertyChangedCallback(_item_IsCheckedChanged);
                    _item.IsChecked = _command.IsSynchronizedwithQAT;
                    if (_container != null)
                    {
                        _container.Items.Insert(index, _item);
                    }
                    index++;
                }
                if (_parentRibbon.SynchronizedCommands.Count() != 0)
                {
                    _container.Items.Insert(_parentRibbon.SynchronizedCommands.Count(), new RibbonSeparator() { Margin = new Thickness(24, 0, 5, 0) });
                }
            }

        }


        /// <summary>
        /// Synchronizes the QAT items.
        /// </summary>
        public void SynchronizeQATItems()
        {
            RibbonMenuGroup _container = PART_QATDropDownControl._dropdown.Content as RibbonMenuGroup;
            foreach (var menuItem in _container.Items)
            {
                if (menuItem is RibbonMenuItem && ((RibbonMenuItem)menuItem)._provider != null)
                    ((RibbonMenuItem)menuItem).IsChecked = ((RibbonMenuItem)menuItem)._provider.IsSynchronizedwithQAT;
            }
        }

        /// <summary>
        /// _item_s the is checked changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void _item_IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonMenuItem _sender = d as RibbonMenuItem;
            RibbonCommandProvider _command = _sender._provider as RibbonCommandProvider;
            if (_command != null)
            {
                RibbonButton _qatItem = new RibbonButton() { Label = _command.Label, SizeMode = SizeMode.Small, SmallIcon = _command.SmallIcon, Command = _command.Command, CommandParameter = _command.CommandParameter, _provider = _command };
                if ((bool)e.NewValue)
                {
                    try
                    {
                        if (!CheckforExistence(_qatItem))
                            Items.Add(_qatItem);
                        _qatItem._provider.IsSynchronizedwithQAT = true;
                    }
                    catch { }
                }
                else
                {
                    bool isRemoved = false;

                    foreach (var item in Items)
                    {
                        if (item is RibbonButton)
                        {
                            if (((RibbonButton)item)._provider != null)
                            {
                                if (((RibbonButton)item)._provider.Command == _command.Command && ((RibbonButton)item)._provider.CommandParameter == _command.CommandParameter)
                                {
                                    Items.Remove(item);
                                    ((RibbonButton)item)._provider.IsSynchronizedwithQAT = false;
                                    isRemoved = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!isRemoved)
                    {
                        var query = from UIElement element in this.OverFlowItems where element is RibbonButton && ((RibbonButton)element)._provider != null && ((RibbonButton)element)._provider.Command == _command.Command && ((RibbonButton)element)._provider.CommandParameter == _command.CommandParameter select element as UIElement;
                        if (query.Count() > 0)
                        {
                            UIElement pickedElement = (UIElement)query.FirstOrDefault();
                            this.OverFlowItems.Remove(pickedElement);
                            this.PART_OverflowItemsControl.Children.Remove(pickedElement);
                            ((RibbonButton)pickedElement)._provider.IsSynchronizedwithQAT = false;
                        }
                    }
                }
            }
            PART_QATDropDownControl._dropdown.IsOpen = false;
        }

        /// <summary>
        /// Checkfors the existence.
        /// </summary>
        /// <param name="_item">The _item.</param>
        /// <returns></returns>
        private bool CheckforExistence(RibbonButton _item)
        {
            if (_item.Command == null) return true;

            var query = from item in Items
                        where item is RibbonButton &&
                        (((RibbonButton)item).Command == _item.Command &&
                        ((RibbonButton)item).CommandParameter == null || ((RibbonButton)item).Command == _item.Command && ((RibbonButton)item).CommandParameter == _item.CommandParameter)
                        select item;

            if (query.Count() == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Updates the synchronization.
        /// </summary>
        internal void UpdateSynchronization()
        {
            RibbonMenuGroup _container = PART_QATDropDownControl._dropdown.Content as RibbonMenuGroup;

            var query = from FrameworkElement item in _container.Items
                        where item.Tag != null && item.Tag is RibbonCommandProvider && !((RibbonCommandProvider)item.Tag).IsSynchronizedwithQAT
                        select item;

            foreach (var item in query)
            {
                RibbonMenuItem menuItem = item as RibbonMenuItem;
                if (menuItem != null && menuItem.IsChecked == true)
                {
                    menuItem.IsChecked = false;
                }
            }

            var query1 = from FrameworkElement item in _container.Items
                         where item.Tag != null && item.Tag is RibbonCommandProvider && ((RibbonCommandProvider)item.Tag).IsSynchronizedwithQAT
                         select item;

            foreach (var item in query1)
            {
                RibbonMenuItem menuItem = item as RibbonMenuItem;
                if (menuItem != null && menuItem.IsChecked == false)
                {
                    menuItem.IsChecked = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the OverFlowItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void OverFlowItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.ClearOverFlowItems();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Subscribes the click events.
        /// </summary>
        private void SubscribeClickEvents()
        {
            if (this.PART_QATDropDownControl != null)
            {
                this.PART_QATDropDownControl.OnMinimizeRibbonClick += new EventHandler(PART_QATDropDownControl_OnMinimizeRibbonClick);
                this.PART_QATDropDownControl.OnMoreCommandsClick += new EventHandler(PART_QATDropDownControl_OnMoreCommandsClick);
                this.PART_QATDropDownControl.OnShowBelowtheRibbonClick += new EventHandler(PART_QATDropDownControl_OnShowBelowtheRibbonClick);
            }
            if (this.PART_OverFlowToggleButton != null)
            {
                this.PART_OverFlowToggleButton.MouseLeftButtonDown += new MouseButtonEventHandler(PART_OverFlowToggleButton_MouseLeftButtonDown);
                this.PART_OverFlowToggleButton.MouseEnter += new MouseEventHandler(PART_OverFlowToggleButton_MouseEnter);
                this.PART_OverFlowToggleButton.MouseLeave += new MouseEventHandler(PART_OverFlowToggleButton_MouseLeave);
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the PART_OverFlowToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PART_OverFlowToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (PART_OverflowRibbonDropDown != null && !this.PART_OverflowRibbonDropDown.IsOpen)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        /// <summary>
        /// Handles the MouseEnter event of the PART_OverFlowToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PART_OverFlowToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.PART_OverflowRibbonDropDown != null && !this.PART_OverflowRibbonDropDown.IsOpen)
                VisualStateManager.GoToState(this, "MouseOver", true);
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the PART_OverFlowToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PART_OverFlowToggleButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PART_OverflowRibbonDropDown != null)
            {
                PART_OverflowRibbonDropDown.IsOpen = (PART_OverflowRibbonDropDown.IsOpen == true) ? false : true;
            }
        }

        /// <summary>
        /// Gets or sets the QAT button caption.
        /// </summary>
        /// <value>The QAT button caption.</value>
        public string QATButtonCaption
        {
            get { return (string)GetValue(QATButtonCaptionProperty); }
            set { SetValue(QATButtonCaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QATButtonCaption.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty QATButtonCaptionProperty =
            DependencyProperty.Register("QATButtonCaption", typeof(string), typeof(QuickAccessToolBar), new PropertyMetadata("Show Below the Ribbon"));

        /// <summary>
        /// Gets or sets a value indicating whether this instance has geometry.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has geometry; otherwise, <c>false</c>.
        /// </value>
        public bool HasGeometry
        {
            get { return (bool)GetValue(HasGeometryProperty); }
            set { SetValue(HasGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasGeometry.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HasGeometryProperty =
            DependencyProperty.Register("HasGeometry", typeof(bool), typeof(QuickAccessToolBar), new PropertyMetadata(true, new PropertyChangedCallback(OnHasGeometryChanged)));

        /// <summary>
        /// Called when [has geometry changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHasGeometryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderobj = sender as QuickAccessToolBar;
            if (senderobj == null) return;

            senderobj.ShowHidePathGeometry();
        }

        /// <summary>
        /// Shows the hide path geometry.
        /// </summary>
        private void ShowHidePathGeometry()
        {
            if (this.HasGeometry)
                this.PathGeometryVisibility = Visibility.Visible;
            else
                this.PathGeometryVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Gets or sets the path geometry visibility.
        /// </summary>
        /// <value>The path geometry visibility.</value>
        public Visibility PathGeometryVisibility
        {
            get { return (Visibility)GetValue(PathGeometryVisibilityProperty); }
            set { SetValue(PathGeometryVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HidePathGeometry.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PathGeometryVisibilityProperty =
            DependencyProperty.Register("PathGeometryVisibility", typeof(Visibility), typeof(QuickAccessToolBar), new PropertyMetadata(Visibility.Visible, OnPathGeometryVisibilityChanged));

        /// <summary>
        /// 
        /// </summary>
        public Visibility SeperatorVisibility
        {
            get { return (Visibility)GetValue(SeperatorVisibilityProperty); }
            set { SetValue(SeperatorVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeperatorVisibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SeperatorVisibilityProperty =
            DependencyProperty.Register("SeperatorVisibility", typeof(Visibility), typeof(QuickAccessToolBar), new PropertyMetadata(Visibility.Visible));


        /// <summary>
        /// Called when [path geometry visibility changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPathGeometryVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderobj = sender as QuickAccessToolBar;
            if (senderobj == null) return;

        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnMoreCommandsClick = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnShowBelowtheRibbonClick = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnMinimizeRibbonClick = delegate { };

        /// <summary>
        /// Hides the popup.
        /// </summary>
        private void HidePopup()
        {
            if (PART_QATDropDownControl != null && PART_QATDropDownControl._dropdown != null)
                PART_QATDropDownControl._dropdown.IsOpen = false;
        }

        /// <summary>
        /// Handles the OnShowBelowtheRibbonClick event of the PART_QATDropDownControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PART_QATDropDownControl_OnShowBelowtheRibbonClick(object sender, EventArgs e)
        {
            OnShowBelowtheRibbonClick(sender, e);
            HidePopup();
        }

        /// <summary>
        /// Handles the OnMoreCommandsClick event of the PART_QATDropDownControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PART_QATDropDownControl_OnMoreCommandsClick(object sender, EventArgs e)
        {
            OnMoreCommandsClick(sender, e);
            HidePopup();
        }

        /// <summary>
        /// Handles the OnMinimizeRibbonClick event of the PART_QATDropDownControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PART_QATDropDownControl_OnMinimizeRibbonClick(object sender, EventArgs e)
        {
            OnMinimizeRibbonClick(sender, e);
            HidePopup();
        }

        ObservableCollection<UIElement> overFlowItems = new ObservableCollection<UIElement>();
        /// <summary>
        /// Gets the over flow items.
        /// </summary>
        /// <value>The over flow items.</value>
        public ObservableCollection<UIElement> OverFlowItems
        {
            get { return overFlowItems; }
        }

        /// <summary>
        /// Gets or sets the over flow visibility.
        /// </summary>
        /// <value>The over flow visibility.</value>
        public Visibility OverFlowVisibility
        {
            get { return (Visibility)GetValue(OverFlowVisibilityProperty); }
            set { SetValue(OverFlowVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverFlowVisibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OverFlowVisibilityProperty =
            DependencyProperty.Register("OverFlowVisibility", typeof(Visibility), typeof(QuickAccessToolBar), new PropertyMetadata(Visibility.Collapsed, OnOverFlowVisibilityChanged));

        /// <summary>
        /// Gets or sets a value indicating whether [save original state].
        /// </summary>
        /// <value><c>true</c> if [save original state]; otherwise, <c>false</c>.</value>
        public bool AutoPersist
        {
            get { return (bool)GetValue(AutoPersistProperty); }
            set { SetValue(AutoPersistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveOriginalState.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Indicates whether to save state persisted on loading.
        /// </summary>
        public static readonly DependencyProperty AutoPersistProperty =
            DependencyProperty.Register("AutoPersist", typeof(bool), typeof(QuickAccessToolBar), new PropertyMetadata(false));

        /// <summary>
        /// Called when [over flow visibility changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOverFlowVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private QATDropDownControl tempQATDropDownControl = null;

        /// <summary>
        /// Checks for over flow.
        /// </summary>
        private void CheckForOverFlow()
        {
            this.AddItemsInOverFlow();
            this.RemoveItemsInOverFlow();
            var getcntVal = from res in this.PART_OverflowItemsControl.Children.OfType<QATDropDownControl>()
                            select res;
            var overflowCount = this.PART_OverflowItemsControl.Children.Count;
            if (getcntVal.Count() > 0) overflowCount -= getcntVal.Count();

            if (this.Items.Count >= DefaultQATItemsCount && overflowCount >= 1)
            {
                this.ReplaceContentToOverflow();
                OverFlowVisibility = System.Windows.Visibility.Visible;
            }
            else if (this.Items.Count < DefaultQATItemsCount || overflowCount <= 1)
            {
                this.RemoveContentInOverFlow();
                OverFlowVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Adds the items in over flow.
        /// </summary>
        private void AddItemsInOverFlow()
        {
            while (this.Items.Count > DefaultQATItemsCount)
            {
                UIElement uielement = this.Items[DefaultQATItemsCount] as UIElement;
                if (this.Items.Count > 0)
                {
                    isOverFlowItemAdded = true;
                    this.Items.RemoveAt(DefaultQATItemsCount);
                }
                if (uielement != null)
                {
                    this.PART_OverflowItemsControl.Children.Insert(0, uielement);
                    //this.PART_OverflowItemsControl.Children.Add(uielement);
                    OverFlowItems.Insert(0, uielement);
                    CheckUnCheckQATMenuItem(uielement as FrameworkElement, true);
                }
            }
        }

        bool isOverFlowItemAdded = false;

        /// <summary>
        /// Removes the items in over flow.
        /// </summary>
        private void RemoveItemsInOverFlow()
        {
            while (this.Items.Count < DefaultQATItemsCount && this.PART_OverflowItemsControl.Children.Count > 1)
            {
                UIElement uielement = this.PART_OverflowItemsControl.Children[0] as UIElement;
                if (this.PART_OverflowItemsControl.Children.Count > 0)
                {
                    this.PART_OverflowItemsControl.Children.RemoveAt(0);
                    OverFlowItems.RemoveAt(0);
                }
                if (uielement != null)
                {
                    isOverFlowItemAdded = true;
                    this.Items.Add(uielement);
                }
            }
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (CurrentQATState == QATState.BelowRibbon)
            {
                if (!double.IsInfinity(availableSize.Width) && !double.IsNaN(availableSize.Width))
                {
                    var totalAvailCount = Convert.ToInt32(availableSize.Width / 30);
                    DefaultQATItemsCount = totalAvailCount;
                }
            }
            else
            {
                DefaultQATItemsCount = 10;
            }

            this.CheckForOverFlow();
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Removes the content in over flow.
        /// </summary>
        private void RemoveContentInOverFlow()
        {
            if (tempQATDropDownControl != null && this.PART_QATDropDownControl != null && this.PART_OverFlowTopContainer != null && this.PART_NormalContainer != null)
            {
                var cntnt = this.PART_OverFlowTopContainer.Children.OfType<QATDropDownControl>().FirstOrDefault();
                this.PART_OverFlowTopContainer.Children.Clear();
                this.PART_NormalContainer.Children.Clear();
                this.PART_NormalContainer.Children.Add(tempQATDropDownControl);
                tempQATDropDownControl = null;
            }
        }

        /// <summary>
        /// Replaces the content to overflow.
        /// </summary>
        private void ReplaceContentToOverflow()
        {
            if (this.PART_QATDropDownControl != null && this.PART_OverFlowTopContainer != null)
            {
                var content = this.PART_NormalContainer.Children.OfType<QATDropDownControl>().FirstOrDefault();
                if (content != null)
                {
                    tempQATDropDownControl = this.PART_QATDropDownControl;
                    this.PART_NormalContainer.Children.Clear();
                    this.PART_OverFlowTopContainer.Children.Clear();
                    this.PART_OverFlowTopContainer.Children.Add(content);
                }
                else if (tempQATDropDownControl != null)
                {
                    content = tempQATDropDownControl;
                    var cntnt = this.PART_OverFlowTopContainer.Children.OfType<QATDropDownControl>().FirstOrDefault();
                    if (cntnt == null && content != null)
                    {
                        this.PART_OverFlowTopContainer.Children.Clear();
                        this.PART_OverFlowTopContainer.Children.Add(content);
                    }
                }
            }
        }

        /// <summary>
        /// Key is parent original ribbon item and value is cloned item which is used in QAT
        /// </summary>
        internal Dictionary<object, object> QATItemCollections = new Dictionary<object, object>();

        internal event EventHandler OnQATItemsChanged;

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (OnQATItemsChanged != null)
                OnQATItemsChanged(this, e);
            if (PART_QATDropDownControl != null && PART_QATDropDownControl._dropdown != null)
            {
                RibbonMenuGroup _container = PART_QATDropDownControl._dropdown.Content as RibbonMenuGroup;

                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

                        if (_container != null)
                        {
                            RibbonButton _item = e.NewItems[0] as RibbonButton;
                            if (_item != null)
                            {
                                foreach (var item in _container.Items)
                                {
                                    RibbonMenuItem _menuItem = item as RibbonMenuItem;
                                    if (_menuItem != null)
                                    {
                                        if (_menuItem._provider != null)
                                        {
                                            if (_item.Command != null)
                                            {
                                                if (_menuItem._provider.CommandParameter == _item.CommandParameter && _menuItem._provider.Command == _item.Command)
                                                {
                                                    _menuItem.IsChecked = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (item is RibbonSeparator)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (e.NewItems != null && e.NewItems[0] != null && CanAddQATItem(e.NewItems[0]) == true && QATItemCollections != null && !QATItemCollections.ContainsKey(e.NewItems[0]) && isOverFlowItemAdded == false)
                            QATItemCollections.Add(e.NewItems[0], e.NewItems[0]);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:

                        if (_container != null)
                        {
                            foreach (var item in _container.Items)
                            {
                                RibbonMenuItem _menuItem = item as RibbonMenuItem;
                                if (_menuItem != null)
                                {
                                    _menuItem.IsChecked = false;
                                }
                                else if (item is RibbonSeparator)
                                {
                                    break;
                                }
                            }
                        }
                        //this.ClearOverFlowItems();
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        if (QATItemCollections != null && QATItemCollections.Count > 0 && e.OldItems != null && e.OldItems[0] != null && isOverFlowItemAdded == false)
                        {
                            var remEle = QATItemCollections.Where(res => res.Value == e.OldItems[0] || res.Key == e.OldItems[0]);
                            if (remEle.Count() > 0) QATItemCollections.Remove(remEle.FirstOrDefault().Key);
                        }

                        CheckUnCheckQATMenuItem(e.OldItems[0] as FrameworkElement, false);
                        break;
                    default:
                        break;
                }
            }
            isOverFlowItemAdded = false;
            PathGeometryVisibility = (this.Items.Count > 0 && this.HasGeometry == true) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Checks the un check QAT menu item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        private void CheckUnCheckQATMenuItem(FrameworkElement obj, bool isChecked)
        {
            if (obj is RibbonButton)
            {
                RibbonButton ribbonButton = (RibbonButton)obj;
                if (ribbonButton._provider != null)
                {
                    RibbonMenuGroup _container = PART_QATDropDownControl._dropdown.Content as RibbonMenuGroup;

                    var query = from UIElement menuItem in _container.Items where menuItem is RibbonMenuItem && ((RibbonMenuItem)menuItem)._provider == ribbonButton._provider select menuItem as RibbonMenuItem;
                    if (query.Count() > 0)
                    {
                        RibbonMenuItem selectedMenuItem = query.FirstOrDefault();

                        selectedMenuItem.IsCheckedChanged -= new PropertyChangedCallback(_item_IsCheckedChanged);

                        selectedMenuItem.IsChecked = isChecked;
                        selectedMenuItem._provider.IsSynchronizedwithQAT = isChecked;

                        selectedMenuItem.IsCheckedChanged += new PropertyChangedCallback(_item_IsCheckedChanged);
                    }
                }
            }
        }

        internal Ribbon parentRibbon = null;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            FrameworkElement realSource = RibbonContextMenu.GetRealSource(this, e);
            if (realSource == null) return;
            var parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon));
            if (parentRibbon != null && parentRibbon is Ribbon)
            {
                ((Ribbon)parentRibbon).RemoveQATinContextMenu(realSource);
            }
            base.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// Determines whether this instance [can add QAT item] the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can add QAT item] the specified item; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanAddQATItem(object item)
        {
            var remEle = from res in this.QATItemCollections
                         where res.Value == item || res.Key == item
                         select res;
            if (remEle.Count() <= 0) return true;
            return false;
        }

        /// <summary>
        /// Clears the over flow items.
        /// </summary>
        internal void ClearOverFlowItems()
        {
            if (this.PART_OverflowItemsControl != null)
            {
                this.PART_OverflowItemsControl.Children.Clear();
            }
        }

        /// <summary>
        /// Gets the qat item.
        /// </summary>
        /// <param name="commandprovider">The commandprovider.</param>
        /// <returns></returns>
        public IRibbonItem GetQatItem(RibbonCommandProvider commandprovider)
        {
            var items = from ICommandSource item in Items
                        where item.Command == commandprovider.Command && item.CommandParameter == commandprovider.CommandParameter
                        select item;

            foreach (var i in items)
            {
                return i as IRibbonItem;
            }
            return null;
        }

        //protected override DependencyObject GetContainerForItemOverride()
        //{
        //    return new RibbonButton() { SizeForm = SizeForm.Small };
        //}

        //protected override bool IsItemItsOwnContainerOverride(object item)
        //{
        //    return item is FrameworkElement;
        //}

        //protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        //{
        //    if (item is RibbonCommandProvider)
        //    {
        //        if (!((RibbonCommandProvider)item).IsItemsHost)
        //        {
        //            RibbonButton elemnt = element as RibbonButton;
        //            var cmdpro = (RibbonCommandProvider)item;
        //            elemnt.Label = cmdpro.Label;
        //            elemnt.SmallIcon = cmdpro.SmallIcon;
        //            elemnt.SizeForm = SizeForm.Small;
        //            ToolTipService.SetToolTip(elemnt, cmdpro.ToolTip);
        //            elemnt.Command = cmdpro.Command;
        //            elemnt.CommandParameter = cmdpro.CommandParameter;
        //            base.PrepareContainerForItemOverride(element, elemnt);
        //        }
        //        else
        //        {
        //            RibbonSplitButton elemnt = new RibbonSplitButton() { SizeForm = SizeForm.Small };
        //            var cmdpro = (RibbonCommandProvider)item;
        //            elemnt.Label = cmdpro.Label;
        //            elemnt.SmallIcon = cmdpro.SmallIcon;
        //            elemnt.Content = ((RibbonSplitButton)((RibbonCommandProvider)item).Host).Content;
        //            ToolTipService.SetToolTip(elemnt, cmdpro.ToolTip);
        //            elemnt.Command = cmdpro.Command;
        //            elemnt.CommandParameter = cmdpro.CommandParameter;
        //            //base.PrepareContainerForItemOverride(elemnt, elemnt);

        //        }
        //    }
        //    else
        //    {
        //        base.PrepareContainerForItemOverride(element, item);
        //    }
        //}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ContentControl cctrl = this.Parent as ContentControl;
            if (cctrl != null)
            {
                // cctrl.Content = null;                
            }
            base.Items.Clear();
            this.Loaded -= new RoutedEventHandler(QuickAccessToolBar_Loaded);
            //this.Unloaded -= new RoutedEventHandler(QuickAccessToolBar_Unloaded);
        }
    }
}
