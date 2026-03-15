// <copyright file="SfTabItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Navigation
{
    /// <summary>
    /// Represents a selectable item inside <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
    /// </summary>
    /// <remarks>
    /// <b>SfTabItem </b>is a <see
    /// cref="N:Windows.UI.Xaml.Controls.ContentControl.">ContentControl</see>.
    /// </remarks>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
    /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
    /// Namespace</seealso>
    [ClassReference(IsReviewed = false)]
    public class SfTabItem : ContentControl,IDisposable
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfTabItem()
        {
            DefaultStyleKey = typeof(SfTabItem);
            LayoutUpdated+=SfTabItem_LayoutUpdated;
            IsEnabledChanged += SfTabItem_IsEnabledChanged;
            Unloaded += SfTabItem_Unloaded;
        }

       
        void SfTabItem_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= tabitem_Loaded;
            LayoutUpdated -= SfTabItem_LayoutUpdated;
            IsEnabledChanged -= SfTabItem_IsEnabledChanged;
            Unloaded -= SfTabItem_Unloaded;
        }

        void SfTabItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, "NormalState", true);
            if (IsEnabled)
            {
                if (IsSelected)
                {
                    if (parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                    {
                        VisualStateManager.GoToState(this, "NormalPointerOver", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "SelectedPointerOver", true);
                    }
                }
                else
                    VisualStateManager.GoToState(this, "NormalState", true);
            }
            else
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        void SfTabItem_LayoutUpdated(object sender, object e)
        {
            if (pinnableButton != null)
            {
                if (!IsSelected && pinnableButton.Opacity == 1 && parentTabControl!=null && parentTabControl.previousItem !=null && parentTabControl.previousItem == this)
                    pinnableButton.Opacity = 0;
                if (ShowPinnableButton)
                {
                    if (IsSelected || isPointerOver)
                    {
                        if (parentTabControl.PinnedItems != null && parentTabControl.pinnedpanel!=null)
                        {
                            if (parentTabControl.SelectedItem == null)
                            {
                                bool isselected=false;
                                for (int i = 0; i < parentTabControl.Items.Count;i++)
                                {
                                    if (parentTabControl.ItemsSource == null)
                                    {
                                        if ((parentTabControl.Items[i] as SfTabItem).IsSelected)
                                        {
                                            parentTabControl.SelectedItem = parentTabControl.Items[i];
                                            isselected = true;
                                            break;
                                        }
                                    }
                                    else if ((parentTabControl.ItemContainerGenerator.ContainerFromIndex(i) as SfTabItem).IsSelected)
                                    {
                                        parentTabControl.SelectedItem = (parentTabControl.ItemContainerGenerator.ContainerFromIndex(i) as SfTabItem);
                                        isselected = true;
                                        break;
                                    }
                                }
                                if (!isselected)
                                    parentTabControl.SelectedItem = this;
                            }
                            if (parentTabControl.pinnedpanel.Children.Contains(this))
                                VisualStateManager.GoToState(pinnableButton, "Pinned", true);
                            else
                                VisualStateManager.GoToState(pinnableButton, "UnPinned", true);
                            if (IsSelected && parentTabControl.SelectedContent == null)
                            {
                                if (Content != null)
                                    parentTabControl.SelectedContent = Content;
                                else
                                {
                                    int index = -1;
                                    foreach (SfTabItem item in parentTabControl.pinnedpanel.Children)
                                    {
                                        if (item.IsSelected)
                                        {
                                            index = parentTabControl.pinnedpanel.Children.IndexOf(item);
                                            break;
                                        }
                                    }
                                    if(index>-1)
                                        parentTabControl.SelectedContent = (parentTabControl.ItemContainerGenerator.ContainerFromIndex(index) as SfTabItem).Content;
                                }
                            }
                        }
                    }
                    else if (pinnableButton.Visibility == Visibility.Visible)
                        pinnableButton.Opacity = 0;
                }
                else
                {
                    pinnableButton.Visibility = Visibility.Collapsed;
                }
                if (IsSelected && parentTabControl!=null && parentTabControl.SelectedContent == null)
                    parentTabControl.SelectedContent = Content;
            }
            if (isinternalupdate)
            {
                isinternalupdate = false;
                if (parentTabControl.ItemsSource == null)
                {
                    foreach (SfTabItem item in parentTabControl.Items)
                    {
                        if (item.IsSelected)
                        {
                            if (item.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                            {
                                VisualStateManager.GoToState(item, "Normal", true);
                            }
                            else
                            {
                                VisualStateManager.GoToState(item, "Selected", true);
                            }
                        }
                        else
                        {
                            VisualStateManager.GoToState(item, "UnSelected", true);
                        }
                    }
                }
            }
        }
        #endregion

        #region Variables

        internal SfTabControl parentTabControl;

        private bool isPointerPressed;

        internal RepeatButton closeButton;

        internal RepeatButton pinnableButton;

        internal bool isinternalupdate;

        internal bool isPointerOver;

        #endregion      

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the data used as header for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>.
        /// </summary>
        /// <value>
        /// The default is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(SfTabItem), new PropertyMetadata("Untitled"));


        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof (DataTemplate), typeof (SfTabItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for the data used as header for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>.
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
            DependencyProperty.Register("HeaderTemplateSelector", typeof (DataTemplateSelector), typeof (SfTabItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets Template Selector for the data used as header for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>.
        /// </summary>
        /// <value>
        /// The default is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SfTabItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Gets or sets the selected background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(SfTabItem), new PropertyMetadata(null,OnSelectedBackgroundChanged));


        /// <summary>
        /// Gets or sets a value to determine if the tabitem can be closed or not.
        /// </summary>
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanClose.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(SfTabItem), new PropertyMetadata(true,OnCanCloseChanged));

        /// <summary>
        /// Gets or sets a value to determine if the tabitem can be pinned or not.
        /// </summary>
        public bool ShowPinnableButton
        {
            get { return (bool)GetValue(ShowPinnableButtonProperty); }
            set { SetValue(ShowPinnableButtonProperty, value); }
        }
        ///<summary>
        /// Using a DependencyProperty as the backing store for ShowPinnableButton.  This enables animation, styling, binding, etc...
        ///</summary>
        public static readonly DependencyProperty ShowPinnableButtonProperty =
            DependencyProperty.Register("ShowPinnableButton", typeof(bool), typeof(SfTabItem), new PropertyMetadata(false, OnShowPinnableButtonChanged));         

        /// <summary>
        /// Gets or sets the selected foreground for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Controls">Syncfusion.UI.Xaml.Controls
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(SfTabItem), new PropertyMetadata(null,OnSelectedForegroundChanged));
               
        #endregion

        #region Helper Methods

        public void Dispose()
        {
            parentTabControl = null;
            LayoutUpdated -= SfTabItem_LayoutUpdated;
            Loaded -= tabitem_Loaded;

            if (closeButton != null)
            {
                closeButton.Click -= closeButton_Click;
                closeButton.PointerCaptureLost -= closeButton_PointerCaptureLost;
                closeButton.PointerEntered -= closeButton_PointerEntered;
                closeButton.PointerExited -= closeButton_PointerExited;
            }

            if (pinnableButton != null)
            {
                pinnableButton.Click -= pinnableButton_Click;
                pinnableButton.PointerEntered -= pinnableButton_PointerEntered;
                pinnableButton.PointerCaptureLost -= pinnableButton_PointerCaptureLost;
            }
        }

        private void tabitem_Loaded(object sender, RoutedEventArgs e)
        {
            SfTabItem tabitem = sender as SfTabItem;
            if (!IsEnabled)
                VisualStateManager.GoToState(tabitem, "Disabled", true);
            if (tabitem != null && tabitem.parentTabControl != null)
            {
                tabitem.Loaded -= tabitem_Loaded;
                if (tabitem.IsSelected)
                {
                    int selectedIndex=tabitem.parentTabControl.ItemContainerGenerator.IndexFromContainer(tabitem);
                    tabitem.parentTabControl.SelectedIndex = selectedIndex!=-1?selectedIndex:tabitem.parentTabControl.SelectedIndex;
                    if (tabitem.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                    {
                        VisualStateManager.GoToState(tabitem, "Normal", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(tabitem, "Selected", true);
                    }
                }
                else
                {
                    VisualStateManager.GoToState(tabitem, "UnSelected", true);
                }
                if (tabitem.closeButton != null)
                    VisualStateManager.GoToState(tabitem.closeButton, "UnSelected", true);
            }          
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Initializes the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabItem"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            if (parentTabControl != null)
            {
                if (IsSelected)
                {
                    if (this.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                    {
                        VisualStateManager.GoToState(this, "Normal", true);
                        VisualStateManager.GoToState(this.parentTabControl, "Normal", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Selected", true);
                        VisualStateManager.GoToState(this.parentTabControl, "Selected", true);
                    }
                    this.Loaded += tabitem_Loaded;
                }
                else
                {

                    VisualStateManager.GoToState(this, "UnSelected", true);
                    VisualStateManager.GoToState(this.parentTabControl, "UnSelected", true);
                }
            }

            closeButton = GetTemplateChild("ClosableButton") as RepeatButton;
            if (closeButton != null)
            {
                closeButton.Click += closeButton_Click;
                closeButton.PointerCaptureLost += closeButton_PointerCaptureLost;
                closeButton.PointerEntered+=closeButton_PointerEntered;
                closeButton.PointerExited+=closeButton_PointerExited;
                VisualStateManager.GoToState(this.closeButton, "UnSelected", true);
            }
            CheckCloseButtonVisibilityOnLoad(closeButton);
            pinnableButton = GetTemplateChild("PinnableButton") as RepeatButton;
            if (pinnableButton != null)
            {
                pinnableButton.Click += pinnableButton_Click;
                pinnableButton.PointerEntered += pinnableButton_PointerEntered;
                pinnableButton.PointerCaptureLost += pinnableButton_PointerCaptureLost;
            }
            if (ShowPinnableButton && pinnableButton!=null)
                pinnableButton.Visibility = Visibility.Visible;
            else
            {
                if(pinnableButton!=null)
                    pinnableButton.Visibility = Visibility.Collapsed;
                if (parentTabControl!=null && parentTabControl.PinnedItems != null)
                    parentTabControl.PinnedItems.Clear();
                if (parentTabControl != null && parentTabControl.pinnedpanel != null)
                {
                    parentTabControl.pinnedpanel.Children.Clear();
                    parentTabControl.pinnedpanel.Visibility = Visibility.Collapsed;
                    if(parentTabControl.pinnedItemsGrid != null)
                        parentTabControl.pinnedItemsGrid.Visibility =Visibility.Collapsed;
                    if(parentTabControl.pinnedSeperator != null)
                        parentTabControl.pinnedSeperator.Visibility = Visibility.Collapsed;
                }
            }
            base.OnApplyTemplate();
        }

        void closeButton_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            CloseTab();
            if (this.parentTabControl.Items.Count == 0)
                this.parentTabControl.SelectedContent = null;
        }

        void closeButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {            
            VisualStateManager.GoToState(this.closeButton, "PointerOver", true);
        }

        void closeButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this.closeButton, "UnSelected", true);
        }
        
        void pinnableButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(pinnableButton, "PointerOverState", true);
        }

        void pinnableButton_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
           VisualStateManager.GoToState(pinnableButton, "Normal", true);
            PinTab();
        }
        /// <summary>
        /// Occurs when the pointer is over the tab items
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (parentTabControl != null)
            {
                if (parentTabControl.CloseButtonType == CloseButtonType.IndividualOnMouseOver || parentTabControl.CloseButtonType == CloseButtonType.Extended)
                {
                    this.closeButton.Opacity = 1;
                }
                if (ShowPinnableButton && pinnableButton != null && pinnableButton.Opacity==0)
                {
                    isPointerOver = true;
                    if (ShowPinnableButton && (IsSelected||isPointerOver))
                    {
                        if (parentTabControl.PinnedItems != null && parentTabControl.pinnedpanel!=null)
                        {
                            if (parentTabControl.pinnedpanel.Children.IndexOf(this)>=0)
                                VisualStateManager.GoToState(pinnableButton, "Pinned", true);
                            else
                                VisualStateManager.GoToState(pinnableButton, "UnPinned", true);
                        }
                    }
                    pinnableButton.Opacity=1;
                }
                if (IsSelected)
                {
                    if (parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                        VisualStateManager.GoToState(this, "NormalPointerOver", true);
                    else
                        VisualStateManager.GoToState(this, "SelectedPointerOver", true);
                }
                else
                    VisualStateManager.GoToState(this, "PointerOver", true);
            }
            base.OnPointerMoved(e);
        }

        /// <summary>
        /// Occurs when the pointer leaves the tab items
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (parentTabControl != null)
            {
                if (parentTabControl.CloseButtonType == CloseButtonType.IndividualOnMouseOver || parentTabControl.CloseButtonType == CloseButtonType.Extended)
                {
                    if (parentTabControl.CloseButtonType == CloseButtonType.Extended && this.IsSelected)
                        this.closeButton.Visibility = Visibility.Visible;
                    else
                        this.closeButton.Opacity=0;
                }
                if (ShowPinnableButton && pinnableButton != null && pinnableButton.Opacity==1 && !IsSelected)
                {
                    isPointerOver = false;
                    pinnableButton.Opacity=0;
                }
                if (!this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "UnSelected", true);
                }
                else
                {
                    if (this.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                        VisualStateManager.GoToState(this, "Normal", true);
                    else
                        VisualStateManager.GoToState(this, "Selected", true);
                }
            }
            base.OnPointerExited(e);
        }

        /// <summary>
        /// Checks the close button visibility on load.
        /// </summary>
        /// <param name="closebutton">The closebutton.</param>
        internal void CheckCloseButtonVisibilityOnLoad(RepeatButton closebutton)
        {
            if (closebutton != null)
            {
                if (CanClose)
                {
                    closebutton.Opacity = 1;
                    if (parentTabControl != null && parentTabControl.CloseButtonType == CloseButtonType.IndividualOnMouseOver)
                    {
                        closebutton.Visibility = Visibility.Visible;
                        closebutton.Opacity = 0;
                    }

                    else if (parentTabControl != null && (parentTabControl.CloseButtonType == CloseButtonType.Both || parentTabControl.CloseButtonType == CloseButtonType.Individual))
                    {
                        closebutton.Visibility = Visibility.Visible;
                    }
                    else if (parentTabControl != null && parentTabControl.CloseButtonType == CloseButtonType.Extended)
                    {
                        closebutton.Visibility = Visibility.Visible;
                        if (!IsSelected)
                            closebutton.Opacity = 0;
                    }
                    else
                    {
                        closebutton.Visibility = Visibility.Collapsed;
                    }
                }
                else
                    closeButton.Visibility = Visibility.Collapsed;
                closebutton.IsEnabled = true;
            }
        }

        internal void CloseTab()
        {
            if (this.CanClose)
            {
                System.Collections.IList list = parentTabControl.ItemsSource as System.Collections.IList;

                if (!parentTabControl.ClosingTab(this))
                {
                    if (parentTabControl.ItemsSource != null)
                    {
                        SfTabItem item = this;
                        if (item == null || item == DependencyProperty.UnsetValue)
                        {
                            return;
                        }
                        else
                        {
                            int index = list.IndexOf(this.DataContext);
                            bool isSelected = false;
                            if (index != -1)
                            {
                                SfTabItem tabitem = parentTabControl.ItemContainerGenerator.ContainerFromIndex(index) as SfTabItem;
                                if (tabitem != null && tabitem.IsSelected)
                                    isSelected = true;
                            }
                            list.RemoveAt(index);
                            if (parentTabControl.PinnedItems != null && parentTabControl.PinnedItems.Contains(this.DataContext))
                            {
                                int closedtabindex = parentTabControl.PinnedItems.IndexOf(this.DataContext); ;
                                parentTabControl.pinnedpanel.Children.RemoveAt(closedtabindex);
                                parentTabControl.PinnedItems.Remove(this.DataContext);                                
                            }
                            SfTabPanel panel = parentTabControl.GetTabPanel();
                            panel.Children.Remove(this);
                            if (isSelected)
                            {
                                foreach (object itemobj in list)
                                {
                                    SfTabItem actualItem = parentTabControl.ItemContainerGenerator.ContainerFromItem(itemobj) as SfTabItem;
                                    if (list.IndexOf(itemobj) == (index - 1))
                                        parentTabControl.SelectedItem = itemobj;
                                    else if (index - 1 == -1)
                                    {
                                        parentTabControl.SelectedItem = itemobj;
                                        break;
                                    }
                                    else
                                        actualItem.IsSelected = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        int index = this.parentTabControl.Items.IndexOf(this);
                        bool isSelected = false;
                        if(index != -1)
                            isSelected=(parentTabControl.Items[index] as SfTabItem).IsSelected;
                        object header = null;
                        if(index!=-1 && index<parentTabControl.Items.Count)
                             header = (parentTabControl.Items[index] as SfTabItem).Header;
                        this.parentTabControl.Items.Remove(this);
                        if (parentTabControl.pinnedpanel!=null && parentTabControl.pinnedpanel.Children.Count>0 && index!=-1 && index < parentTabControl.pinnedpanel.Children.Count)
                        {
                            parentTabControl.pinnedpanel.Children.RemoveAt(index);
                            if (parentTabControl.PinnedItems.Count != 0 && parentTabControl.PinnedItems.Contains(this))
                                parentTabControl.PinnedItems.RemoveAt(index);
                        }
                        if (parentTabControl.PinnedItems != null)
                        {
                            SfTabItem actualItem = null;
                            foreach (SfTabItem item in parentTabControl.Items)
                            {
                                if (item.Header.Equals(this.Header))
                                {
                                    actualItem = item;
                                    break;
                                }
                            }
                            if (actualItem == null && parentTabControl!=null && parentTabControl.pinnedpanel != null && parentTabControl.pinnedpanel.Children.Count!=0)
                            {
                                foreach (var tabitem in parentTabControl.pinnedpanel.Children)
                                {
                                    if ((tabitem as SfTabItem).Header == header)
                                    {
                                        actualItem = tabitem as SfTabItem;
                                        break;
                                    }
                                }
                                parentTabControl.pinnedpanel.Children.Remove(actualItem);
                                if(parentTabControl.PinnedItems.Count!=0 && parentTabControl.PinnedItems.Contains(this))
                                    parentTabControl.PinnedItems.Remove(this);
                            }
                            if (actualItem != null && parentTabControl.pinnedpanel!=null)
                            {
                                int closedtabindex = parentTabControl.pinnedpanel.Children.IndexOf(this);
                                if(closedtabindex!=-1 && closedtabindex < parentTabControl.Items.Count)
                                    isSelected = (parentTabControl.Items[closedtabindex] as SfTabItem).IsSelected;
                                if (closedtabindex >= 0)
                                {
                                    if((parentTabControl.Items[closedtabindex] as SfTabItem).Header==(parentTabControl.pinnedpanel.Children[closedtabindex] as SfTabItem).Header)
                                        parentTabControl.Items.RemoveAt(closedtabindex);
                                    parentTabControl.pinnedpanel.Children.RemoveAt(closedtabindex);
                                    parentTabControl.PinnedItems.RemoveAt(closedtabindex);
                                    index = closedtabindex;
                                }
                            }
                        }                        
                        if (isSelected)
                        {
                            foreach (SfTabItem item in parentTabControl.Items)
                            {
                                if (parentTabControl.Items.IndexOf(item) == (index - 1))
                                    parentTabControl.SelectedItem = item;
                                else if (index - 1 == -1)
                                {
                                    parentTabControl.SelectedItem = item;
                                    break;
                                }
                                else
                                    item.IsSelected = false;
                            }
                        }
                    }
                }
                parentTabControl.UpdateLayout();
                parentTabControl.ClosedTab(this);
                if ((parentTabControl.ItemsSource != null && list.Count == 0) || (parentTabControl.ItemsSource == null && parentTabControl.Items.Count == 0))
                {
                    parentTabControl.commonCloseBorder.Visibility = Visibility.Collapsed;
                }
            }
            if (parentTabControl!=null && parentTabControl.pinnedpanel != null && parentTabControl.pinnedpanel.Children.Count == 0)
            {
                parentTabControl.pinnedpanel.Visibility = Visibility.Collapsed;
                if (parentTabControl.pinnedItemsGrid != null)
                    parentTabControl.pinnedItemsGrid.Visibility = Visibility.Collapsed;
                if (parentTabControl.pinnedSeperator != null)
                    parentTabControl.pinnedSeperator.Visibility = Visibility.Collapsed;
            }
            if (parentTabControl.Items.Count == 0)
                parentTabControl.ContentTemplate = null;
        }

        internal void PinTab()
        {
            parentTabControl.pinnedpanel.Visibility = Visibility.Visible;
            parentTabControl.pinnedItemsGrid.Visibility = Visibility.Visible;
            parentTabControl.pinnedSeperator.Visibility = Visibility.Visible;
            bool ispinned = true;
            isPointerOver = false;
            parentTabControl.internalremove = true;
            if (parentTabControl.ItemsSource != null)
            {
                System.Collections.IList list = parentTabControl.ItemsSource as System.Collections.IList;
                SfTabItem item = this;
                if (item == null || item == DependencyProperty.UnsetValue)
                {
                    return;
                }
                else
                {
                    if (parentTabControl.PinnedItems == null)
                        parentTabControl.PinnedItems = new ObservableCollection<object>();
                    foreach (SfTabItem tabitem in parentTabControl.pinnedpanel.Children)
                    {
                        if (tabitem == this)
                        {
                            ispinned = false;
                            parentTabControl.PinnedItems.Remove(this.DataContext);
                            break;
                        }
                    }
                    if (ispinned)
                    {
                        parentTabControl.PinnedItems.Add(this.DataContext);
                        SfTabItem clonedItem = new SfTabItem();                        
                        Cloning(clonedItem);
                        parentTabControl.pinnedpanel.Children.Add(clonedItem);
                        isinternalupdate = true; 
                        if (list != null)
                            list.Remove(this.DataContext);
                        int index = parentTabControl.pinnedpanel.Children.Count > 0
                                        ? parentTabControl.pinnedpanel.Children.Count - 1
                                        : 0;
                        if (list != null)
                            list.Insert(index, this.DataContext);
                        SfTabItem updatedItem = parentTabControl.ItemContainerGenerator.ContainerFromItem(DataContext) as SfTabItem;
                        if(updatedItem != null)
                            updatedItem.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        bool isselected= (parentTabControl.ItemContainerGenerator.ContainerFromIndex(parentTabControl.pinnedpanel.Children.IndexOf(this)) as SfTabItem).IsSelected;
                        parentTabControl.pinnedpanel.Children.RemoveAt(parentTabControl.pinnedpanel.Children.IndexOf(this));
                        if (list != null)
                            list.Remove(this.DataContext);
                        int index = 0;
                        foreach (object tabItem in parentTabControl.Items)
                        {
                            SfTabItem actualitem = parentTabControl.ItemContainerGenerator.ContainerFromItem(tabItem) as SfTabItem;
                            if (actualitem.Visibility == Visibility.Collapsed)
                                index++;
                            else
                                break;
                        }
                        if (list != null)
                            list.Insert(index, this.DataContext);
                        SfTabItem actualItem = parentTabControl.ItemContainerGenerator.ContainerFromItem(this.DataContext) as SfTabItem;
                        if(actualItem != null)
                        {
                            actualItem.Visibility = Visibility.Visible;
                            if(!actualItem.IsSelected && isselected)
                                actualItem.IsSelected = isselected;
                        }                   
                    }
                }
            }
            else
            {
                if (parentTabControl.PinnedItems == null)
                    parentTabControl.PinnedItems = new ObservableCollection<object>();
                foreach (SfTabItem tabitem in parentTabControl.pinnedpanel.Children)
                {
                    if (tabitem == this)
                    {
                        ispinned = false;
                        break;
                    }
                }
                if (ispinned)
                {
                    parentTabControl.PinnedItems.Add(this);
                    SfTabItem clonedItem = new SfTabItem();
                    Cloning(clonedItem);
                    parentTabControl.pinnedpanel.Children.Add(clonedItem);
                    int index = parentTabControl.pinnedpanel.Children.Count > 0
                                    ? parentTabControl.pinnedpanel.Children.Count - 1
                                    : 0;
                    isinternalupdate = true;
                    parentTabControl.Items.RemoveAt(parentTabControl.Items.IndexOf(this));
                    parentTabControl.Items.Insert(index, this);
                    SfTabItem updatedItem = parentTabControl.Items[index] as SfTabItem;
                    if (updatedItem != null)
                        updatedItem.Visibility = Visibility.Collapsed;
                }
                else
                {
                    int index = parentTabControl.pinnedpanel.Children.IndexOf(this);
                    bool isselected = (parentTabControl.pinnedpanel.Children[index] as SfTabItem).IsSelected;
                    parentTabControl.pinnedpanel.Children.RemoveAt(index);
                    SfTabItem actualItem = null;
                    foreach (SfTabItem item in parentTabControl.Items)
                    {
                        if (item.Header.Equals(this.Header))
                            actualItem = item;
                    }
                    parentTabControl.Items.Remove(actualItem);
                    if(parentTabControl!=null && parentTabControl.PinnedItems.Contains(actualItem))
                        parentTabControl.PinnedItems.Remove(actualItem);
                    int insertindex = 0;
                    foreach (SfTabItem tabItem in parentTabControl.Items)
                    {
                        if (tabItem.Visibility == Visibility.Collapsed)
                            insertindex++;
                        else
                            break;
                    }
                    if (actualItem != null)
                    {
                        parentTabControl.Items.Insert(insertindex, actualItem);
                        actualItem.Visibility = Visibility.Visible;
                        if(!actualItem.IsSelected && isselected)
                            actualItem.IsSelected = isselected;
                        if (!actualItem.IsSelected)
                            VisualStateManager.GoToState(actualItem, "UnSelected", true);
                    }
                    parentTabControl.PinnedItems.Remove(this);  
                }
            }
            CheckCloseButtonVisibilityOnLoad(closeButton);
            isinternalupdate = true;
            parentTabControl.internalremove = true;
            if (parentTabControl.pinnedpanel.Children.Count == 0)
            {
                parentTabControl.pinnedpanel.Visibility = Visibility.Collapsed;
                parentTabControl.pinnedItemsGrid.Visibility = Visibility.Collapsed;
                parentTabControl.pinnedSeperator.Visibility = Visibility.Collapsed;
            }
            parentTabControl.UpdateLayout();
            if (parentTabControl.SelectionStyle == SelectionStyle.CompleteHeader)
            {
                if (parentTabControl != null && parentTabControl.pinnedpanel != null && parentTabControl.pinnedpanel.Children.Count > 0 && ((parentTabControl.pinnedpanel.Children[parentTabControl.pinnedpanel.Children.Count - 1] as SfTabItem).SelectedBackground as SolidColorBrush).Color != ((parentTabControl.ItemContainerGenerator.ContainerFromIndex(parentTabControl.pinnedpanel.Children.Count - 1) as SfTabItem).SelectedBackground as SolidColorBrush).Color)
                    (parentTabControl.pinnedpanel.Children[parentTabControl.pinnedpanel.Children.Count - 1] as SfTabItem).SelectedBackground = (parentTabControl.ItemContainerGenerator.ContainerFromIndex(parentTabControl.pinnedpanel.Children.Count - 1) as SfTabItem).SelectedBackground;
            }
            else
            {
                if (parentTabControl != null && parentTabControl.pinnedpanel != null && parentTabControl.pinnedpanel.Children.Count > 0 && ((parentTabControl.pinnedpanel.Children[parentTabControl.pinnedpanel.Children.Count - 1] as SfTabItem).SelectedForeground as SolidColorBrush).Color != ((parentTabControl.ItemContainerGenerator.ContainerFromIndex(parentTabControl.pinnedpanel.Children.Count - 1) as SfTabItem).SelectedForeground as SolidColorBrush).Color)
                    (parentTabControl.pinnedpanel.Children[parentTabControl.pinnedpanel.Children.Count - 1] as SfTabItem).SelectedForeground = (parentTabControl.ItemContainerGenerator.ContainerFromIndex(parentTabControl.pinnedpanel.Children.Count - 1) as SfTabItem).SelectedForeground;
            }

            bool horizontalplacement = parentTabControl.TabStripPlacement == TabStripPlacement.Left ||
                                           parentTabControl.TabStripPlacement == TabStripPlacement.Right
                                               ? false
                                               : true;
            if (horizontalplacement)
            {
                
                if (parentTabControl!=null && parentTabControl.PinnedScrollViewer!=null && parentTabControl.PinnedScrollViewer.ViewportWidth > 0)
                {
                    parentTabControl.PinnedScrollViewer.ScrollToHorizontalOffset(0);
                    double completewidth = (parentTabControl.PinnedScrollViewer.ViewportWidth+
                                            parentTabControl.ScrollViewer.ViewportWidth);
                    if (parentTabControl.commonButtonGrid!=null)
                        completewidth -=  parentTabControl.pinnedCommonButtonGrid.ActualWidth;
                    parentTabControl.PinnedScrollViewer.MaxWidth = completewidth - completewidth / 1.5;
                }
            }
            else
            {
                if (parentTabControl != null && parentTabControl.PinnedScrollViewer != null && parentTabControl.PinnedScrollViewer.ViewportHeight > 0)
                {
                    parentTabControl.PinnedScrollViewer.ScrollToVerticalOffset(0);
                    double completeheight = (parentTabControl.PinnedScrollViewer.ViewportHeight +
                                             parentTabControl.ScrollViewer.ViewportHeight);
                    if (parentTabControl.commonButtonGrid!=null)
                        completeheight -= parentTabControl.pinnedCommonButtonGrid.ActualHeight;                   
                    parentTabControl.PinnedScrollViewer.MaxHeight = completeheight - completeheight / 1.5;
                }
            }
        }

        internal void Cloning(SfTabItem newItem)
        {
            newItem.parentTabControl = parentTabControl;
            newItem.DataContext = DataContext;
            Binding style = new Binding();
            style.Source = this;
            style.Path = new PropertyPath("Style");
            style.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.StyleProperty, style);
            Binding foreground = new Binding();
            foreground.Source = this;
            foreground.Path = new PropertyPath("Foreground");
            foreground.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.ForegroundProperty, foreground);
            Binding background = new Binding();
            background.Source = this;
            background.Path = new PropertyPath("Background");
            background.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.BackgroundProperty, background);
            Binding header = new Binding();
            header.Source = this;
            header.Path = new PropertyPath("Header");
            header.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.HeaderProperty, header);
            Binding headerTemplate = new Binding();
            headerTemplate.Source = this;
            headerTemplate.Path = new PropertyPath("HeaderTemplate");
            headerTemplate.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.HeaderTemplateProperty, headerTemplate);
            Binding headerTemplateSelector = new Binding();
            headerTemplateSelector.Source = this;
            headerTemplateSelector.Path = new PropertyPath("HeaderTemplateSelector");
            headerTemplateSelector.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.HeaderTemplateSelectorProperty, headerTemplateSelector);
            Binding canClose = new Binding();
            canClose.Source = this;
            canClose.Path = new PropertyPath("CanClose");
            canClose.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.CanCloseProperty, canClose);
            Binding showPinnableButton = new Binding();
            showPinnableButton.Source = this;
            showPinnableButton.Path = new PropertyPath("ShowPinnableButton");
            showPinnableButton.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.ShowPinnableButtonProperty, showPinnableButton);
            Binding isSelected = new Binding();
            isSelected.Source = this;
            isSelected.Path = new PropertyPath("IsSelected");
            isSelected.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.IsSelectedProperty, isSelected);
            Binding selectedBackground = new Binding();
            selectedBackground.Source = this;
            selectedBackground.Path = new PropertyPath("SelectedBackground");
            selectedBackground.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.SelectedBackgroundProperty, selectedBackground);
            Binding selectedForeground = new Binding();
            selectedForeground.Source = this;
            selectedForeground.Path = new PropertyPath("SelectedForeground");
            selectedForeground.Mode = BindingMode.TwoWay;
            newItem.SetBinding(SfTabItem.SelectedForegroundProperty, selectedForeground);
        }

        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSelected)
            {
                if (parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                    VisualStateManager.GoToState(this.closeButton, "HeadertextPressed", true);
                else
                    VisualStateManager.GoToState(this.closeButton, "CompleteHeaderPressed", true);
            }
            else
                VisualStateManager.GoToState(this.closeButton, "Pressed", true);
        }
        
        void pinnableButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSelected)
            {
                if (this.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                    VisualStateManager.GoToState(pinnableButton, "HeaderTextPressed", true);
                else
                    VisualStateManager.GoToState(pinnableButton, "CompleteHeaderPressed", true);
            }
            else
                VisualStateManager.GoToState(pinnableButton, "NormalPressed", true);
        }

        /// <summary>
        /// Invoked when the pointer is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            isPointerPressed = true;
            base.OnPointerPressed(e);
        }

        /// <summary>
        /// Invoked when the pointer is released
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (parentTabControl != null && !IsSelected && isPointerPressed)
            {
                foreach (var item in parentTabControl.Items)
                {
                    SfTabItem tabitem=null;
                    if(parentTabControl.ItemsSource==null)
                        tabitem = item as SfTabItem;
                    else
                        tabitem=parentTabControl.ItemContainerGenerator.ContainerFromIndex(parentTabControl.Items.IndexOf(item)) as SfTabItem;
                    if (tabitem != null && tabitem.IsSelected && tabitem != this && tabitem != parentTabControl.previousItem)
                    {
                        tabitem.IsSelected = false;
                        if (tabitem.pinnableButton != null && tabitem.ShowPinnableButton)
                            tabitem.pinnableButton.Opacity = 0;
                        VisualStateManager.GoToState(tabitem, "NormalState", true);
                    }
                }
                if (parentTabControl.previousItem != null)
                {
                    parentTabControl.previousItem.IsSelected = false;
                    if (parentTabControl.previousItem.ShowPinnableButton && parentTabControl.previousItem.pinnableButton != null)
                        parentTabControl.previousItem.pinnableButton.Opacity = 0;
                    SfTabItem tabItem = null;
                    int index = this.parentTabControl.ItemContainerGenerator.IndexFromContainer(parentTabControl.previousItem);
                    if (index > 0)
                        tabItem = this.parentTabControl.ItemContainerGenerator.ContainerFromIndex(index) as SfTabItem;
                    else
                        tabItem = this.parentTabControl.ItemContainerGenerator.ContainerFromIndex(0) as SfTabItem;
                    if (parentTabControl.CloseButtonType == CloseButtonType.Extended && tabItem.closeButton != null)
                    {
                        tabItem.closeButton.Opacity=0;
                    }
                }
                parentTabControl.previousItem = this;
                IsSelected = true;
            }
            isPointerPressed = false;
            base.OnPointerReleased(e);
        }

        /// <summary>
        /// Defines the Selected content when the content is changed.
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (parentTabControl != null && parentTabControl.SelectedContent != this.Content && parentTabControl.SelectedItem != null)
            {
                if (parentTabControl.SelectedItem.Equals(this))
                    parentTabControl.SelectedContent = this.Content;
            }
            base.OnContentChanged(oldContent, newContent);
        }
        #endregion

        #region Callback Methods
        
        private static void OnSelectedForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfTabItem tabitem = sender as SfTabItem;
            if (tabitem != null && tabitem.IsSelected && tabitem.parentTabControl!=null)
            {
                VisualStateManager.GoToState(tabitem, "UnSelected", true);
                if(tabitem.parentTabControl.SelectionStyle==SelectionStyle.HeaderText)
                    VisualStateManager.GoToState(tabitem, "Normal", true);
                else
                    VisualStateManager.GoToState(tabitem, "Selected", true);
            }
        }

        private static void OnSelectedBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfTabItem tabitem = sender as SfTabItem;
            if (tabitem != null && tabitem.IsSelected && tabitem.parentTabControl != null && args.NewValue!=null)
            {
                VisualStateManager.GoToState(tabitem, "UnSelected", true);
                if (tabitem.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                    VisualStateManager.GoToState(tabitem, "Normal", true);
                else
                    VisualStateManager.GoToState(tabitem, "Selected", true);
            }
        }

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfTabItem tabitem = sender as SfTabItem;
            if (tabitem != null && tabitem.parentTabControl != null)
            {
                if (tabitem.IsSelected)
                {
                    if (tabitem.parentTabControl.pinnedpanel != null)
                    {
                        foreach (SfTabItem pinitem in tabitem.parentTabControl.pinnedpanel.Children)
                        {
                            if (tabitem.parentTabControl.pinnedpanel.Children.Contains(tabitem) && pinitem != tabitem)
                            {
                                pinitem.IsSelected = false;
                                if (pinitem.pinnableButton != null)
                                    pinitem.pinnableButton.Opacity = 0;
                            }
                        }
                        if (tabitem.parentTabControl.pinnedpanel.Children.Contains(tabitem) && tabitem.parentTabControl.ItemsSource == null)
                        {
                            foreach (SfTabItem unpinneditem in tabitem.parentTabControl.Items)
                            {
                                if (unpinneditem.Visibility == Visibility.Visible)
                                {
                                    unpinneditem.IsSelected = false;
                                    if (unpinneditem.pinnableButton != null && unpinneditem.ShowPinnableButton)
                                        unpinneditem.pinnableButton.Opacity = 0;
                                }
                            }
                        }
                    }
                    if (tabitem.parentTabControl.ItemsSource != null)
                    {
                        tabitem.parentTabControl.SelectedIndex = tabitem.parentTabControl.Items.IndexOf(tabitem.DataContext);
                        tabitem.parentTabControl.SelectedItem = tabitem.DataContext;
                    }
                    else
                    {
                        int index=tabitem.parentTabControl.Items.IndexOf(tabitem);
                        if (index >= 0)
                            tabitem.parentTabControl.SelectedIndex = index;
                    }
                    if (tabitem.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                    {
                        VisualStateManager.GoToState(tabitem, "Normal", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(tabitem, "Selected", true);
                    }
                }
                else
                {
                    VisualStateManager.GoToState(tabitem, "UnSelected", true);
                    VisualStateManager.GoToState(tabitem.parentTabControl, "UnSelected", true);
                }
            }
            tabitem.CheckCloseButtonVisibilityOnLoad(tabitem.closeButton);
        }

        private static void OnCanCloseChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfTabItem tabitem = sender as SfTabItem;
            if (tabitem != null && tabitem.closeButton != null)
            {
                tabitem.CheckCloseButtonVisibilityOnLoad(tabitem.closeButton);
            }
        }

        private static void OnShowPinnableButtonChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfTabItem tabitem = sender as SfTabItem;
            if (tabitem != null && tabitem.pinnableButton != null)
            {
                if ((bool)args.NewValue == true)
                {
                    tabitem.pinnableButton.Visibility = Visibility.Visible;
                    if (!tabitem.IsSelected)
                        tabitem.pinnableButton.Opacity = 0;
                    else
                    {
                        if (tabitem.parentTabControl.SelectionStyle == SelectionStyle.HeaderText)
                            VisualStateManager.GoToState(tabitem, "NormalPointerOver", true);
                        else
                            VisualStateManager.GoToState(tabitem, "SelectedPointerOver", true);
                    }
                    VisualStateManager.GoToState(tabitem.pinnableButton, "UnPinned", true);
                }
                else
                {
                    tabitem.pinnableButton.Visibility = Visibility.Collapsed;
                    int index = tabitem.parentTabControl.Items.IndexOf(tabitem);
                    if (index == -1)
                    {
                        if(tabitem.parentTabControl!=null && tabitem.parentTabControl.pinnedpanel!=null && tabitem.parentTabControl.pinnedpanel.Children.Count > 0)
                            index =tabitem.parentTabControl.pinnedpanel.Children.IndexOf(tabitem.parentTabControl.pinnedpanel.Children.OfType<SfTabItem>().Where(item => item.Header==tabitem.Header).FirstOrDefault());
                        if (index != -1)
                        {
                            SfTabItem actualItem = tabitem.parentTabControl.ItemContainerGenerator.ContainerFromIndex(index) as SfTabItem;
                            if (actualItem != null && actualItem.ShowPinnableButton)
                                tabitem.ShowPinnableButton = true;
                        }
                    }
                    if (tabitem.parentTabControl.ItemsSource == null && tabitem.parentTabControl.PinnedItems != null && tabitem.parentTabControl.PinnedItems.Contains(tabitem))
                        if (!tabitem.isinternalupdate && (index<0 ||((tabitem.parentTabControl.pinnedpanel.Children[index] as SfTabItem)!=null && !(tabitem.parentTabControl.pinnedpanel.Children[index] as SfTabItem).ShowPinnableButton)))
                        {
                            tabitem.parentTabControl.PinnedItems.Remove(tabitem);
                        }
                        else if (tabitem.parentTabControl.ItemsSource != null && tabitem.parentTabControl.PinnedItems != null && tabitem.parentTabControl.PinnedItems.Contains(tabitem.DataContext))
                            if (!tabitem.isinternalupdate && (index < 0 || !(tabitem.parentTabControl.pinnedpanel.Children[index] as SfTabItem).ShowPinnableButton))
                            {
                                tabitem.parentTabControl.PinnedItems.Remove(tabitem.DataContext);
                            }
                    tabitem.isinternalupdate = false;
                }
                tabitem.parentTabControl.UpdateLayout();
            }
        }

        #endregion
       
    }
}
