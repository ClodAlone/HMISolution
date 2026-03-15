#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Browser;
using System.Collections;
using System.Linq;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Partially calls DockingManager Control,these class contains the dependencey property
    /// </summary>
    public partial class DockingManager
    {


        /// <summary>   
        /// Flag indicating Navigator/Firefox/Safari or Internet Explorer   
        /// </summary>   
        private static bool _isNavigator;

        /// <summary>   
        /// Gets the window object's client width   
        /// </summary>   
        public double ClientWidth
        {
            get
            {
                //return 600;
                return Application.Current.IsRunningOutOfBrowser ? ActualWidth:_isNavigator ? (double)HtmlPage.Window.GetProperty("innerWidth")
                    : (double)HtmlPage.Document.Body.GetProperty("clientWidth");
            }

        }

        /// <summary>   
        /// Gets the window object's client height   
        /// </summary>   
        public double ClientHeight
        {
            get
            {
                //return 600;
                return Application.Current.IsRunningOutOfBrowser ? ActualHeight:_isNavigator ? (double)HtmlPage.Window.GetProperty("innerHeight")
                    : (double)HtmlPage.Document.Body.GetProperty("clientHeight");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal DockingManager ParentDockingManager;

        /// <summary>
        /// Gets or sets the size of the content.
        /// </summary>
        /// <value>The size of the content.</value>
        protected internal double ContentSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dock fill].
        /// </summary>
        /// <value><c>true</c> if [dock fill]; otherwise, <c>false</c>.</value>
        public bool DockFill
        {
            get
            {
                return (bool)GetValue(DockFillProperty);
            }

            set
            {
                SetValue(DockFillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the active window.
        /// </summary>
        /// <value>The active window.</value>
        public Window ActiveWindow
        {
            get
            {
                return (Window)GetValue(ActiveWindowProperty);
            }

            set
            {
                SetValue(ActiveWindowProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the auto hide animation speed.
        /// </summary>
        /// <value>The auto hide animation speed.</value>
        public int AutoHideAnimationSpeed
        {
            get
            {
                return (int)GetValue(AutoHideAnimationSpeedProperty);
            }

            set
            {
                SetValue(AutoHideAnimationSpeedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the side tab background.
        /// </summary>
        /// <value>The side tab background.</value>
        public Brush SideTabBackground
        {
            get
            {
                return (Brush)GetValue(SideTabBackgroundProperty);
            }

            set
            {
                SetValue(SideTabBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the window background.
        /// </summary>
        /// <value>The window background.</value>
        public Brush WindowBackground
        {
            get
            {
                return (Brush)GetValue(WindowBackgroundProperty);
            }

            set
            {
                SetValue(WindowBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom over image path.
        /// </summary>
        /// <value>The bottom over image path.</value>
        public string BottomOverImagePath
        {
            get
            {
                return (string)GetValue(BottomOverImagePathProperty);
            }

            set
            {
                SetValue(BottomOverImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the left over image path.
        /// </summary>
        /// <value>The left over image path.</value>
        public string LeftOverImagePath
        {
            get
            {
                return (string)GetValue(LeftOverImagePathProperty);
            }

            set
            {
                SetValue(LeftOverImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right over image path.
        /// </summary>
        /// <value>The right over image path.</value>
        public string RightOverImagePath
        {
            get
            {
                return (string)GetValue(RightOverImagePathProperty);
            }

            set
            {
                SetValue(RightOverImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the top over image path.
        /// </summary>
        /// <value>The top over image path.</value>
        public string TopOverImagePath
        {
            get
            {
                return (string)GetValue(TopOverImagePathProperty);
            }

            set
            {
                SetValue(TopOverImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the center over image path.
        /// </summary>
        /// <value>The center over image path.</value>
        public string CenterOverImagePath
        {
            get
            {
                return (string)GetValue(CenterOverImagePathProperty);
            }

            set
            {
                SetValue(CenterOverImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the center image path.
        /// </summary>
        /// <value>The center image path.</value>
        public string CenterImagePath
        {
            get
            {
                return (string)GetValue(CenterImagePathProperty);
            }

            set
            {
                SetValue(CenterImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom image path.
        /// </summary>
        /// <value>The bottom image path.</value>
        public string BottomImagePath
        {
            get
            {
                return (string)GetValue(BottomImagePathProperty);
            }

            set
            {
                SetValue(BottomImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the left image path.
        /// </summary>
        /// <value>The left image path.</value>
        public string LeftImagePath
        {
            get
            {
                return (string)GetValue(LeftImagePathProperty);
            }

            set
            {
                SetValue(LeftImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right image path.
        /// </summary>
        /// <value>The right image path.</value>
        public string RightImagePath
        {
            get
            {
                return (string)GetValue(RightImagePathProperty);
            }

            set
            {
                SetValue(RightImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the top image path.
        /// </summary>
        /// <value>The top image path.</value>
        public string TopImagePath
        {
            get
            {
                return (string)GetValue(TopImagePathProperty);
            }

            set
            {
                SetValue(TopImagePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackground
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }

            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OuterDockAbility OuterDockAbility
        {
            get
            {
                return (OuterDockAbility)GetValue(OuterDockAbilityProperty);
            }
            set
            {
                SetValue(OuterDockAbilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value whether to allow resizing of all the docked windows
        /// </summary>
        public bool FreezeLayout
        {
            get
            {
                return (bool)GetValue(FreezeLayoutProperty);
            }
            set
            {
                SetValue(FreezeLayoutProperty,value);
            }
        }

        /// <summary>
        /// Gets or sets the resource dictionary.
        /// </summary>
        /// <value>The resource dictionary.</value>
        protected internal ResourceDictionary ResourceDictionary
        {
            get
            {
                return (ResourceDictionary)GetValue(ResourceDictionaryProperty);
            }

            set
            {
                SetValue(ResourceDictionaryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tar get name collection.
        /// </summary>
        /// <value>The tar get name collection.</value>
        protected internal List<string> TarGetNameCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the window collection.
        /// </summary>
        /// <value>The window collection.</value>
        protected internal Dictionary<int, Window> WindowCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client element collection.
        /// </summary>
        /// <value>The client element collection.</value>
        protected internal Dictionary<int, UIElement> ClientElementCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the state of the dock.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static DockState GetDockState(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (DockState)element.GetValue(DockStateProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="dockstate">The needed boolean value.</param>
        public static void SetDockState(UIElement element, DockState dockstate)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(DockStateProperty, dockstate);
            //if (element != null)
            //{
            //    OnChangeState(element, dockstate);
            //}
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty DockStateProperty =
            DependencyProperty.RegisterAttached(
                "DockState",
                typeof(DockState),
                typeof(DockingManager),
                new PropertyMetadata(DockState.Dock, OnDockingStatePropertyChanged));

        /// <summary>
        /// Gets the dock ability.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static DockAbility GetDockAbility(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (DockAbility)element.GetValue(DockAbilityProperty);
        }

        /// <summary>
        /// Sets the dock abiiity.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dockAbility">The dock ability.</param>
        public static void SetDockAbility(UIElement element, DockAbility dockAbility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(DockAbilityProperty, dockAbility);
        }

        /// <summary>
        /// Specifies where user can dock an element inside other element using inner DragProviders
        /// </summary>
        public static readonly DependencyProperty DockAbilityProperty = DependencyProperty.RegisterAttached("DockAbility", typeof(DockAbility), typeof(DockingManager), new PropertyMetadata(DockAbility.All));

        /// <summary>
        /// Called when [change state].
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="dockstate">The dockstate.</param>
        private static void OnChangeState(UIElement el, DockState dockstate)
        {
            if (((FrameworkElement)el).Parent != null)
            {
                if (((FrameworkElement)el).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabItem cstabitem = ((FrameworkElement)el).Parent as CustomTabItem;
                    //cstabitem.OwnWindow.DockState = dockstate;
                    CustomTabControl cstab = cstabitem.Parent as CustomTabControl;
                    if (cstabitem.OwnWindow != null)
                    {
                        if (cstabitem.OwnWindow != null)
                        {
                            if (dockstate == DockState.Dock && ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                            {
                                cstabitem.OwnWindow.StateMaintanceForWindowContainer();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes from docking manager.
        /// </summary>
        /// <param name="d">The d.</param>
        protected internal void RemoveFromDockingManager(UIElement d)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabItem cstabitem = ((FrameworkElement)d).Parent as CustomTabItem;

                    List<Window> windowCollection = new List<Window>(WindowCollection.Values);

                    //windowCollection = windowquery.ToList();
                    //windowquery = windowCollection.Where(tempwindow => ((Window)tempwindow).CustomTabControl.Items.Contains(cstabitem));                 

                    CustomTabControl cstab = cstabitem.Parent as CustomTabControl;
                    Window releatedWindow = null;
                    if (cstab != null)
                    {
                        releatedWindow = cstabitem.OwnWindow.DockingManager.GetWindow(cstab);
                        if (releatedWindow == null)
                        {
                            releatedWindow = cstab.RelatedWindow;
                        }
                    }
                    if (cstabitem.OwnWindow != null)
                    {
                        DockManager dm = releatedWindow.DockManager;
                        DockManager oldDockmanager = cstabitem.OwnWindow.OldValueDockManager;
                        DockManager dockmanager = cstabitem.OwnWindow.DockManager;
                        if (base.Children.Contains(releatedWindow) && releatedWindow.DockState != DockState.AutoHidden)
                        {
                            if (releatedWindow.CustomTabControl.Items.Count > 1)
                            {
                                if (releatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                {
                                    releatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                    if (cstabitem.OwnWindow.CustomTabControl != null && releatedWindow != cstabitem.OwnWindow)
                                    {
                                        if (!cstabitem.OwnWindow.CustomTabControl.Items.Contains(cstabitem))
                                        {
                                            cstabitem.OwnWindow.CustomTabControl.Items.Add(cstabitem);
                                        }
                                    }
                                }

                                if (releatedWindow == cstabitem.OwnWindow)
                                {
                                    _parentTabbedWindow = releatedWindow;
                                    TabbedWindow = cstabitem.OwnWindow;
                                    UpdateCustomTabItem();
                                }
                                else
                                {
                                    _parentTabbedWindow = releatedWindow;
                                    TabbedWindow = cstabitem.OwnWindow;
                                    RemovedTabItem = cstabitem;
                                    UpdateCustomTabItem();
                                    if (releatedWindow.CustomTabControl.SelectedItem != null)
                                    {
                                        releatedWindow.Caption = ((CustomTabItem)releatedWindow.CustomTabControl.SelectedItem).Header.ToString();
                                    }
                                    else if (releatedWindow.CustomTabControl.Items.Count == 1)
                                    {
                                        releatedWindow.Caption = ((CustomTabItem)releatedWindow.CustomTabControl.Items[0]).Header.ToString();
                                    }
                                    if (cstabitem.OwnWindow.CustomTabControl.SelectedItem != null)
                                    {
                                        cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.SelectedItem).Header.ToString();
                                    }
                                    else if (cstabitem.OwnWindow.CustomTabControl.Items.Count == 1)
                                    {
                                        cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.Items[0]).Header.ToString();
                                    }
                                }
                            }
                            else
                            {
                                releatedWindow.CommonMethodForHideClose();
                            }
                        }
                        else
                        {
                            //releatedWindow.CommonMethodForHideClose();

                            if (releatedWindow.DockState == DockState.AutoHidden)
                            {
                                releatedWindow.DockingManager.RecentlyMouseHoveredSidePanel = releatedWindow.DockingManager.GetsidePanel(releatedWindow);
                                releatedWindow.CommonMethodForHideClose();
                                RemoveSideGrid(releatedWindow);
                                if (tabNameCollection.Contains(cstabitem.OwnWindow._Caption))
                                {
                                    tabNameCollection.Remove(cstabitem.OwnWindow._Caption);
                                }
                            }
                            else
                            {
                                if (releatedWindow.CustomTabControl.Items.Count > 1)
                                {
                                    if (releatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                    {
                                        releatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                        if (cstabitem.OwnWindow.CustomTabControl != null && releatedWindow != cstabitem.OwnWindow)
                                        {
                                            if (!cstabitem.OwnWindow.CustomTabControl.Items.Contains(cstabitem))
                                            {
                                                cstabitem.OwnWindow.CustomTabControl.Items.Add(cstabitem);
                                            }
                                        }
                                    }

                                    if (releatedWindow == cstabitem.OwnWindow)
                                    {
                                        _parentTabbedWindow = releatedWindow;
                                        TabbedWindow = cstabitem.OwnWindow;
                                        RemovedTabItem = null;
                                        UpdateCustomTabItem();
                                    }
                                    else
                                    {
                                        _parentTabbedWindow = releatedWindow;
                                        TabbedWindow = cstabitem.OwnWindow;
                                        RemovedTabItem = cstabitem;
                                        UpdateCustomTabItem();
                                        if (releatedWindow.CustomTabControl.SelectedItem != null)
                                        {
                                            releatedWindow.Caption = ((CustomTabItem)releatedWindow.CustomTabControl.SelectedItem).Header.ToString();
                                        }
                                        else if (releatedWindow.CustomTabControl.Items.Count == 1)
                                        {
                                            releatedWindow.Caption = ((CustomTabItem)releatedWindow.CustomTabControl.Items[0]).Header.ToString();
                                        }
                                        if (cstabitem.OwnWindow.CustomTabControl.SelectedItem != null)
                                        {
                                            cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.SelectedItem).Header.ToString();
                                        }
                                        else if (cstabitem.OwnWindow.CustomTabControl.Items.Count == 1)
                                        {
                                            cstabitem.OwnWindow.Caption = ((CustomTabItem)cstabitem.OwnWindow.CustomTabControl.Items[0]).Header.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (cstab.Items.Contains(cstabitem))
                                    {
                                        cstab.SelectedItem = cstabitem;
                                    }
                                    releatedWindow.CommonMethodForHideClose();
                                }
                            }
                        }
                        if (dm != null && dm.gridDocking != null)
                        {
                            //cstabitem.OwnWindow.DockState = DockState.Dock;
                            dm.gridDocking.Remove(cstabitem.OwnWindow);
                        }
                        if (oldDockmanager != null && oldDockmanager.gridDocking != null)
                        {
                            oldDockmanager.gridDocking.Remove(cstabitem.OwnWindow);
                        }
                        DockingManager parent = cstabitem.OwnWindow.DockingManager;
                        Window window = cstabitem.OwnWindow;
                        if (oldDockmanager != null)
                        {
                            oldDockmanager.DetachPaneEvents(window);
                        }
                        if (dm != null)
                        {
                            dm.DetachPaneEvents(window);
                        }
                        if (dockmanager != null)
                        {
                            dockmanager.DetachPaneEvents(window);
                        }


                        parent.RemoveMemoryLeak(d, true);
                        window.DockManager = null;
                        window.OldValueDockManager = null;
                        parent = null;
                        window.DockingManager = null;
                        //cstabitem.OwnWindow.DeattachCaptionBarEvent();
                        //for (int i = 1,j=1; i < this.WindowCollection.Count(); i++,j++)
                        //{                           
                        //    if (!removed)
                        //    {
                        //        if (cstabitem.OwnWindow == this.WindowCollection[i])
                        //        {
                        //            this.WindowCollection.Remove(i);
                        //            removed = true;
                        //            this.WindowCollection.Add(i, this.WindowCollection[j + 1]);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        this.WindowCollection.Remove(i);
                        //        if (this.WindowCollection.ContainsKey(j + 1))
                        //        {
                        //            this.WindowCollection.Add(i, this.WindowCollection[j + 1]);
                        //        }
                        //    }                            
                        //}

                        //if (this.WindowCollection.ContainsKey(this.WindowCollection.Count()))
                        //{
                        //    Window w = this.WindowCollection[this.WindowCollection.Count()];                            
                        //    this.WindowCollection.Remove(this.WindowCollection.Count());
                        //}

                        cstabitem.Content = null;
                        cstabitem.Header = string.Empty;

                        //if (floatwithTab && window != null)
                        //{
                        //    SetboolValueWithSideInMode(window, Dock.Tabbed, DockState.Float);
                        //    DockingManager.SetSideInFloatMode(d, Dock.Tabbed);
                        //    SetboolValueWithTargetName(window, window.floatWindowTargetName, DockState.Float);
                        //    DockingManager.SetTargetNameInFloatingMode(d, window.floatWindowTargetName);
                        //}
                        //else if (dockwithTab && window != null)
                        //{
                        //    //cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                        //    SetboolValueWithSideInMode(window, Dock.Tabbed, DockState.Dock);
                        //    DockingManager.SetSideInDockedMode(d, Dock.Tabbed);
                        //    SetboolValueWithTargetName(window, window.floatWindowTargetName, DockState.Dock);
                        //    DockingManager.SetTargetNameInDockedMode(d, window.floatWindowTargetName);
                        //}

                    }
                }
            }
        }

        /// <summary>
        /// Removes the window.
        /// </summary>
        /// <param name="d">The d.</param>
        protected internal void RemoveWindow(UIElement d)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabItem cstabitem = ((FrameworkElement)d).Parent as CustomTabItem;
                    CustomTabControl cstab = cstabitem.Parent as CustomTabControl;
                    if (cstabitem.OwnWindow != null)
                    {

                        if (cstabitem.OwnWindow != null)
                        {
                            if (((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                            {
                                ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Remove(cstabitem.OwnWindow);
                                
                                if (cstab.RelatedWindow.CustomTabControl != null && cstabitem.OwnWindow != cstab.RelatedWindow)
                                {
                                    if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                    {
                                        cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                        if (cstab.RelatedWindow.CustomTabControl.SelectedItem != null)
                                        {
                                            cstab.RelatedWindow.Caption = (cstab.RelatedWindow.CustomTabControl.SelectedItem as CustomTabItem).Header.ToString();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (cstabitem.OwnWindow.DockManager != null)
                                {
                                    if (cstabitem.OwnWindow == cstab.RelatedWindow)
                                    {
                                        //cstabitem.OwnWindow.DockState = DockState.Hidden;
                                        if (cstabitem.OwnWindow.CustomTabControl != null)
                                        {
                                            foreach (CustomTabItem custab in cstabitem.OwnWindow.CustomTabControl.Items)
                                            {
                                                if (custab.OwnWindow.WindowChildElement == d)
                                                {
                                                    custab.OwnWindow.CustomTabControl.SelectedItem = custab;
                                                    break;
                                                }
                                            }
                                        }
                                        cstabitem.OwnWindow.CommonMethodForHideClose();
                                    }
                                    else
                                    {
                                        if (cstab.RelatedWindow.CustomTabControl != null)
                                        {
                                            if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                            {
                                                cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                            }
                                        }
                                    }
                                    if (((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                                    {
                                        ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Remove(cstabitem.OwnWindow);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [docking state property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDockingStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (((FrameworkElement)d).Parent != null)
                {
                    if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                    {
                        CustomTabItem cstabitem = ((FrameworkElement)d).Parent as CustomTabItem;
                        CustomTabControl cstab = cstabitem.Parent as CustomTabControl;
                        if (cstabitem.OwnWindow != null)
                        {
                            if (!cstabitem.OwnWindow.InternalllyRaisedDockStateChanged)
                            {
                                if ((DockState)e.NewValue == DockState.Dock)
                                {
                                    if (!cstabitem.OwnWindow.InternalllyRaisedDockStateChanged)
                                    {
                                        //cstabitem.OwnWindow.Visibility = Visibility.Visible;
                                        if ((DockState)e.OldValue == DockState.AutoHidden && cstab != null)
                                        {
                                            Window actualWindow = cstabitem.OwnWindow.DockingManager.GetWindow(cstab);
                                            if (actualWindow.dockToggle != null)
                                            {
                                                if (actualWindow.dockToggle.Visibility == Visibility.Visible)
                                                {
                                                    cstabitem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabitem.OwnWindow.DockingManager.GetsidePanel(actualWindow);
                                                    actualWindow.dockToggle.IsChecked = false;
                                                    actualWindow.Visibility = Visibility.Visible;
                                                }
                                            }
                                            else
                                            {
                                                cstabitem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabitem.OwnWindow.DockingManager.GetsidePanel(actualWindow);
                                                actualWindow.WindowUnPinned();
                                                actualWindow.Visibility = Visibility.Visible;
                                            }
                                        }
                                        else if ((DockState)e.OldValue == DockState.Float)
                                        {
                                            if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                            {
                                                cstabitem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                                cstabitem.OwnWindow.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                                cstabitem.OwnWindow.DockingManager.TabDoubleClick(cstabitem);
                                            }
                                            else
                                            {
                                                DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabitem.OwnWindow.WindowChildElement, DockState.Float, DockState.Dock,DockSide.Left);
                                                cstabitem.OwnWindow.DockingManager.FireDockStateChanging(args);
                                                if (!args.Cancel)
                                                {
                                                    cstabitem.OwnWindow._headerEventFired = true;
                                                    cstabitem.OwnWindow.StateTrans();
                                                    cstabitem.OwnWindow.DockingManager.FireDockStateChanged(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue);
                                                }
                                            }

                                        }
                                        else if ((DockState)e.OldValue == DockState.Hidden)
                                        {
                                            DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabitem.OwnWindow.WindowChildElement, DockState.Hidden, DockState.Dock,DockSide.Left);
                                            cstabitem.OwnWindow.DockingManager.FireDockStateChanging(args);
                                            if (!args.Cancel)
                                            {
                                                Window autoHideWindow = cstabitem.OwnWindow.DockingManager.GetExactParentWindowForDockedTabWindow(cstabitem.OwnWindow);//GetWindow(DockingManager.GetTargetNameInDockedMode(d));
                                                DockState dockState = autoHideWindow.DockState;
                                                if (DockingManager.GetSideInDockedMode(d) == Dock.Tabbed && autoHideWindow.DockState != DockState.Hidden)
                                                {
                                                    if (autoHideWindow.DockState == DockState.AutoHidden)
                                                    {
                                                        if (autoHideWindow.dockToggle != null)
                                                        {
                                                            if (autoHideWindow.dockToggle.Visibility == Visibility.Visible)
                                                            {
                                                                cstabitem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabitem.OwnWindow.DockingManager.GetsidePanel(autoHideWindow);
                                                                autoHideWindow.dockToggle.IsChecked = false;
                                                            }
                                                        }
                                                    }
                                                    if (!autoHideWindow.CustomTabControl.Items.Contains(cstabitem))
                                                    {
                                                        if (cstabitem.Parent is CustomTabControl)
                                                        {
                                                            if ((cstabitem.Parent as CustomTabControl).Items.Contains(cstabitem))
                                                            {
                                                                (cstabitem.Parent as CustomTabControl).Items.Remove(cstabitem);
                                                            }
                                                        }
                                                        autoHideWindow.CustomTabControl.Items.Add(cstabitem);
                                                        autoHideWindow.DockingManager.HideTabPanel(autoHideWindow);
                                                        if (((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                                                        {
                                                            if (cstabitem.OwnWindow.Visibility == Visibility.Visible)
                                                            {
                                                                cstabitem.OwnWindow.Visibility = Visibility.Collapsed;
                                                            }
                                                            //((Canvas)cstabitem.OwnWindow.DockingManager).Children.Remove(cstabitem.OwnWindow);
                                                        }
                                                    }
                                                    autoHideWindow.DockState = DockState.Dock;
                                                    if (dockState == DockState.AutoHidden)
                                                    {
                                                        autoHideWindow.dockToggle.IsChecked = true;
                                                    }

                                                }
                                                else
                                                {
                                                    cstabitem.OwnWindow.DockState = DockState.Float;
                                                    if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                                    {
                                                        cstabitem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                                        cstabitem.OwnWindow.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                                        cstabitem.OwnWindow.DockingManager.TabDoubleClick(cstabitem);
                                                    }
                                                    else
                                                    {
                                                        cstabitem.OwnWindow._headerEventFired = true;
                                                        cstabitem.OwnWindow.StateTrans();
                                                    }
                                                }

                                                cstabitem.OwnWindow.DockState = (DockState)e.NewValue;
                                                cstabitem.OwnWindow.DockingManager.FireDockStateChanged(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue);
                                            }

                                            cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                                        }
                                    }
                                    //cstabitem.OwnWindow.ChangingStateAfterLoaded();
                                }
                                else if ((DockState)e.NewValue == DockState.AutoHidden && (DockState)e.OldValue != DockState.Float)
                                {
                                    if (!cstabitem.OwnWindow.InternalllyRaisedDockStateChanged)
                                    {
                                        DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue,DockSide.Left);
                                        cstabitem.OwnWindow.DockingManager.FireDockStateChanging(args);
                                        if (!args.Cancel)
                                        {
                                            if ((DockState)e.OldValue == DockState.Hidden)
                                            {
                                                cstabitem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabitem.OwnWindow.DockingManager.GeneratedSidePanelAsWindowCaption(cstabitem.OwnWindow, cstabitem.OwnWindow);
                                                cstabitem.OwnWindow.DockState = DockState.Float;
                                                if (cstab != null)
                                                {
                                                    if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                                    {
                                                        cstabitem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                                        cstabitem.OwnWindow.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                                        cstabitem.OwnWindow.DockingManager.TabDoubleClick(cstabitem);
                                                    }
                                                    else
                                                    {
                                                        cstabitem.OwnWindow._headerEventFired = true;
                                                        cstabitem.OwnWindow.StateTrans();
                                                    }
                                                }
                                                else
                                                {
                                                    cstabitem.OwnWindow._headerEventFired = true;
                                                    cstabitem.OwnWindow.StateTrans();
                                                }
                                            }
                                            if (cstabitem.OwnWindow.CustomTabControl.Items.Count <= 0)
                                            {
                                                (cstabitem.Parent as CustomTabControl).RelatedWindow.DockState = DockState.AutoHidden;
                                                if ((cstabitem.Parent as CustomTabControl).RelatedWindow.dockToggle != null && (cstabitem.Parent as CustomTabControl).RelatedWindow.DockState == DockState.AutoHidden)
                                                {
                                                    (cstabitem.Parent as CustomTabControl).RelatedWindow.dockToggle.IsChecked = false;
                                                    (cstabitem.Parent as CustomTabControl).RelatedWindow.dockToggle.IsChecked = true;
                                                }
                                                else
                                                {
                                                    (cstabitem.Parent as CustomTabControl).RelatedWindow.DockState = DockState.Dock;
                                                    (cstabitem.Parent as CustomTabControl).RelatedWindow.DockState = DockState.AutoHidden;
                                                }
                                                if ((cstabitem.Parent as CustomTabControl).RelatedWindow.DockManager != null)
                                                {
                                                    if ((cstabitem.Parent as CustomTabControl).RelatedWindow.DockManager.gridDocking != null)
                                                    {
                                                        (cstabitem.Parent as CustomTabControl).RelatedWindow.DockManager.gridDocking.ArrangeLayout();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cstabitem.OwnWindow.DockState = DockState.AutoHidden;
                                                if (cstabitem.OwnWindow.DockManager != null)
                                                {
                                                    if (cstabitem.OwnWindow.DockManager.gridDocking != null)
                                                    {
                                                        cstabitem.OwnWindow.DockManager.gridDocking.ArrangeLayout();
                                                    }
                                                }
                                            }
                                            if ((cstabitem.OwnWindow.DockingManager as Canvas).Children.Contains(cstabitem.OwnWindow))
                                            {
                                                //(cstabitem.OwnWindow.DockingManager as Canvas).Children.Remove(cstabitem.OwnWindow);
                                                cstabitem.OwnWindow.Visibility = Visibility.Collapsed;
                                            }
                                            cstabitem.OwnWindow.DockingManager.UpdateSidePanelLayout();
                                            cstabitem.OwnWindow.DockingManager.FireDockStateChanged(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue);
                                        }
                                    }
                                }
                                else if ((DockState)e.NewValue == DockState.Float)
                                {
                                    if ((DockState)e.OldValue == DockState.Hidden)
                                    {
                                        if (!cstabitem.OwnWindow.InternalllyRaisedDockStateChanged)
                                        {
                                            //cstabitem.OwnWindow.StateMaintanceForWindowContainer();
                                            //cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                                            //DockingGrid dg = cstabitem.OwnWindow.DockingManager.GetParentDockManager();
                                            //if (dg != null)
                                            //{
                                            //    DockManager dm = dg._dockManager;
                                            //    if (((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                                            //    {
                                            //        ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Remove(cstabitem.OwnWindow);
                                            //    }
                                            //    Canvas.SetLeft(cstabitem.OwnWindow, cstabitem.OwnWindow.LeftPosition);
                                            //    Canvas.SetTop(cstabitem.OwnWindow, cstabitem.OwnWindow.TopPosition);
                                            //    Canvas.SetZIndex(cstabitem.OwnWindow, ++Window.currentZIndex);
                                            //    cstabitem.Height = (cstabitem.OwnWindow.FloatHeight > 0) ? cstabitem.OwnWindow.FloatHeight : 200;
                                            //    cstabitem.Width = (cstabitem.OwnWindow.FloatWidth > 0) ? cstabitem.OwnWindow.FloatWidth : 200;
                                            //    ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Add(cstabitem.OwnWindow);
                                            //}
                                            DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabitem.OwnWindow.WindowChildElement, DockState.Hidden, DockState.Float,DockSide.Left);
                                            cstabitem.OwnWindow.DockingManager.FireDockStateChanging(args);
                                            if(!args.Cancel)
                                            {
                                                cstabitem.OwnWindow.DockState = DockState.Dock;
                                                Dock dockSide = DockingManager.GetSideInDockedMode(cstabitem.OwnWindow.WindowChildElement);
                                                if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                                {
                                                    List<object> elementCollection = cstab.RelatedWindow.CustomTabControl.Items.ToList();
                                                    cstabitem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                                    cstabitem.OwnWindow.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                                    cstabitem.OwnWindow.DockingManager.TabDoubleClick(cstabitem);
                                                    foreach (CustomTabItem tabItem in elementCollection)
                                                    {
                                                        if (tabItem.OwnWindow != cstabitem.OwnWindow)
                                                        {
                                                            DockingManager.SetDockState(tabItem.OwnWindow.WindowChildElement, DockState.Hidden);
                                                            tabItem.OwnWindow.DockState = DockState.Hidden;
                                                            tabItem.OwnWindow.PreviousDockState = DockState.Float;
                                                            dockSide = DockingManager.GetSideInDockedMode(tabItem.OwnWindow.WindowChildElement);
                                                            if (!tabItem.OwnWindow.CustomTabControl.Items.Contains(tabItem))
                                                            {
                                                                if (cstabitem.OwnWindow.CustomTabControl.Items.Contains(tabItem))
                                                                {
                                                                    cstabitem.OwnWindow.CustomTabControl.Items.Remove(tabItem);
                                                                    tabItem.OwnWindow.CustomTabControl.Items.Add(tabItem);
                                                                    cstabitem.OwnWindow.DockingManager.HideTabPanel(tabItem.OwnWindow);
                                                                }
                                                            }
                                                        }
                                                        cstabitem.OwnWindow.Caption = cstabitem.Header.ToString();
                                                        cstabitem.OwnWindow.DockingManager.HideTabPanel(cstabitem.OwnWindow);
                                                    }
                                                }
                                                else
                                                {
                                                    cstabitem.OwnWindow._headerEventFired = true;
                                                    cstabitem.OwnWindow.StateTrans();
                                                }
                                                cstabitem.OwnWindow.DockingManager.FireDockStateChanged(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue);
                                            }
                                        }
                                    }
                                    else if ((DockState)e.OldValue == DockState.AutoHidden)
                                    {
                                        if (!cstabitem.OwnWindow.InternalllyRaisedDockStateChanged)
                                        {
                                            if (cstabitem.OwnWindow.dockToggle != null)
                                            {
                                                if (cstabitem.OwnWindow.dockToggle.Visibility == Visibility.Visible)
                                                {
                                                    cstabitem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabitem.OwnWindow.DockingManager.GetsidePanel(cstabitem.OwnWindow);
                                                    cstabitem.OwnWindow.dockToggle.IsChecked = false;
                                                }
                                            }
                                            if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                            {
                                                cstabitem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                                cstabitem.OwnWindow.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                                cstabitem.OwnWindow.DockingManager.TabDoubleClick(cstabitem);
                                            }
                                            else
                                            {
                                                cstabitem.OwnWindow._headerEventFired = true;
                                                cstabitem.OwnWindow.StateTrans();
                                            }
                                        }
                                        //cstabitem.OwnWindow.DockState = DockState.AutoHidden;
                                    }
                                    else if ((DockState)e.OldValue == DockState.Dock)
                                    {
                                        if (!cstabitem.OwnWindow.InternalllyRaisedDockStateChanged)
                                        {
                                            if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                            {
                                                DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabitem.OwnWindow.WindowChildElement, DockState.Dock, DockState.Float, DockSide.Left);
                                                cstabitem.OwnWindow.DockingManager.FireDockStateChanging(args);
                                                if (!args.Cancel)
                                                {
                                                    cstabitem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                                    cstabitem.OwnWindow.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                                    cstabitem.OwnWindow.DockingManager.TabDoubleClick(cstabitem);
                                                    cstabitem.OwnWindow.DockingManager.FireDockStateChanged(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue);
                                                }
                                            }
                                            else
                                            {
                                                DockStateChangingEventArgs args = new DockStateChangingEventArgs(cstabitem.OwnWindow.WindowChildElement, DockState.Dock, DockState.Float, DockSide.Left);
                                                cstabitem.OwnWindow.DockingManager.FireDockStateChanging(args);
                                                if (!args.Cancel)
                                                {
                                                    cstabitem.OwnWindow._headerEventFired = true;
                                                    cstabitem.OwnWindow.StateTrans();
                                                    cstabitem.OwnWindow.DockingManager.FireDockStateChanged(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue);
                                                }
                                            }
                                            cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                                            //if (cstab.RelatedWindow == cstabitem.OwnWindow)
                                            //{
                                            //    if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                            //    {
                                            //        if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                            //        {
                                            //           //cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                            //            if (cstabitem.OwnWindow.CustomTabControl != null && cstab.RelatedWindow != cstabitem.OwnWindow)
                                            //            {
                                            //                if (!cstabitem.OwnWindow.CustomTabControl.Items.Contains(cstabitem))
                                            //                {
                                            //                    cstabitem.OwnWindow.CustomTabControl.Items.Add(cstabitem);
                                            //                }
                                            //            }
                                            //        }
                                            //    }
                                            //    cstabitem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                            //    cstabitem.OwnWindow.DockingManager.TabbedWindow = cstabitem.OwnWindow;
                                            //    cstabitem.OwnWindow.DockingManager.UpdateCustomTabItem();
                                            //    cstabitem.OwnWindow.DockState = DockState.Float;
                                            //    cstabitem.OwnWindow.Visibility = Visibility.Visible;
                                            //    if (!((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                                            //    {
                                            //        ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Add(cstabitem.OwnWindow);
                                            //    }

                                            //}
                                            //cstabitem.OwnWindow.DockState = DockState.Float;                                            
                                        }
                                    }
                                }
                                else if ((DockState)e.NewValue == DockState.Hidden)
                                {
                                    if (!cstabitem.OwnWindow.InternalllyRaisedDockStateChanged)
                                    {
                                        if (cstab.RelatedWindow.CustomTabControl != null)
                                        {
                                            if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                            {
                                                cstab.RelatedWindow.CustomTabControl.SelectedItem = cstabitem;
                                            }
                                            cstabitem.OwnWindow.CommonMethodForHideClose();
                                        }
                                        //if (((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                                        //{
                                        //    ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Remove(cstabitem.OwnWindow);
                                        //    if (cstab.RelatedWindow.CustomTabControl != null && cstabitem.OwnWindow != cstab.RelatedWindow)
                                        //    {
                                        //        if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                        //        {
                                        //            cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                        //            if (cstab.RelatedWindow.CustomTabControl.SelectedItem != null)
                                        //            {
                                        //                cstab.RelatedWindow.Caption = (cstab.RelatedWindow.CustomTabControl.SelectedItem as CustomTabItem).Header.ToString();
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    if (cstabitem.OwnWindow.DockManager != null)
                                        //    {
                                        //        if (cstabitem.OwnWindow == cstab.RelatedWindow)
                                        //        {
                                        //            cstabitem.OwnWindow.CommonMethodForHideClose();
                                        //            //(cstabitem.OwnWindow.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                        //        }
                                        //        else
                                        //        {
                                        //            if (cstab.RelatedWindow.CustomTabControl != null)
                                        //            {
                                        //                if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabitem))
                                        //                {
                                        //                    cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabitem);
                                        //                }
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                        cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                                    }
                                }
                            }
                            cstabitem.OwnWindow.DockState = (DockState)e.NewValue;
                            cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                            //if ((DockState)e.NewValue == DockState.Dock)
                            //{
                            //    cstab.RelatedWindow.Width = double.NaN;
                            //    cstab.RelatedWindow.Height = double.NaN;
                            //}
                            //if (cstab.RelatedWindow.DockManager != null)
                            //{                            
                            //    (cstab.RelatedWindow.DockManager.Children[0] as DockingGrid).ArrangeLayout();
                            //}
                            //if (cstab.RelatedWindow.DockState == DockState.Hidden)
                            //{
                            //    if (cstab.RelatedWindow.DockingManager.Children.Contains(cstab.RelatedWindow))
                            //    {
                            //       // cstab.RelatedWindow.DockingManager.Children.Remove(cstab.RelatedWindow);
                            //    }
                            //}


                            //cstabitem.OwnWindow.DockingManager.FireDockStateChanged(cstabitem.OwnWindow.WindowChildElement, e.OldValue, e.NewValue);
                            if (cstabitem.OwnWindow.WindowChildElement != null)
                            {
                                cstabitem.OwnWindow.NoHeaderVisibility(DockingManager.GetNoHeader(cstabitem.OwnWindow.WindowChildElement), DockingManager.GetHeaderHeight(cstabitem.OwnWindow.WindowChildElement));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format("{0} parent is not null", d));
            }
        }

        /// <summary>
        /// Gets the can drag.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetCanDrag(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(CanDragProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="candrag">The needed boolean value.</param>
        public static void SetCanDrag(UIElement element, bool candrag)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CanDragProperty, candrag);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.RegisterAttached(
                "CanDrag",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(true, OnCanDragPropertyChanged));

        /// <summary>
        /// Called when [can drag property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanDragPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    if (cstab.RelatedWindow != null)
                    {
                        cstab.RelatedWindow.CanDrag = (bool)e.NewValue;
                    }
                }
            }

        }

        /// <summary>
        /// Gets the can float.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetCanFloat(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(CanFloatProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="canfloat">The needed boolean value.</param>
        public static void SetCanFloat(UIElement element, bool canfloat)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CanFloatProperty, canfloat);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty CanFloatProperty =
            DependencyProperty.RegisterAttached(
                "CanFloat",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(true, OnCanFloatPropertyChanged));

        /// <summary>
        /// Called when [can float property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanFloatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.CanFloat = (bool)e.NewValue;
                    if (cstab != null && cstab.Items.Count > 1)
                    {
                        ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.DockingManager.GetWindow(cstab).CanFloat = (bool)e.NewValue;
                    }
                }
            }

        }

        /// <summary>
        /// Gets the can close.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetCanClose(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(CanCloseProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="canclose">The needed Dock value.</param>
        public static void SetCanClose(UIElement element, bool canclose)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CanCloseProperty, canclose);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.RegisterAttached(
                "CanClose",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(true, OnCanClosePropertyChanged));

        /// <summary>
        /// Occurs when [can close property changed].
        /// </summary>
        public event PropertyChangedCallback CanClosePropertyChanged;


        /// <summary>
        /// Raises the <see cref="E:CanClosePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCanClosePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CanClosePropertyChanged != null)
            {
                CanClosePropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [can close property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanClosePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        { 
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.DockingManager.OnCanClosePropertyChanged(e);
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    if (cstab.RelatedWindow != null)
                    {
                        cstab.RelatedWindow.CanClose = (bool)e.NewValue;
                    }
                }
            }

        }













        /// <summary>
        /// Gets the can resize.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetCanResize(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(CanResizeProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="canclose">The needed Dock value.</param>
        public static void SetCanResize(UIElement element, bool canclose)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CanResizeProperty, canclose);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.RegisterAttached(
                "CanResize",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(true, OnCanResizePropertyChanged));

        /// <summary>
        /// Occurs when [can resize property changed].
        /// </summary>
        public event PropertyChangedCallback CanResizePropertyChanged;


        /// <summary>
        /// Raises the <see cref="E:CanResizePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCanResizePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CanResizePropertyChanged != null)
            {
                CanResizePropertyChanged(this, e);
            }
        }
        /// <summary>
        /// Called when [can resize property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanResizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.DockingManager.OnCanResizePropertyChanged(e);
                    if (((((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow as FrameworkElement).Parent is Grid))
                    {
                        Grid grid = ((((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow as FrameworkElement).Parent as FrameworkElement).Parent as Grid;
                        if (grid !=null && grid.Children.Count==3)
                        {
                            if (grid.Children[2] is CustomGridSplitter)
                            {
                                if (e.NewValue.Equals(false))
                                {
                                    (grid.Children[2] as GridSplitter).IsEnabled = false;
                                    if (grid.Parent is Grid)
                                    {
                                        Grid grd = grid.Parent as Grid;
                                        if (grd.Children.Count == 3)
                                        {
                                            (grd.Children[2] as GridSplitter).IsEnabled = false;
                                        }
                                    }
                                }
                                else
                                {
                                    (grid.Children[2] as GridSplitter).IsEnabled = true;
                                    if (grid.Parent is Grid)
                                    {
                                        Grid grd = grid.Parent as Grid;
                                        if (grd.Children.Count == 3)
                                        {
                                            for (int i = 0; i <= 1; i++)
                                            {
                                                if (grd.Children[i] is Grid && (grd.Children[i] as Grid).Children[0] is Window && DockingManager.GetCanResize(((grd.Children[i] as Grid).Children[0] as Window).WindowChildElement))
                                                {
                                                    (grd.Children[2] as GridSplitter).IsEnabled = true;
                                                }
                                            }
                                            if (grd.Children[0] is Grid && grd.Children[1] is Grid)
                                            {
                                                (grd.Children[2] as GridSplitter).IsEnabled = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the close button visibility for individual window
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetCloseButtonVisible(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CloseButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets the close button visibility for individual window
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetCloseButtonVisible(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool) element.GetValue(CloseButtonVisibleProperty);
        }

        /// <summary>
        /// CloseButtonVisible Property
        /// </summary>
        public static readonly DependencyProperty CloseButtonVisibleProperty =
            DependencyProperty.RegisterAttached("CloseButtonVisible", typeof (bool), typeof (DockingManager),
                                                new PropertyMetadata(true, OnCloseButtonVisibiltyChange));

        /// <summary>
        /// This method will be called when CloseButtonVisible Property is changed
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        private static void OnCloseButtonVisibiltyChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement) dependencyObject;
            if (element != null)
            {
                CustomTabItem customTabItem = element.Parent as CustomTabItem;
                if (customTabItem != null && customTabItem.OwnWindow != null && customTabItem.OwnWindow.closeButton != null)
                {
                    customTabItem.OwnWindow.closeButton.Visibility = (bool) args.NewValue
                                                                         ? Visibility.Visible
                                                                         : Visibility.Collapsed;
                }
            }

        }

        /// <summary>
        /// Sets the maximized state
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="value"></param>
        public static void SetMaximizedState(UIElement uiElement, MaximizedState value)
        {
            if (uiElement == null)
            {
                throw new ArgumentNullException("uiElement");
            }

            uiElement.SetValue(MaximizedStateProperty, value);
        }

        /// <summary>
        /// Gets the maximized state
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static MaximizedState GetMaximizedState(UIElement uiElement)
        {
            if (uiElement == null)
            {
                throw new ArgumentNullException("uiElement");
            }

            return (MaximizedState)uiElement.GetValue(MaximizedStateProperty);
        }

        /// <summary>
        /// Gets or Sets the maximized state for a window
        /// </summary>
        public static readonly DependencyProperty MaximizedStateProperty = DependencyProperty.RegisterAttached("MaximizedState", typeof(MaximizedState), typeof(DockingManager), new PropertyMetadata(MaximizedState.Restored, OnMaximizedStatePropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnMaximizedStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement)d;
            if (element != null)
            {
                CustomTabItem customTabItem = element.Parent as CustomTabItem;
                if (((MaximizedState)args.NewValue) == MaximizedState.Maximized)
                {
                    if (customTabItem != null && customTabItem.OwnWindow != null && !customTabItem.OwnWindow.InternallyRaisedMaximizedStateChanged)
                    {
                        if (customTabItem.OwnWindow.maximizeButton != null)
                            customTabItem.OwnWindow.maximizeButton.IsChecked = true;
                    }
                }
                else if (((MaximizedState)args.NewValue) == MaximizedState.Restored)
                {
                    if (customTabItem != null && customTabItem.OwnWindow != null && !customTabItem.OwnWindow.InternallyRaisedMaximizedStateChanged)
                    {
                        if(customTabItem.OwnWindow.maximizeButton != null)
                            customTabItem.OwnWindow.maximizeButton.IsChecked = false;
                    }
                }
            }           
        }

        /// <summary>
        /// Sets maximize button visibility for individual window
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="value"></param>
        public static void SetMaximizeButtonVisible(UIElement uiElement, bool value)
        {
            if (uiElement == null)
            {
                throw new ArgumentNullException("uiElement");
            }

            uiElement.SetValue(MaximizeButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets maximize button visibility for individual window
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static bool GetMaximizeButtonVisible(UIElement uiElement)
        {
            if (uiElement == null)
            {
                throw new ArgumentNullException("uiElement");
            }

            return (bool)uiElement.GetValue(MaximizeButtonVisibleProperty);
        }

        /// <summary>
        /// Gets or Sets MaximizeButtonVisibleProperty
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonVisibleProperty = DependencyProperty.RegisterAttached("MaximizeButtonVisible", typeof(bool), typeof(DockingManager), new PropertyMetadata(true, OnMaximizeButtonVisiblePropertyChanged));

        /// <summary>
        /// This method is called when MaximizeButtonVisible is changed
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        public static void OnMaximizeButtonVisiblePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;
            if (element != null)
            {
                CustomTabItem customTabItem = element.Parent as CustomTabItem;
                if (customTabItem != null && customTabItem.OwnWindow != null && customTabItem.OwnWindow.maximizeButton != null)
                {
                    customTabItem.OwnWindow.maximizeButton.Visibility = (bool)e.NewValue
                                                                         ? Visibility.Visible
                                                                         : Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Sets the menu button visibility for individual window
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetMenuButtonVisible(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(MenuButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets the menu button visibility for individual window
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetMenuButtonVisible(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(MenuButtonVisibleProperty);
        }

        /// <summary>
        /// MenuButtonVisible Property
        /// </summary>
        public static readonly DependencyProperty MenuButtonVisibleProperty =
            DependencyProperty.RegisterAttached("MenuButtonVisible", typeof(bool), typeof(DockingManager),
                                                new PropertyMetadata(true, OnMenuButtonVisibiltyChange));

        private static void OnMenuButtonVisibiltyChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;
            if (element != null)
            {
                CustomTabItem customTabItem = element.Parent as CustomTabItem;
                if (customTabItem != null && customTabItem.OwnWindow != null && customTabItem.OwnWindow.optionsButton!=null)
                {
                    customTabItem.OwnWindow.optionsButton.Visibility = (bool)args.NewValue
                                                                         ? Visibility.Visible
                                                                         : Visibility.Collapsed;
                }
            }

        }


        /// <summary>
        /// Sets the awl button visibility for individual window
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetAwlButtonVisible(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(AwlButtonVisibleProperty, value);
        }

        /// <summary>
        /// Gets the awl button visibility for individual window
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetAwlButtonVisible(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(AwlButtonVisibleProperty);
        }

        /// <summary>
        /// AwlButtonVisible Property
        /// </summary>
        public static readonly DependencyProperty AwlButtonVisibleProperty =
            DependencyProperty.RegisterAttached("AwlButtonVisible", typeof(bool), typeof(DockingManager),
                                                new PropertyMetadata(true, OnAwlButtonVisibiltyChange));

        private static void OnAwlButtonVisibiltyChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;
            if (element != null)
            {
                CustomTabItem customTabItem = element.Parent as CustomTabItem;
                if (customTabItem != null && customTabItem.OwnWindow != null && customTabItem.OwnWindow.dockToggle != null)
                {
                    customTabItem.OwnWindow.dockToggle.Visibility = (bool)args.NewValue
                                                                         ? Visibility.Visible
                                                                         : Visibility.Collapsed;
                }
            }

        }

        /// <summary>
        /// Gets the name of the window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string GetWindowName(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (string)element.GetValue(WindowNameProperty);
        }


        /// <summary>
        /// Sets the name of the window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="windowName">Name of the window.</param>
        public static void SetWindowName(UIElement element, string windowName)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(WindowNameProperty, windowName);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowNameProperty =
            DependencyProperty.RegisterAttached(
                "WindowName",
                typeof(string),
                typeof(DockingManager),
                new PropertyMetadata(string.Empty, OnWindowNamePropertyChanged));



        /// <summary>
        /// Called when [window name property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }









        /// <summary>
        /// Gets the can auto hide.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetCanAutoHide(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(CanAutoHideProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="canautohide">The needed bool value.</param>
        public static void SetCanAutoHide(UIElement element, bool canautohide)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CanAutoHideProperty, canautohide);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty CanAutoHideProperty =
            DependencyProperty.RegisterAttached(
                "CanAutoHide",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(true, OnCanAutoHidePropertyChanged));

        /// <summary>
        /// Called when [can auto hide property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanAutoHidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    if (cstab.RelatedWindow != null)
                    {
                        cstab.RelatedWindow.CanAutoHide = (bool)e.NewValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the can dock.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetCanDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(CanDockProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="candock">The needed boolen value.</param>
        public static void SetCanDock(UIElement element, bool candock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CanDockProperty, candock);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty CanDockProperty =
            DependencyProperty.RegisterAttached(
                "CanDock",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(true, OnCanDockPropertyChanged));

        /// <summary>
        /// Called when [can dock property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCanDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl; 
                    ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.CanDock = (bool)e.NewValue; 
                    if (cstab != null && cstab.Items.Count > 1)
                    {
                        ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.DockingManager.GetWindow(cstab).CanDock = (bool)e.NewValue;
                    }
                }
            }

        }

        /// <summary>
        /// Gets the dock.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static Dock GetDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (Dock)element.GetValue(DockProperty);
        }

        /// <summary>
        /// Sets the value of the Dock attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="dock">The needed Dock value.</param>
        public static void SetDock(UIElement element, Dock dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(DockProperty, dock);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.RegisterAttached(
                "Dock",
                typeof(Dock),
                typeof(DockingManager),
                new PropertyMetadata(Dock.Left, OnDockPropertyChanged));

        /// <summary>
        /// DockProperty property changed handler.
        /// </summary>
        /// <param name="d">UIElement that changed its Dock.</param>
        /// <param name="e">Event arguments.</param>

        private static void OnDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string GetHeader(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (string)element.GetValue(HeaderProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="header">The needed Header value.</param>
        public static void SetHeader(UIElement element, string header)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(HeaderProperty, header);
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached(
                "Header",
                typeof(string),
                typeof(DockingManager),
                new PropertyMetadata(String.Empty, OnHeaderPropertyChanged));

        /// <summary>
        /// Called when [header property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabItem tabItem = ((CustomTabItem)((FrameworkElement)d).Parent);
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    if(cstab.SelectedItem == tabItem)
                        cstab.RelatedWindow.Caption = e.NewValue.ToString();
                    if (tabItem.OwnWindow != null && cstab.RelatedWindow != tabItem.OwnWindow)
                    {
                        tabItem.OwnWindow.Caption = e.NewValue.ToString();
                    }
                    tabItem.Header = e.NewValue.ToString();
                    if(tabItem.dc != null)
                        tabItem.dc.Text = e.NewValue.ToString();
                }
            }
            ////    //Window w = new Window();
            ////    ////w.contentpresenter.Children.Add((UIElement)d);
            ////    //w.Caption = e.NewValue.ToString();
            ////    //w.WindowChildElement = (UIElement)d;                
            ////    //w.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            ////    //w.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;                               
            ////    //wm.ShowWindow(w, new Point(0, 0));
            ////    //if (!(StaticWindowCollection.ContainsKey(tempKey)))
            ////    //    StaticWindowCollection.Add(tempKey++, w);                
            ////}
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ImageBrush GetIcon(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (ImageBrush)element.GetValue(IconProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="icon">The needed ImageBrush value.</param>
        public static void SetIcon(UIElement element, ImageBrush icon)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(IconProperty, icon);
        }

        /// <summary>
        /// Identifies the Dependency Property Icon.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(ImageBrush), typeof(DockingManager), new PropertyMetadata(null, OnIconPropertyChanged));

        /// <summary>
        /// Identifies DockingManager.SideInDockedMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the element DockSide in docked mode.
        /// DockSide means how element is located regards its target and is used for layout.
        /// For all possible cases see <see cref="DockSide"/> enum. The default value is DockSide.Left.
        /// </remarks>
        public static readonly DependencyProperty SideInDockedModeProperty =
            DependencyProperty.RegisterAttached("SideInDockedMode", typeof(Dock), typeof(DockingManager), new PropertyMetadata(Dock.Left, new PropertyChangedCallback(OnSideInDockedModeChanged)));

        /// <summary>
        /// Identifies DockingManager.SideInFloatMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the element DockSide in float mode.
        /// DockSide means how element is located regards its target and is used for layout.
        /// For all possible cases see <see cref="DockSide"/> enum. The default value is DockSide.Left.
        /// </remarks>
        public static readonly DependencyProperty SideInFloatModeProperty =
            DependencyProperty.RegisterAttached("SideInFloatMode", typeof(Dock), typeof(DockingManager), new PropertyMetadata(Dock.Left, new PropertyChangedCallback(OnSideInFloatModeChanged)));

        /// <summary>
        /// Calls OnSideInDockedModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSideInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager.OnSideModeChanged(d, (Dock)e.NewValue, (Dock)e.OldValue, DockState.Dock);
        }

        /// <summary>
        /// Calls OnSideInFloatModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSideInFloatModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager.OnSideModeChanged(d, (Dock)e.NewValue, (Dock)e.OldValue, DockState.Float);
        }

        /// <summary>
        /// Gets the value of the DockingManager.TargetNameInFloatingMode attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.TargetNameInFloatingMode attached property.</returns>
        public static string GetTargetNameInFloatingMode(DependencyObject obj)
        {
            return (string)obj.GetValue(TargetNameInFloatingModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.TargetNameInFloatingMode attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.TargetNameInFloatingMode attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTargetNameInFloatingMode(DependencyObject obj, string value)
        {
            obj.SetValue(TargetNameInFloatingModeProperty, value);
        }

        /// <summary>
        /// Gets the value of the DockingManager.TargetNameInDockedMode attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.TargetNameInDockedMode attached property.</returns>
        public static string GetTargetNameInDockedMode(DependencyObject obj)
        {
            return (string)obj.GetValue(TargetNameInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.TargetNameInDockedMode attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.TargetNameInDockedMode attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetTargetNameInDockedMode(DependencyObject obj, string value)
        {
            obj.SetValue(TargetNameInDockedModeProperty, value);
        }

        /// <summary>
        /// Identifies DockingManager.TargetNameInFloatingMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the name of the element's target in float mode.
        /// If name is an empty string it means child is connected directly to a docking manager and it is displayed as a float window, 
        /// otherwise it's connected to its target which is also one of docking manager child with the specified name.
        /// The default value is empty string.
        /// </remarks>
        public static readonly DependencyProperty TargetNameInFloatingModeProperty =
            DependencyProperty.RegisterAttached("TargetNameInFloatingMode", typeof(string), typeof(DockingManager), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTargetNameInFloatingModeChanged)));

        /// <summary>
        /// Identifies DockingManager.TargetNameInDockedMode attached property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the name of the element's target in docked mode.
        /// If name is an empty string it means child is connected directly to a docking manager main host, otherwise it's connected to its target 
        /// which is also one of docking manager child with the specified name.
        /// The default value is empty string.
        /// </remarks>
        public static readonly DependencyProperty TargetNameInDockedModeProperty =
            DependencyProperty.RegisterAttached("TargetNameInDockedMode", typeof(string), typeof(DockingManager), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTargetNameInDockedModeChanged)));

        /// <summary>
        /// Calls OnTargetNameInFloatingModeChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTargetNameInFloatingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabItem cstabitem = ((FrameworkElement)d).Parent as CustomTabItem;
                    CustomTabControl cstab = cstabitem.Parent as CustomTabControl;
                    if (cstabitem.OwnWindow != null)
                    {
                        cstabitem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                    }
                }
            }
            ////FrameworkElement element = (FrameworkElement)d;
            ////element.CoerceValue(DockingManager.SideInFloatModeProperty);
            ////DockingManager manager = ResolveManager(element);
            ////string newName = e.NewValue.ToString();
            ////if (manager != null)
            ////{
            ////    string oldName = e.OldValue.ToString();
            ////    manager.RaiseTargetNameInFloatingModeChangedEvent(element, oldName, newName);
            ////}
            ////DockState state = GetState(element);
            ////if (state == DockState.Float && string.IsNullOrEmpty(newName) &&
            ////    !manager.LockPropertyChangedAction)
            ////{
            ////    InitFloatingWindowRect(element, DockState.Float);
            ////}
            ////UpdateLayout(element);
        }

        ///// <summary>
        ///// Gets the value of the DockingManager.State attached property from a given DependencyObject.
        ///// </summary>
        ///// <param name="obj">The element from which to read the property value.</param>
        ///// <returns>The value of the DockingManager.State attached property.</returns>


        ///// <summary>
        ///// Sets the value of the DockingManager.State attached property to a given DependencyObject.
        ///// </summary>
        ///// <param name="obj">The element on which to set the DockingManager.State attached property.</param>
        ///// <param name="value">The property value to set.</param>
        ///// <remarks>
        ///// See <see cref="DockState"/> for all possiible DockState cases.
        ///// </remarks>


        /// <summary>
        /// Identifies DockingManager.State dependency property.
        /// </summary>
        /// <remarks>
        /// This property can be attached to a docking manager child and is used to get / set the element DockState.
        /// For all possible cases see <see cref="DockState"/> enum. The default value is DockState.Dock.
        /// </remarks>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State", typeof(DockState), typeof(DockingManager), new PropertyMetadata(DockState.Dock, OnStatePropertyChanged));

        /// <summary>
        /// Identifies DockingManager.DesiredWidthInDockedMode dependency property.
        /// </summary>
        public static readonly DependencyProperty DesiredWidthInDockedModeProperty =
           DependencyProperty.RegisterAttached("DesiredWidthInDockedMode", typeof(double), typeof(DockingManager), new PropertyMetadata(80d, OnDesiredWidthInDockedModePropertyChanged));

        /// <summary>
        /// Called when [desired width in docked mode property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredWidthInDockedModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    if (cstab.RelatedWindow != null)
                    {
                        cstab.RelatedWindow.PaneWidth = (double)e.NewValue;
                        if (cstab.RelatedWindow.DockManager != null)
                        {
                            // (cstab.RelatedWindow.DockManager.Children[0] as DockingGrid).ArrangeLayout();

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the desired width in docked mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetDesiredWidthInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredWidthInDockedModeProperty);
        }

        /// <summary>
        /// Sets the desired width in docked mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDesiredWidthInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredWidthInDockedModeProperty, value);
        }

        /// <summary>
        /// Represents the DesiredHeightInDocked ModeProperty.
        /// </summary>
        public static readonly DependencyProperty DesiredHeightInDockedModeProperty =
            DependencyProperty.RegisterAttached("DesiredHeightInDockedMode", typeof(double), typeof(DockingManager), new PropertyMetadata(80d, OnDesiredHeightInDockedModePropertyChanged));

        /// <summary>
        /// Called when [desired height in docked mode property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDesiredHeightInDockedModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    if (cstab.RelatedWindow != null)
                    {
                        cstab.RelatedWindow.PaneHeight = (double)e.NewValue;
                        if (cstab.RelatedWindow.DockManager != null)
                        {
                            //(cstab.RelatedWindow.DockManager.Children[0] as DockingGrid).ArrangeLayout();

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the desired height in docked mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetDesiredHeightInDockedMode(DependencyObject obj)
        {
            return (double)obj.GetValue(DesiredHeightInDockedModeProperty);
        }

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public Window GetWindow(UIElement element)
        {
            Window eleWindow = null;
            foreach (Window window in WindowCollection.Values)
            {
                if (window.WindowChildElement.Equals(element))
                {
                    eleWindow = window;
                    break;
                }
            }
            return eleWindow;
        }

        /// <summary>
        /// Indicates whether two elements are in same tab group.
        /// </summary>
        /// <param name="element1"></param>
        /// <param name="element2"></param>
        /// <returns></returns>
        public bool IsInSameTabbedGroup(UIElement element1, UIElement element2)
        {
            if ((element1 as FrameworkElement) != null && (element2 as FrameworkElement) != null)
            {
                Window window1 = GetWindow(element1);
                Window window2 = GetWindow(element2);
                CustomTabItem custab1 = (element1 as FrameworkElement).Parent as CustomTabItem;
                CustomTabItem custab2 = (element2 as FrameworkElement).Parent as CustomTabItem;
                if (custab2 != null && window1 != null && window1.CustomTabControl.Items.Count > 1 && window1.CustomTabControl.Items.Contains(custab2))
                {
                    return true;
                }

                if (custab1 != null && window2 != null && window2.CustomTabControl.Items.Count > 1 && window2.CustomTabControl.Items.Contains(custab1))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the tab control
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public CustomTabControl GetTabControl(UIElement element)
        {
            Window window = GetWindow(element);
            return window.CustomTabControl;
        }
        /// <summary>
        /// Sets the desired height in docked mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDesiredHeightInDockedMode(DependencyObject obj, double value)
        {
            obj.SetValue(DesiredHeightInDockedModeProperty, value);
        }

        /// <summary>
        /// Called when [state property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            ////try
            ////{
            ////    d.CoerceValue(CanDockProperty);
            ////    d.CoerceValue(CanFloatProperty);
            ////    d.CoerceValue(CanCloseProperty);
            ////    d.CoerceValue(CanAutoHideProperty);
            ////    DockState newState = (DockState)e.NewValue;
            ////    DockState oldState = (DockState)e.OldValue;
            ////    DockingManager.SetPreviousState(element, oldState);
            ////    DockingManager.ValidateFloatingWindowRect(element, newState);
            ////    DockingManager owner = DockingManager.ResolveManager(element);
            ////    if (null != owner)
            ////    {
            ////        if (!owner.LockPropertyChangedAction)
            ////        {
            ////            DockingManager.ChangeState(element, newState);
            ////        }
            ////        owner.OnStatePropertyChanged(element, e);
            ////        if (newState == DockState.Hidden)
            ////        {
            ////            owner.OnElementHidden(element);
            ////        }
            ////        else if (oldState == DockState.Hidden)
            ////        {
            ////            owner.OnElementShown(element);
            ////        }
            ////        if (owner.DockFill && (newState == DockState.Document || oldState == DockState.Document))
            ////        {
            ////            owner.ActivateDockFill();
            ////        }
            ////        if (owner.DockFill && (newState == DockState.Dock || oldState == DockState.Dock))
            ////        {
            ////            owner.CheckDockFillProperty();
            ////        }
            ////    }
            ////    UpdateLayout(element);
            ////}
            ////finally
            ////{
            ////    RoutedEventArgs args = new RoutedEventArgs(StateChangedEvent, element);
            ////    element.RaiseEvent(args);
            ////}
        }

        /// <summary>
        /// Calls OnTargetNameInDockedModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnTargetNameInDockedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    CustomTabItem cstabItem = ((FrameworkElement)d).Parent as CustomTabItem;
                    CustomTabControl cstab = cstabItem.Parent as CustomTabControl;
                    bool autoHiddenPresent = false;
                    if (cstabItem.OwnWindow != null)
                    {
                        if (!cstabItem.OwnWindow.InternalllyRaisedDockStateChanged)
                        {
                            DockingGrid dockingGrid = cstabItem.OwnWindow.DockingManager.GetParentDockManager();
                            DockManager dm = null;
                            if (dockingGrid != null)
                            {
                                dm = dockingGrid._dockManager;
                            }
                            Window w = cstabItem.OwnWindow.DockingManager.GetWindow(e.NewValue.ToString());

                            if (w != null)
                            {
                                #region old Data
                                //if (cstabitem.OwnWindow.DockState == DockState.Float)
                                //{
                                //    if (((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow) && cstabitem.Visibility == Visibility.Visible && cstabitem.OwnWindow.CustomTabControl.Items.Count > 0)
                                //    {
                                //        double height = cstabitem.OwnWindow.Height;
                                //        double width = cstabitem.OwnWindow.Width;
                                //        double left = Canvas.GetLeft(cstabitem.OwnWindow);
                                //        double top = Canvas.GetTop(cstabitem.OwnWindow);
                                //        dm.gridDocking.MoveTo(cstabitem.OwnWindow, w, cstabitem.OwnWindow.DockPosition);
                                //        cstabitem.OwnWindow.Height = height;
                                //        cstabitem.OwnWindow.Width = width;
                                //        Canvas.SetLeft(cstabitem.OwnWindow,left);
                                //        Canvas.SetTop(cstabitem.OwnWindow,top);
                                //        if (((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                                //        {
                                //            ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Remove(cstabitem.OwnWindow);
                                //        }
                                //        if (!((Canvas)cstabitem.OwnWindow.DockingManager).Children.Contains(cstabitem.OwnWindow))
                                //        {
                                //            ((Canvas)cstabitem.OwnWindow.DockingManager).Children.Add(cstabitem.OwnWindow);
                                //        }
                                //    }
                                //    else if(cstabitem.OwnWindow.DockManager.Parent is WindowContainer)
                                //    {
                                //        DockManager swap = cstabitem.OwnWindow.DockManager;
                                //        dm.gridDocking.MoveTo(cstabitem.OwnWindow, w, cstabitem.OwnWindow.DockPosition);
                                //    }
                                //}
                                //else if (cstabitem.OwnWindow.DockState == DockState.AutoHidden)
                                //{
                                //}
                                //else
                                //{
                                //    dm.gridDocking.MoveTo(cstabitem.OwnWindow, w, cstabitem.OwnWindow.DockPosition);
                                //}
                                #endregion
                                DockState relatedWindowDockState = cstabItem.OwnWindow.DockState;
                                if (cstabItem.OwnWindow.DockManager.Parent is WindowContainer && cstabItem.OwnWindow.CustomTabControl.Items.Count > 0)
                                {
                                    cstabItem.OwnWindow.StateTrans();
                                }
                                if (cstabItem.OwnWindow.DockState == DockState.AutoHidden)
                                {
                                    if (cstabItem.OwnWindow.dockToggle != null)
                                    {
                                        if (cstabItem.OwnWindow.dockToggle.Visibility == Visibility.Visible)
                                        {
                                            cstabItem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabItem.OwnWindow.DockingManager.GetsidePanel(cstabItem.OwnWindow);
                                            cstabItem.OwnWindow.dockToggle.IsChecked = false;
                                            autoHiddenPresent = true;
                                        }
                                    }
                                }

                                if (DockingManager.GetSideInDockedMode(d) != Dock.Tabbed)
                                {
                                    if (cstab.RelatedWindow == cstabItem.OwnWindow)
                                    {
                                        if (cstab.RelatedWindow.CustomTabControl.Items.Count > 1)
                                        {
                                            if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabItem))
                                            {

                                                cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabItem);
                                                if (cstabItem.OwnWindow.CustomTabControl != null && cstab.RelatedWindow != cstabItem.OwnWindow)
                                                {
                                                    if (!cstabItem.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                                    {
                                                        cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                    }
                                                }
                                            }
                                            cstabItem.OwnWindow.DockingManager._parentTabbedWindow = cstab.RelatedWindow;
                                            cstabItem.OwnWindow.DockingManager.TabbedWindow = cstabItem.OwnWindow;
                                            cstabItem.OwnWindow.DockingManager.UpdateCustomTabItem();
                                        }

                                    }
                                    else
                                    {
                                        cstabItem.OwnWindow.DockManager.gridDocking.Remove(cstabItem.OwnWindow);
                                    }

                                }
                                else
                                {
                                    if (cstab.RelatedWindow != cstabItem.OwnWindow)
                                    {
                                        if (cstab.RelatedWindow.CustomTabControl.Items.Contains(cstabItem))
                                        {
                                            cstab.RelatedWindow.CustomTabControl.Items.Remove(cstabItem);
                                            cstab.RelatedWindow.DockingManager.HideTabPanel(cstab.RelatedWindow);
                                            if (!cstabItem.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                            {
                                                cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                cstabItem.OwnWindow.DockingManager.HideTabPanel(cstabItem.OwnWindow);
                                                cstabItem.OwnWindow.Visibility = Visibility.Visible;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cstabItem.OwnWindow.CommonMethodForHideClose();
                                    }
                                    Window hoveredWindow = cstabItem.OwnWindow.DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(d));
                                    if (hoveredWindow != null)
                                    {
                                        cstabItem.OwnWindow.DockingManager.mouseHoveredWindow = cstabItem.OwnWindow.DockingManager.GetExactParentWindowForDockedTabWindow(hoveredWindow);

                                        cstabItem.OwnWindow.DockingManager._tarGetWindow = cstabItem.OwnWindow;
                                        cstabItem.OwnWindow.DockingManager.HostingElementByCenterDragProvider(DockState.Dock, new Point());
                                        cstabItem.OwnWindow.DockingManager.ApplyDefaultBackground(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                        cstabItem.OwnWindow.DockingManager.HideTabPanel(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                    }
                                }
                                if (DockingManager.GetSideInDockedMode(d) != Dock.Tabbed)
                                {
                                    cstabItem.OwnWindow.DockManager = cstabItem.OwnWindow.DockingManager.GetParentDockManager()._dockManager;
                                    Window hoverWindow = cstabItem.OwnWindow.DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(d));
                                    if (hoverWindow != null)
                                    {
                                        cstabItem.OwnWindow.DockingManager.mouseHoveredWindow = cstabItem.OwnWindow.DockingManager.GetExactParentWindowForDockedTabWindow(hoverWindow);

                                        cstabItem.OwnWindow.DockingManager._tarGetWindow = cstabItem.OwnWindow;
                                        if (cstabItem.OwnWindow.DockingManager.mouseHoveredWindow != null)
                                        {
                                            if (cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.DockState == DockState.Dock || cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.DockState == DockState.AutoHidden || cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.DockState == DockState.Hidden)
                                            {
                                                UIElement parent = cstabItem.OwnWindow.DockingManager.GetDockingGrid((UIElement)cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                                DockState ds = DockState.Dock;

                                                ds = cstabItem.OwnWindow.DockingManager.GetParentWindowContainer(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                                if (parent != null)
                                                {
                                                    parent = (UIElement)VisualTreeHelper.GetParent((UIElement)parent);
                                                }
                                                cstabItem.OwnWindow.DockingManager.ApplyDefaultBackground(cstabItem.OwnWindow.DockingManager._tarGetWindow);
                                                cstabItem.OwnWindow.DockingManager.ApplyDefaultBackground(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                                cstabItem.OwnWindow.DockingManager.DockingSingleWindowFill(DockingManager.GetSideInDockedMode(cstabItem.OwnWindow), cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.ActualWidth, cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.ActualHeight);
                                                ;
                                                //cstabItem.OwnWindow.DockingManager.DockingFill(parent, ds, newValue);
                                                if (relatedWindowDockState == DockState.Float)
                                                {
                                                    cstabItem.OwnWindow.StateTrans();
                                                }
                                            }
                                            else if (cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.DockState == DockState.Float)
                                            {
                                                cstabItem.OwnWindow.DockingManager.UpdateTargetNameForMoveToDockWindow(cstabItem.OwnWindow.DockingManager._tarGetWindow, cstabItem.OwnWindow.DockingManager.mouseHoveredWindow, DockingManager.GetSideInDockedMode(cstabItem.OwnWindow));
                                                if (cstabItem.OwnWindow.DockingManager._tarGetWindow.DockManager == dm && cstabItem.OwnWindow.DockingManager._tarGetWindow.DockManager.Parent is DockingManager)
                                                {
                                                    cstabItem.OwnWindow.DockingManager._tarGetWindow.DockManager = dm;
                                                }
                                                cstabItem.OwnWindow.DockingManager._tarGetWindow.MoveTo(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow, DockingManager.GetSideInDockedMode(cstabItem.OwnWindow));
                                                if (relatedWindowDockState == DockState.Float)
                                                {
                                                    cstabItem.OwnWindow.StateTrans();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cstabItem.OwnWindow.DockingManager.SetDockByPopUp(cstabItem.OwnWindow, DockingManager.GetSideInDockedMode(cstabItem.OwnWindow));
                                    }
                                }
                                if (autoHiddenPresent)
                                {
                                    cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                    cstabItem.OwnWindow.dockToggle.IsChecked = true;
                                    cstabItem.OwnWindow.DockState = DockState.AutoHidden;
                                }
                            }
                            cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                        }
                        cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                    }
                }
            }
            ////UIElement element = (UIElement)d;
            ////if (!(StaticTarGetNameCollection.Contains(((string)e.NewValue))))
            ////    StaticTarGetNameCollection.Add(((string)e.NewValue));
            ////for (int i = 1; i <= StaticWindowCollection.Count; i++)
            ////{
            ////    Window _window = StaticWindowCollection[i];
            ////    if (_window.WindowChildElement.GetType() == element.GetType())
            ////    {
            ////        _window.WindowTargetNameInDockedMode = (string)e.NewValue;
            ////        break;
            ////    }
            ////}            
            ////element.CoerceValue(DockingManager.SideInDockedModeProperty);
            ////DockingManager manager = ResolveManager(element);
            ////if (manager != null)
            ////{
            ////    manager.RaiseTargetNameInDockedModeChangedEvent(element, e.OldValue.ToString(), e.NewValue.ToString());
            ////}
            ////UpdateLayout(element);
        }

        /// <summary>
        /// Called when [side mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="state">The state.</param>
        private static void OnSideModeChanged(DependencyObject d, Dock newValue, Dock oldValue, DockState state)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    #region
                    CustomTabItem cstabItem = ((FrameworkElement)d).Parent as CustomTabItem;
                    CustomTabControl cstab = ((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl;
                    Window releatedWindow = null;
                    if (cstab != null)
                    {
                        releatedWindow = cstabItem.OwnWindow.DockingManager.GetWindow(cstab);
                        if (releatedWindow == null)
                        {
                            releatedWindow = cstab.RelatedWindow;
                        }
                    }
                    
                    bool autoHiddenPresent = false;
                    if (cstabItem != null)
                    {
                        if (cstabItem.OwnWindow != null && cstabItem.OwnWindow.DockState != DockState.Float && !cstabItem.OwnWindow.InternalllyRaisedDockStateChanged && state == DockState.Dock)
                        {
                            if (newValue == Dock.Tabbed)
                            {
                                Window hoverWindow = cstabItem.OwnWindow.DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(d));
                                if (oldValue != Dock.Tabbed && hoverWindow != null)
                                {
                                    bool isParentDockingManager = false;
                                    bool parentIsGridElement = false;
                                    bool isWindowContainerPresent = false;
                                    cstabItem.OwnWindow.WindowMoveFromDockLayout(ref isParentDockingManager, ref parentIsGridElement, ref isWindowContainerPresent);
                                    //cstabItem.OwnWindow.DockManager.gridDocking.Remove(cstabItem.OwnWindow);
                                    cstabItem.OwnWindow.DockPosition = newValue;
                                    cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                    DockingManager.SetSideInDockedMode(d, oldValue);
                                   
                                    cstabItem.OwnWindow.DockingManager.mouseHoveredWindow = cstabItem.OwnWindow.DockingManager.GetExactParentWindowForDockedTabWindow(hoverWindow);

                                    cstabItem.OwnWindow.DockingManager._tarGetWindow = cstabItem.OwnWindow;
                                    cstabItem.OwnWindow.DockingManager.HostingElementByCenterDragProvider(hoverWindow.DockState, new Point(0,0));
                                }
                                else if (hoverWindow == null)
                                {
                                    cstabItem.OwnWindow.DockingManager.SetboolValueWithSideInMode(cstabItem.OwnWindow, oldValue, DockState.Dock);
                                    //cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                    DockingManager.SetSideInDockedMode(d, oldValue);
                                }
                            }
                            else
                            {
                                if (cstabItem.OwnWindow.DockState == DockState.AutoHidden)
                                {
                                    if (cstabItem.OwnWindow.dockToggle != null)
                                    {
                                        if (cstabItem.OwnWindow.dockToggle.Visibility == Visibility.Visible)
                                        {
                                            cstabItem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabItem.OwnWindow.DockingManager.GetsidePanel(cstabItem.OwnWindow);
                                            cstabItem.OwnWindow.dockToggle.IsChecked = false;
                                            autoHiddenPresent = true;
                                        }
                                    }
                                }

                                if (oldValue != Dock.Tabbed)
                                {
                                    if (releatedWindow == cstabItem.OwnWindow)
                                    {
                                        if (releatedWindow.CustomTabControl.Items.Count > 1)
                                        {
                                            if (releatedWindow.CustomTabControl.Items.Contains(cstabItem))
                                            {

                                                releatedWindow.CustomTabControl.Items.Remove(cstabItem);
                                                if (cstabItem.OwnWindow.CustomTabControl != null && releatedWindow != cstabItem.OwnWindow)
                                                {
                                                    if (!cstabItem.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                                    {
                                                        cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                    }
                                                }
                                            }
                                            cstabItem.OwnWindow.DockingManager._parentTabbedWindow = releatedWindow;
                                            cstabItem.OwnWindow.DockingManager.TabbedWindow = cstabItem.OwnWindow;
                                            cstabItem.OwnWindow.DockingManager.UpdateCustomTabItem();
                                            if (!cstabItem.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                            {
                                                cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        cstabItem.OwnWindow.DockManager.gridDocking.Remove(cstabItem.OwnWindow);
                                    }

                                }
                                else
                                {
                                    if (releatedWindow != cstabItem.OwnWindow)
                                    {
                                        if (releatedWindow.CustomTabControl.Items.Contains(cstabItem))
                                        {
                                            releatedWindow.CustomTabControl.Items.Remove(cstabItem);
                                            releatedWindow.DockingManager.HideTabPanel(releatedWindow);
                                            if (!cstabItem.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                            {
                                                cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                cstabItem.OwnWindow.DockingManager.HideTabPanel(cstabItem.OwnWindow);
                                                cstabItem.OwnWindow.Visibility = Visibility.Visible;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cstabItem.OwnWindow.CommonMethodForHideClose();
                                    }
                                }
                                cstabItem.OwnWindow.DockManager = cstabItem.OwnWindow.DockingManager.GetParentDockManager()._dockManager;
                                Window hoverWindow = cstabItem.OwnWindow.DockingManager.GetWindow(DockingManager.GetTargetNameInDockedMode(d));
                                if (hoverWindow != null && DockingManager.GetSideInDockedMode(d) != Dock.Tabbed)
                                {
                                    cstabItem.OwnWindow.DockingManager.mouseHoveredWindow = cstabItem.OwnWindow.DockingManager.GetExactParentWindowForDockedTabWindow(hoverWindow);

                                    cstabItem.OwnWindow.DockingManager._tarGetWindow = cstabItem.OwnWindow;
                                    if (cstabItem.OwnWindow.DockingManager.mouseHoveredWindow != null)
                                    {
                                        //if (cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.DockState == DockState.Dock || cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.DockState == DockState.AutoHidden)
                                        //{
                                            UIElement parent = cstabItem.OwnWindow.DockingManager.GetDockingGrid((UIElement)cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                            DockState ds = DockState.Dock;
                                            ds = cstabItem.OwnWindow.DockingManager.GetParentWindowContainer(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                            if (parent != null)
                                            {
                                                parent = (UIElement)VisualTreeHelper.GetParent((UIElement)parent);
                                            }
                                            cstabItem.OwnWindow.DockingManager.ApplyDefaultBackground(cstabItem.OwnWindow.DockingManager._tarGetWindow);
                                            cstabItem.OwnWindow.DockingManager.ApplyDefaultBackground(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                            cstabItem.OwnWindow.DockingManager.DockingSingleWindowFill(newValue, cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.ActualWidth, cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.ActualHeight);
                                            //cstabItem.OwnWindow.DockingManager.DockingFill(parent, ds, newValue);
                                        //}
                                    }
                                }
                                else
                                {
                                    cstabItem.OwnWindow.DockingManager.SetDockByPopUp(cstabItem.OwnWindow, newValue);
                                }
                                if (autoHiddenPresent)
                                {
                                    cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                    cstabItem.OwnWindow.dockToggle.IsChecked = true;
                                    cstabItem.OwnWindow.DockState = DockState.AutoHidden;
                                }
                            }

                            // cstab.RelatedWindow.DockPosition = newValue;//Dont use, it will suffer by replacechild method in dockmanager and window header double click                            
                        }
                        else if (!cstabItem.OwnWindow.InternalllyRaisedDockStateChanged && cstabItem.OwnWindow.DockManager.Parent is WindowContainer && state == DockState.Float)
                        {

                            if (newValue == Dock.Tabbed)
                            {
                                Window hoverWindow = cstabItem.OwnWindow.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(d));
                                if (oldValue != Dock.Tabbed && hoverWindow != null)
                                {
                                    bool isParentDockingManager = false;
                                    bool parentIsGridElement = false;
                                    bool isWindowContainerPresent = false;
                                    cstabItem.OwnWindow.WindowMoveFromDockLayout(ref isParentDockingManager, ref parentIsGridElement, ref isWindowContainerPresent);
                                    //cstabItem.OwnWindow.DockManager.gridDocking.Remove(cstabItem.OwnWindow);
                                    cstabItem.OwnWindow.DockPosition = newValue;
                                    cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                    DockingManager.SetSideInDockedMode(d, oldValue);

                                    cstabItem.OwnWindow.DockingManager.mouseHoveredWindow = cstabItem.OwnWindow.DockingManager.GetExactParentWindowForDockedTabWindow(hoverWindow);

                                    cstabItem.OwnWindow.DockingManager._tarGetWindow = cstabItem.OwnWindow;
                                    cstabItem.OwnWindow.DockingManager.HostingElementByCenterDragProvider(hoverWindow.DockState, new Point(0, 0));
                                }
                                else if (hoverWindow == null)
                                {
                                    cstabItem.OwnWindow.DockingManager.SetboolValueWithSideInMode(cstabItem.OwnWindow, oldValue, DockState.Float);
                                    DockingManager.SetSideInFloatMode(d, oldValue);
                                }
                            }
                            else
                            {
                                if (cstabItem.OwnWindow.DockState == DockState.AutoHidden)
                                {
                                    if (cstabItem.OwnWindow.dockToggle != null)
                                    {
                                        if (cstabItem.OwnWindow.dockToggle.Visibility == Visibility.Visible)
                                        {
                                            cstabItem.OwnWindow.DockingManager.RecentlyMouseHoveredSidePanel = cstabItem.OwnWindow.DockingManager.GetsidePanel(cstabItem.OwnWindow);
                                            cstabItem.OwnWindow.dockToggle.IsChecked = false;
                                            autoHiddenPresent = true;
                                        }
                                    }
                                }

                                if (oldValue != Dock.Tabbed)
                                {
                                    if (releatedWindow == cstabItem.OwnWindow)
                                    {
                                        if (releatedWindow.CustomTabControl.Items.Count > 1)
                                        {
                                            if (releatedWindow.CustomTabControl.Items.Contains(cstabItem))
                                            {

                                                releatedWindow.CustomTabControl.Items.Remove(cstabItem);
                                                if (cstabItem.OwnWindow.CustomTabControl != null && releatedWindow != cstabItem.OwnWindow)
                                                {
                                                    if (!cstabItem.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                                    {
                                                        cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                    }
                                                }
                                            }

                                            cstabItem.OwnWindow.DockingManager._parentTabbedWindow = releatedWindow;
                                            cstabItem.OwnWindow.DockingManager.TabbedWindow = cstabItem.OwnWindow;
                                            cstabItem.OwnWindow.DockingManager.UpdateCustomTabItem();
                                        }
                                    }
                                    else
                                    {
                                        cstabItem.OwnWindow.DockManager.gridDocking.Remove(cstabItem.OwnWindow);
                                    }

                                }
                                else
                                {
                                    if (releatedWindow != cstabItem.OwnWindow)
                                    {
                                        if (releatedWindow.CustomTabControl.Items.Contains(cstabItem))
                                        {
                                            releatedWindow.CustomTabControl.Items.Remove(cstabItem);
                                            releatedWindow.DockingManager.HideTabPanel(releatedWindow);
                                            if (!cstabItem.OwnWindow.CustomTabControl.Items.Contains(cstabItem))
                                            {
                                                cstabItem.OwnWindow.CustomTabControl.Items.Add(cstabItem);
                                                cstabItem.OwnWindow.DockingManager.HideTabPanel(cstabItem.OwnWindow);
                                                cstabItem.OwnWindow.Visibility = Visibility.Visible;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cstabItem.OwnWindow.CommonMethodForHideClose();
                                    }
                                }
                                //cstabItem.OwnWindow.DockManager = cstabItem.OwnWindow.DockingManager.GetParentDockManager()._dockManager;
                                Window hoverWindow = cstabItem.OwnWindow.DockingManager.GetWindow(DockingManager.GetTargetNameInFloatingMode(d));
                                if (hoverWindow != null)
                                {
                                    cstabItem.OwnWindow.DockingManager.mouseHoveredWindow = cstabItem.OwnWindow.DockingManager.GetExactParentWindowForFloatTabWindow(hoverWindow);

                                    cstabItem.OwnWindow.DockingManager._tarGetWindow = cstabItem.OwnWindow;
                                    if (cstabItem.OwnWindow.DockingManager.mouseHoveredWindow != null)
                                    {
                                        UIElement parent = cstabItem.OwnWindow.DockingManager.GetDockingGrid((UIElement)cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                        DockState ds = DockState.Dock;
                                        ds = cstabItem.OwnWindow.DockingManager.GetParentWindowContainer(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                        if (parent != null)
                                        {
                                            parent = (UIElement)VisualTreeHelper.GetParent((UIElement)parent);
                                        }
                                        cstabItem.OwnWindow.DockingManager.ApplyDefaultBackground(cstabItem.OwnWindow.DockingManager._tarGetWindow);
                                        cstabItem.OwnWindow.DockingManager.ApplyDefaultBackground(cstabItem.OwnWindow.DockingManager.mouseHoveredWindow);
                                        cstabItem.OwnWindow.DockingManager.DockingSingleWindowFill(newValue, cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.ActualWidth, cstabItem.OwnWindow.DockingManager.mouseHoveredWindow.ActualHeight);
                                        //cstabItem.OwnWindow.DockingManager.DockingFill(parent, ds, newValue);

                                    }
                                }
                                else
                                {
                                    //cstabItem.OwnWindow.DockingManager.SetDockByPopUp(cstabItem.OwnWindow, newValue);
                                }
                                if (autoHiddenPresent)
                                {
                                    cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = true;
                                    cstabItem.OwnWindow.dockToggle.IsChecked = true;
                                    cstabItem.OwnWindow.DockState = DockState.AutoHidden;
                                }
                            }



                        }
                        cstabItem.OwnWindow.InternalllyRaisedDockStateChanged = false;
                    }

                    #endregion
                }
            }
        }


        /// <summary>
        /// Gets the no header.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetNoHeader(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(NoHeaderProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="noHeader">if set to <c>true</c> [no header].</param>
        public static void SetNoHeader(UIElement element, bool noHeader)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(NoHeaderProperty, noHeader);
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty NoHeaderProperty =
            DependencyProperty.RegisterAttached(
                "NoHeader",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(false, OnNoHeaderPropertyChanged));

        /// <summary>
        /// Called when [no header property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNoHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    if (((CustomTabItem)((FrameworkElement)d).Parent).IsSelected && ((CustomTabItem)((FrameworkElement)d).Parent).Parent != null && ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.WindowChildElement != null)
                    {
                        if (((CustomTabItem)((FrameworkElement)d).Parent).Parent is CustomTabControl)
                        {
                            if (((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.DockState != DockState.Dock)
                            {
                                DockingManager.SetNoHeader(d as UIElement, false);
                            }
                           (((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl).RelatedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.WindowChildElement), DockingManager.GetHeaderHeight(((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.WindowChildElement));
                        }
                    }
                }
            }
        }




        /// <summary>
        /// Gets the height of the header.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static double GetHeaderHeight(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (double)element.GetValue(HeaderHeightProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="headerHeight">Height of the header.</param>
        public static void SetHeaderHeight(UIElement element, double headerHeight)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(HeaderHeightProperty, headerHeight);
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.RegisterAttached(
                "HeaderHeight",
                typeof(double),
                typeof(DockingManager),
                new PropertyMetadata(18d, OnHeaderHeightPropertyChanged));

        /// <summary>
        /// Called when [header height property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    if (((CustomTabItem)((FrameworkElement)d).Parent).IsSelected && ((CustomTabItem)((FrameworkElement)d).Parent).Parent != null && ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.WindowChildElement != null)
                    {
                        if (((CustomTabItem)((FrameworkElement)d).Parent).Parent is CustomTabControl)
                        {
                            (((CustomTabItem)((FrameworkElement)d).Parent).Parent as CustomTabControl).RelatedWindow.NoHeaderVisibility(DockingManager.GetNoHeader(((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.WindowChildElement), DockingManager.GetHeaderHeight(((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.WindowChildElement));
                        }
                    }
                }
            }
        }







        /// <summary>
        /// Gets the custom context menu items.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static CustomContextMenuItemCollection GetCustomContextMenuItems(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (CustomContextMenuItemCollection)element.GetValue(CustomContextMenuItemsProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="customMenuItemCollection">The custom menu item collection.</param>
        public static void SetCustomContextMenuItems(UIElement element, CustomContextMenuItemCollection customMenuItemCollection)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(CustomContextMenuItemsProperty, customMenuItemCollection);
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomContextMenuItemsProperty =
            DependencyProperty.RegisterAttached(
                "CustomContextMenuItems",
                typeof(CustomContextMenuItemCollection),
                typeof(DockingManager),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the content of the size to.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetSizeToContent(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(SizeToContentProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="setSizeToContent">if set to <c>true</c> [set size to content].</param>
        public static void SetSizeToContent(UIElement element, bool setSizeToContent)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(SizeToContentProperty, setSizeToContent);
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeToContentProperty =
            DependencyProperty.RegisterAttached(
                "SizeToContent",
                typeof(bool),
                typeof(DockingManager),
                new PropertyMetadata(false, OnSizeToContentPropertyChanged));





        /// <summary>
        /// Called when [size to content property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSizeToContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    //if (((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.menuItemadv != null)
                    //{
                        Window temp = ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow;
                        if (temp.WindowChildElement != null)
                        {
                            if ((temp.WindowChildElement as FrameworkElement).Height != 0.0 && (temp.WindowChildElement as FrameworkElement).Height.ToString() != "NaN")
                            {
                                temp.FloatHeight = (temp.WindowChildElement as FrameworkElement).Height;
                            }
                            else
                            {
                                temp.FloatHeight = 200;
                            }
                            if ((temp.WindowChildElement as FrameworkElement).Width != 0.0 && (temp.WindowChildElement as FrameworkElement).Width.ToString() != "NaN")
                            {
                                temp.FloatWidth = (temp.WindowChildElement as FrameworkElement).Width;
                            }
                            else
                            {
                                temp.FloatWidth = 200;
                            }

                        }
                    //}
                    // ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.NoHeaderVisibility((bool)e.NewValue);
                }
            }
        }









        /// <summary>
        /// Gets the floating window rect.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static Rect GetFloatingWindowRect(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (Rect)element.GetValue(FloatingWindowRectProperty);
        }

        /// <summary>
        /// Sets the value of the Header attached property to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void SetFloatingWindowRect(UIElement element, Rect rectangle)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(FloatingWindowRectProperty, rectangle);
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty FloatingWindowRectProperty =
            DependencyProperty.RegisterAttached(
                "FloatingWindowRect",
                typeof(Rect),
                typeof(DockingManager),
                new PropertyMetadata(Rect.Empty, OnFloatingWindowRectPropertyChanged));

        /// <summary>
        /// Called when [floating window rect property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatingWindowRectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    //if (((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.menuItemadv != null)
                    //{
                        ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.LeftPosition = ((Rect)e.NewValue).Left;
                        ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.TopPosition = ((Rect)e.NewValue).Top;
                        ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.FloatHeight = ((Rect)e.NewValue).Height;
                        ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.FloatWidth = ((Rect)e.NewValue).Width;
                        //foreach (MenuItemAdv item in e.NewValue as CustomMenuItemCollection)
                        //{
                        //    ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.menuItemadv.Items.Add(item);
                        //}
                    //}
                    // ((CustomTabItem)((FrameworkElement)d).Parent).OwnWindow.NoHeaderVisibility((bool)e.NewValue);
                }
            }
        }


        /// <summary>
        /// Called when [icon property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((FrameworkElement)d).Parent != null)
            {
                if (((FrameworkElement)d).Parent.GetType() == typeof(CustomTabItem))
                {
                    ((CustomTabItem)((FrameworkElement)d).Parent).Icon = (Brush)e.NewValue;
                }
            }
        }

        /// <summary>
        /// Gets the side.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public static Dock GetSide(DependencyObject obj, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    return GetSideInDockedMode(obj);

                case DockState.Float:
                    return GetSideInFloatMode(obj);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets the side.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="side">The side.</param>
        /// <param name="state">The state.</param>
        public static void SetSide(DependencyObject obj, Dock side, DockState state)
        {
            switch (state)
            {
                case DockState.Dock:
                    SetSideInDockedMode(obj, side);
                    break;

                case DockState.Float:
                    SetSideInFloatMode(obj, side);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the value of the DockingManager.SideInFloatMode attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.SideInFloatMode attached property.</returns>
        public static Dock GetSideInFloatMode(DependencyObject obj)
        {
            return (Dock)obj.GetValue(SideInFloatModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.SideInFloatMode attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.SideInFloatMode attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetSideInFloatMode(DependencyObject obj, Dock value)
        {
            obj.SetValue(SideInFloatModeProperty, value);

        }

        /// <summary>
        /// Gets the value of the DockingManager.SideInDockedMode attached property from a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element from which to read the property value.</param>
        /// <returns>The value of the DockingManager.SideInDockedMode attached property.</returns>
        public static Dock GetSideInDockedMode(DependencyObject obj)
        {
            return (Dock)obj.GetValue(SideInDockedModeProperty);
        }

        /// <summary>
        /// Sets the value of the DockingManager.SideInDockedMode attached property to a given DependencyObject.
        /// </summary>
        /// <param name="obj">The element on which to set the DockingManager.SideInDockedMode attached property.</param>
        /// <param name="value">The property value to set.</param>
        /// <remarks>
        /// See <see cref="DockSide"/> for all possiible DockSide cases.
        /// </remarks>
        public static void SetSideInDockedMode(DependencyObject obj, Dock value)
        {
            obj.SetValue(SideInDockedModeProperty, value);
        }

        /// <summary>
        /// Invoked whenever the WindowBackgroundProperty is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWindowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowBackgroundPropertyChanged(e);
        }

        /// <summary>
        /// Called when [side tab background property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSideTabBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnSideTabBackgroundPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the HeaderBackgroundProperty is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data</param>
        private static void OnHeaderBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnHeaderBackgroundPropertyChanged(e);
        }

        /// <summary>
        /// Called when [resource dictionary property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnResourceDictionaryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnResourceDictionaryPropertyChanged(e);
        }

        /// <summary>
        /// Called when [active window property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnActiveWindowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnActiveWindowPropertyChanged(e);
        }

        /// <summary>
        /// Called when [dock fill property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDockFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnDockFillPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DockFillPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDockFillPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                DockingGrid dg = this.GetParentDockManager();
                if (dg != null)
                {
                    dg.rootWindow.DockingManager = this;
                    dg.rootWindow.DockState = DockState.Hidden;
                    //dg.colletion.Clear();
                    //dg.GetWindowElement(dg.gridDocking);
                    //dg.ArangeEntireDockFillLayout();
                    dg.ArrangeLayout();
                }
            }
            else
            {
                DockingGrid dg = this.GetParentDockManager();
                if (dg != null)
                {
                    dg.rootWindow.DockingManager = this;
                    dg.rootedWindow.Visibility = Visibility.Visible;
                    dg.rootWindow.DockState = DockState.Dock;
                    dg.rootWindow.Background = DocumentBackGround;
                    dg.rootWindow.WindowBackGround = WindowBackground;
                    dg.rootWindow.WindowBorderBrush = WindowBorderBrush;
                    dg.rootWindow.WindowBorderThickness = WindowBorderThickness;
                    dg.rootWindow.WindowBorderBrush = DocumentBorderBrush;

                    // dg.colletion.Clear();
                    // dg.GetAllWindowElement(dg.gridDocking);
                    // if (!dg.colletion.Contains(dg.rootWindow))
                    // {
                    //     dg.colletion.Add(dg.rootWindow);
                    // }
                    // dg.ArangeEntireUnDockFillLayout();
                    dg.ArrangeLayout();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnFreezeLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnFreezeLayoutPropertyChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFreezeLayoutPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(!e.NewValue.Equals(e.OldValue))
            {
                DockingGrid dg = this.GetParentDockManager();
                bool isFreeze = (bool)e.NewValue;
                if (dg != null && dg.gridDocking != null)
                {
                    this.FreezeGridLayout(dg.gridDocking, isFreeze);
                }
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ActiveWindowChanged"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback ActiveWindowChanged;

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <param name="cstabControl">The cstab control.</param>
        /// <returns></returns>
        protected internal Window GetWindow(CustomTabControl cstabControl)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].CustomTabControl != null)
                {
                    if (this.WindowCollection[i].CustomTabControl == cstabControl)
                    {
                        return this.WindowCollection[i];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Raises the <see cref="E:ActiveWindowPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnActiveWindowPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ActiveWindowChanged != null)
            {
                ActiveWindowChanged(this, e);
            }

            Window newWindow = null;
            Window oldWindow = null;

            if (((Window)e.NewValue) != null)
            {
                CustomTabControl cstab = null;
                if (e.NewValue is Window && (FrameworkElement)((Window)e.NewValue).WindowChildElement != null && ((FrameworkElement)((Window)e.NewValue).WindowChildElement).Parent != null && ((FrameworkElement)((Window)e.NewValue).WindowChildElement).Parent.GetType() == typeof(CustomTabItem))
                {
                    cstab = ((CustomTabItem)((FrameworkElement)((Window)e.NewValue).WindowChildElement).Parent).Parent as CustomTabControl;
                    
                    if (cstab != null && cstab.Items.Count > 1)
                    {
                        newWindow = GetWindow(cstab);
                    }
                    else
                    {
                        newWindow = (Window)e.NewValue;
                    }
                }

                if (newWindow != null && cstab != null)
                {
                    newWindow.ActiveForeground = ActiveForeground;
                    if (newWindow.maximizeButton != null)
                        VisualStateManager.GoToState(newWindow.maximizeButton, "Active", false);
                    if (newWindow.closeButton != null)
                        VisualStateManager.GoToState(newWindow.closeButton, "Active", false);
                    if (newWindow.dockToggle != null)
                        VisualStateManager.GoToState(newWindow.dockToggle, "Active", false);
                    if (newWindow.optionsButton != null)
                        VisualStateManager.GoToState(newWindow.optionsButton, "Active", false);
                    if (newWindow.DockState == DockState.Float || newWindow.DockState == DockState.Hidden)
                    {
                        newWindow.HeaderBackgroud = FloatWindowActiveHeaderBackground;
                        // ((Window)e.NewValue).WindowBorderBrush = FloatWindowActiveBorderBrush;
                        //((Window)e.NewValue).ApplyBorderForFloatWindow();
                    }
                    else
                    {
                        newWindow.HeaderBackgroud = ActiveWindowColor;
                        //((Window)e.NewValue).WindowBorderBrush = WindowBorderBrush;
                        //if (((Window)e.NewValue).contentBorder != null)
                        //{
                        //    ((Window)e.NewValue).contentBorder.Margin = new Thickness(0);
                        //}
                    }
                }
            }

            if (((Window)e.OldValue) != null)
            {
                CustomTabControl cstab = null;
                if (e.OldValue is Window && (FrameworkElement)((Window)e.OldValue).WindowChildElement != null && ((FrameworkElement)((Window)e.OldValue).WindowChildElement).Parent != null && ((FrameworkElement)((Window)e.OldValue).WindowChildElement).Parent.GetType() == typeof(CustomTabItem))
                {
                    cstab = ((CustomTabItem)((FrameworkElement)((Window)e.OldValue).WindowChildElement).Parent).Parent as CustomTabControl;
                    if (cstab != null && cstab.Items.Count>1)
                    {
                        oldWindow = GetWindow(cstab); ;
                    }
                    else
                    {
                        oldWindow = (Window)e.OldValue;
                    }
                }
                if (oldWindow != null && oldWindow != newWindow && cstab != null)
                {
                    if (oldWindow.DockState == DockState.AutoHidden)
                    {
                        oldWindow.AutoHide = true;
                        this.timer.Start();
                    }

                    oldWindow.CaptionForeGround = CaptionForeGround;

                    if (oldWindow.maximizeButton != null)
                        VisualStateManager.GoToState(oldWindow.maximizeButton, "InActive", false);
                    if (oldWindow.closeButton != null)
                        VisualStateManager.GoToState(oldWindow.closeButton, "InActive", false);
                    if (oldWindow.dockToggle != null)
                        VisualStateManager.GoToState(oldWindow.dockToggle, "InActive", false);
                    if (oldWindow.optionsButton != null)
                        VisualStateManager.GoToState(oldWindow.optionsButton, "InActive", false);

                    if (oldWindow.DockState == DockState.Float || oldWindow.DockState == DockState.Hidden)
                    {
                        oldWindow.HeaderBackgroud = FloatWindowHeaderBackground;
                        //((Window)e.OldValue).WindowBorderBrush = FloatWindowBorderBrush;
                        // ((Window)e.NewValue).ApplyBorderForFloatWindow();
                    }
                    else
                    {
                        oldWindow.HeaderBackgroud = HeaderBackground;
                        //((Window)e.OldValue).WindowBorderBrush = WindowBorderBrush;
                        //if (((Window)e.OldValue).contentBorder != null)
                        //{
                        //    ((Window)e.OldValue).contentBorder.Margin = new Thickness(0);
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// Removes the auto hide animation.
        /// </summary>
        /// <param name="leftButtonDownWindow">The left button down window.</param>
        protected internal void RemoveAutoHideAnimation(Window leftButtonDownWindow)
        {
            List<Window> windowCollection = new List<Window>(this.WindowCollection.Values); //((Window)tempwindow).WindowDockPin == DockPin.Pinned
            IEnumerable<Window> windowquery = windowCollection.Where(tempwindow => ((Window)tempwindow).DockState == DockState.AutoHidden && ((Window)tempwindow).dockToggle != null && base.Children.Contains(((Window)tempwindow)) && ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).CustomTabControl.Items.Count > 0 && ((Window)tempwindow) != leftButtonDownWindow);
            foreach (Window w in windowquery)
            {
                if ((bool)w.dockToggle.IsChecked)
                {
                    Animation objAnimation = new Animation(w);
                    switch (w.DockPosition)
                    {
                        case Dock.Bottom:
                            objAnimation.AnimateSize(w.Width, 0);
                            objAnimation.AnimatePosition(Canvas.GetLeft(w), this.ActualHeight - 20);
                            break;

                        case Dock.Left:
                            objAnimation.AnimateSize(0, w.Height);
                            objAnimation.AnimatePosition(20, Canvas.GetTop(w));
                            break;

                        case Dock.Right:
                            objAnimation.AnimateSize(0, w.Height);
                            objAnimation.AnimatePosition(this.ActualWidth - 20, Canvas.GetTop(w));
                            break;

                        case Dock.Top:
                            objAnimation.AnimateSize(w.Width, 0);
                            objAnimation.AnimatePosition(Canvas.GetLeft(w), 20);
                            break;
                    }

                    timer.Stop();
                    stackPanelLeave = false;
                    w.AutoHide = false;
                }
            }
        }


        /// <summary>
        /// Raises the <see cref="E:HeaderBackgroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnHeaderBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].HeaderBackgroud = newBrush;
                }
            }
            else
            {
                throw new ArgumentException("Grid Background Changed property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ResourceDictionaryPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnResourceDictionaryPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary newBrush = e.NewValue as ResourceDictionary;
            //OnApplyTheme(Theme);
        }

        /// <summary>
        /// Raises the <see cref="E:SideTabBackgroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSideTabBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (m_bottomSideGrid != null)
            {
                m_bottomSideGrid.Background = newBrush;
            }

            if (m_leftSideGrid != null)
            {
                m_leftSideGrid.Background = newBrush;
            }

            if (m_rightSideGrid != null)
            {
                m_rightSideGrid.Background = newBrush;
            }

            if (m_topSideGrid != null)
            {
                m_topSideGrid.Background = newBrush;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WindowBackgroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].CustomTabControl.WindowBackground = newBrush;
                        this.WindowCollection[i].Background = newBrush;
                        this.WindowCollection[i].WindowBackGround = newBrush;
                    }
                }
                DockingGrid dockingGrid = GetParentDockManager();
                DockManager dm = null;
                if (dockingGrid != null && dockingGrid.Parent != null)
                {
                    dm = dockingGrid.Parent as DockManager;
                }
                if (dm != null)
                {
                    dm.Background = WindowBackground;
                    if (clientGrid != null)
                    {
                        clientGrid.Background = WindowBackground;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Grid Background Changed property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Identifies the ActiveWindow Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ActiveWindowProperty =
           DependencyProperty.Register("ActiveWindow", typeof(Window), typeof(DockingManager), new PropertyMetadata(null, OnActiveWindowPropertyChanged));

        /// <summary>
        /// Identifies the AutoHideAnimationSpeed Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AutoHideAnimationSpeedProperty =
           DependencyProperty.Register("AutoHideAnimationSpeed", typeof(int), typeof(DockingManager), new PropertyMetadata(500));

        /// <summary>
        /// Identifies the DockFill Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DockFillProperty =
           DependencyProperty.Register("DockFill", typeof(bool), typeof(DockingManager), new PropertyMetadata(false, OnDockFillPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowBackgroundProperty =
            DependencyProperty.Register("WindowBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xD5, 0xE4, 0xF2)), OnWindowBackgroundPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="SideTabBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SideTabBackgroundProperty =
          DependencyProperty.Register("SideTabBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata((new SolidColorBrush(Color.FromArgb(0xFF, 0xCA, 0xDE, 0xF7))), OnSideTabBackgroundPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="BottomImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomImagePathProperty =
           DependencyProperty.Register("BottomImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008Bottom.png"));

        /// <summary>
        /// Identifies the <see cref="TopImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopImagePathProperty =
           DependencyProperty.Register("TopImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008Top.png"));

        /// <summary>
        /// Identifies the <see cref="LeftImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftImagePathProperty =
           DependencyProperty.Register("LeftImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008Left.png"));
        /// <summary>
        /// Identifies the <see cref="RightImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightImagePathProperty =
           DependencyProperty.Register("RightImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008Right.png"));
        /// <summary>
        /// Identifies the <see cref="BottomOverImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomOverImagePathProperty =
           DependencyProperty.Register("BottomOverImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008BottomOver.png"));
        /// <summary>
        /// Identifies the <see cref="TopOverImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopOverImagePathProperty =
           DependencyProperty.Register("TopOverImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008TopOver.png"));
        /// <summary>
        /// Identifies the <see cref="LeftOverImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftOverImagePathProperty =
           DependencyProperty.Register("LeftOverImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008LeftOver.png"));
        /// <summary>
        /// Identifies the <see cref="RightOverImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightOverImagePathProperty =
           DependencyProperty.Register("RightOverImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008RightOver.png"));
        /// <summary>
        /// Identifies the <see cref="CenterImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterImagePathProperty =
          DependencyProperty.Register("CenterImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008Center.png"));
        /// <summary>
        /// Identifies the <see cref="CenterOverImagePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterOverImagePathProperty =
          DependencyProperty.Register("CenterOverImagePath", typeof(string), typeof(DockingManager), new PropertyMetadata("/Syncfusion.DockingManager.Silverlight;component/Images/DockPreviewVS2008CenterOver.png"));

        /// <summary>
        /// Identifies the <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(DefaultHeaderbackground(), OnHeaderBackgroundPropertyChanged));
        /// <summary>
        /// Identifies the <see cref="ResourceDictionary"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResourceDictionaryProperty =
          DependencyProperty.Register("ResourceDictionary", typeof(ResourceDictionary), typeof(DockingManager), new PropertyMetadata(null, OnResourceDictionaryPropertyChanged));

        /// <summary>
        /// Specifies where user can dock an element
        /// </summary>
        public static readonly DependencyProperty OuterDockAbilityProperty = DependencyProperty.Register("OuterDockAbility", typeof(OuterDockAbility), typeof(DockingManager), new PropertyMetadata(OuterDockAbility.All));

        /// <summary>
        /// Indicates whether to allow resizing of all the windows
        /// </summary>
        public static readonly DependencyProperty FreezeLayoutProperty =DependencyProperty.Register("FreezeLayout",typeof(bool),typeof(DockingManager), new PropertyMetadata(false,new PropertyChangedCallback(OnFreezeLayoutPropertyChanged)));


        /// <summary>
        /// Defaults the active headerbackground.
        /// </summary>
        /// <returns></returns>
        protected static LinearGradientBrush DefaultActiveHeaderbackground()
        {
            LinearGradientBrush linearGradient = new LinearGradientBrush();
            linearGradient.StartPoint = new Point(0.5, 0);
            linearGradient.EndPoint = new Point(0.5, 1);
            GradientStopCollection gradientCollection = new GradientStopCollection();
            GradientStop gradient = new GradientStop();
            gradient.Offset = 0;
            gradient.Color = Color.FromArgb(0xFF, 0xCA, 0xDE, 0xF7);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 1;
            gradient.Color = Color.FromArgb(0xFF, 0xE2, 0xD1, 0x9C);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0;
            gradient.Color = Color.FromArgb(0xFF, 0xF8, 0xF5, 0xED);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.4;
            gradient.Color = Color.FromArgb(0xFF, 0xFF, 0xBB, 0x6E);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.4;
            gradient.Color = Color.FromArgb(0xFF, 0xF9, 0xCE, 0x9F);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 1;
            gradient.Color = Color.FromArgb(0xFF, 0xFF, 0xE3, 0x93);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.381;
            gradient.Color = Color.FromArgb(0xFF, 0xFA, 0xCC, 0x99);
            gradientCollection.Add(gradient);
            linearGradient.GradientStops = gradientCollection;
            return linearGradient;
        }



        /// <summary>
        /// Defaults the floata active headerbackground.
        /// </summary>
        /// <returns></returns>
        protected static LinearGradientBrush DefaultFloataActiveHeaderbackground()
        {
            LinearGradientBrush linearGradient = new LinearGradientBrush();
            linearGradient.StartPoint = new Point(0, 0);
            linearGradient.EndPoint = new Point(0, 1);
            GradientStopCollection gradientCollection = new GradientStopCollection();
            GradientStop gradient = new GradientStop();
            gradient.Offset = 0;
            gradient.Color = Color.FromArgb(0xFF, 0xE0, 0xE2, 0xE2);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.25;
            gradient.Color = Color.FromArgb(0xFF, 0xE8, 0xEF, 0xF7);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.3;
            gradient.Color = Color.FromArgb(0xFF, 0xE6, 0xE9, 0xED);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 1;
            gradient.Color = Color.FromArgb(0xFF, 0xDE, 0xE5, 0xE8);
            gradientCollection.Add(gradient);
            linearGradient.GradientStops = gradientCollection;
            return linearGradient;
        }




        /// <summary>
        /// Defaults the float headerbackground.
        /// </summary>
        /// <returns></returns>
        protected static LinearGradientBrush DefaultFloatHeaderbackground()
        {
            LinearGradientBrush linearGradient = new LinearGradientBrush();
            linearGradient.StartPoint = new Point(0, 0);
            linearGradient.EndPoint = new Point(0, 1);
            GradientStopCollection gradientCollection = new GradientStopCollection();
            GradientStop gradient = new GradientStop();
            gradient.Offset = 0;
            gradient.Color = Color.FromArgb(0xFF, 0xE1, 0xEB, 0xF8);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.2;
            gradient.Color = Color.FromArgb(0xFF, 0xD7, 0xE6, 0xF9);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.3;
            gradient.Color = Color.FromArgb(0xFF, 0x96, 0xc8, 0xF2);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 1;
            gradient.Color = Color.FromArgb(0xFF, 0xE4, 0xEF, 0xFD);
            gradientCollection.Add(gradient);
            linearGradient.GradientStops = gradientCollection;
            return linearGradient;
        }


        /// <summary>
        /// Defaults the headerbackground.
        /// </summary>
        /// <returns></returns>
        protected static LinearGradientBrush DefaultHeaderbackground()
        {
            LinearGradientBrush linearGradient = new LinearGradientBrush();
            linearGradient.StartPoint = new Point(0.5, 0);
            linearGradient.EndPoint = new Point(0.5, 1);
            GradientStopCollection gradientCollection = new GradientStopCollection();
            GradientStop gradient = new GradientStop();
            gradient.Offset = 0;
            gradient.Color = Color.FromArgb(0xFF, 0xE4, 0xEB, 0xF6);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.32;
            gradient.Color = Color.FromArgb(0xFF, 0xD9, 0xE7, 0xF9);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 0.35;
            gradient.Color = Color.FromArgb(0xFF, 0xCA, 0xDE, 0xF7);
            gradientCollection.Add(gradient);

            gradient = new GradientStop();
            gradient.Offset = 1;
            gradient.Color = Color.FromArgb(0xFF, 0xDB, 0xF4, 0xFE);
            gradientCollection.Add(gradient);
            linearGradient.GradientStops = gradientCollection;
            return linearGradient;
        }
        /// <summary>
        /// Gets or sets the style used by TabControl when it is rendered. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Provides TabControlStyle value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialise the style of tab control and set TabControlStyle property in XAML like you set <see cref="TabControlStyle"/> property.
        /// </example>
        public Style TabControlStyle
        {
            get
            {
                return (Style)GetValue(TabControlStyleProperty);
            }

            set
            {
                SetValue(TabControlStyleProperty, value);
            }
        }

        /// <summary>
        /// Identifies the TabControlStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabControlStyleProperty =
            DependencyProperty.Register("TabControlStyle", typeof(Style), typeof(DockingManager), new PropertyMetadata(OnTabControlStyleChanged));

        /// <summary>
        /// Called when [tab control style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabControlStyleChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabControlStyleChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the TabItemBackgroundSelected
        /// TabItemBackgroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemBackgroundSelected value for the <see cref="DockingManager"/>. The default value of the TabItemBackgroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemBackgroundSelected dependency property defines background of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemBackgroundSelected property please see <see cref="SplitterBackGroundColor"/> property example.
        /// </example>
        public Brush TabItemBackgroundSelected
        {
            get
            {
                return (Brush)GetValue(TabItemBackgroundSelectedProperty);
            }

            set
            {
                SetValue(TabItemBackgroundSelectedProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemBackgroundSelected"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemBackgroundSelectedProperty =
        DependencyProperty.Register("TabItemBackgroundSelected", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xF0, 0xFA)), OnTabItemBackgroundSelectedPropertyChanged));

        /// <summary>
        /// Called when [tab item background selected property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemBackgroundSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemBackgroundSelectedPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemBackgroundSelectedPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemBackgroundSelectedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = (Brush)e.NewValue;
            if (newBrush != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the document back ground.
        /// </summary>
        /// <value>The document back ground.</value>
        public Brush DocumentBackGround
        {
            get
            {
                return (Brush)GetValue(DocumentBackGroundProperty);
            }

            set
            {
                SetValue(DocumentBackGroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DocumentBackGround"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DocumentBackGroundProperty =
         DependencyProperty.Register("DocumentBackGround", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xF0, 0xFA)), OnDocumentBackGroundPropertyChanged));//#D5E4F2"


        /// <summary>
        /// Called when [document back ground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDocumentBackGroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnDocumentBackGroundPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DocumentBackGroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDocumentBackGroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = (Brush)e.NewValue;
            if (newBrush != null)
            {
                DockingGrid dockingGrid = GetParentDockManager();
                DockManager dm = null;
                if (dockingGrid != null && dockingGrid.Parent != null)
                {
                    dm = dockingGrid.Parent as DockManager;
                }
                if (dm != null)
                {
                    if (!DockFill)
                    {
                        DockingGrid gr = dockingGrid;
                        gr.rootWindow.Background = newBrush;

                        if (gr.rootWindow.ContentGrid != null)
                        {
                            (gr.rootWindow.ContentGrid.Parent as Border).Background = newBrush;
                            ((gr.rootWindow.ContentGrid.Parent as Border).Parent as Border).Background = newBrush;
                            gr.rootWindow.contentpresenter.Background = newBrush;
                            clientGrid.Background = newBrush;

                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the document border brush.
        /// </summary>
        /// <value>The document border brush.</value>
        public Brush DocumentBorderBrush
        {
            get
            {
                return (Brush)GetValue(DocumentBorderBrushProperty);
            }

            set
            {
                SetValue(DocumentBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DocumentBorderBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DocumentBorderBrushProperty =
         DependencyProperty.Register("DocumentBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x79, 0x99, 0xC7)), OnDocumentBorderBrushPropertyChanged));

        /// <summary>
        /// Called when [document border brush property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDocumentBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnDocumentBorderBrushPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DocumentBorderBrushPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDocumentBorderBrushPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = (Brush)e.NewValue;
            if (newBrush != null)
            {
                DockingGrid dockingGrid = GetParentDockManager();
                DockManager dm = null;
                if (dockingGrid != null && dockingGrid.Parent != null)
                {
                    dm = dockingGrid.Parent as DockManager;
                }
                if (dm != null)
                {
                    if (!DockFill)
                    {
                        DockingGrid gr = dockingGrid;
                        gr.rootWindow.WindowBorderBrush = newBrush;

                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the TabItemForeGroundSelected
        /// TabItemForegroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemForegroundSelected value for the <see cref="DockingManager"/>. The default value of the TabItemForegroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemForegroundSelected dependency property defines foreground of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemForegroundSelected property please see <see cref="SplitterBackGroundColor"/> property example.
        /// </example>
        public Brush TabItemForegroundSelected
        {
            get
            {
                return (Brush)GetValue(TabItemForegroundSelectedProperty);
            }

            set
            {
                SetValue(TabItemForegroundSelectedProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemForegroundSelected"/> Dependency property.
        /// </summary>
        public static readonly DependencyProperty TabItemForegroundSelectedProperty =
        DependencyProperty.Register("TabItemForegroundSelected", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x04, 0x22, 0x71)), OnTabItemForegroundSelectedPropertyChanged));

        /// <summary>
        /// Called when [tab item foreground selected property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemForegroundSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemForegroundSelectedPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemForegroundSelectedPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemForegroundSelectedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = (Brush)e.NewValue;
            if (newBrush != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the window border brush.
        /// </summary>
        /// <value>The window border brush.</value>
        public Brush WindowBorderBrush
        {
            get
            {
                return (Brush)GetValue(WindowBorderBrushProperty);
            }

            set
            {
                SetValue(WindowBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowBorderBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty WindowBorderBrushProperty =
      DependencyProperty.Register("WindowBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x79, 0x99, 0xC7)), OnWindowBorderBrushPropertyChanged));

        /// <summary>
        /// Gets or sets the window border thickness.
        /// </summary>
        /// <value>The window border thickness.</value>
        public Thickness WindowBorderThickness
        {
            get
            {
                return (Thickness)GetValue(WindowBorderThicknessProperty);
            }

            set
            {
                SetValue(WindowBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowBorderThickness"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty WindowBorderThicknessProperty =
      DependencyProperty.Register("WindowBorderThickness", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(1), OnWindowBorderThicknessPropertyChanged));

        /// <summary>
        /// Gets or sets the window corner radius.
        /// </summary>
        /// <value>The window corner radius.</value>
        public CornerRadius WindowCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(WindowCornerRadiusProperty);
            }

            set
            {
                SetValue(WindowCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowCornerRadius"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty WindowCornerRadiusProperty =
      DependencyProperty.Register("WindowCornerRadius", typeof(CornerRadius), typeof(DockingManager), new PropertyMetadata(new CornerRadius(1), OnWindowCornerRadiusPropertyChanged));

        /// <summary>
        /// Gets or sets the header border brush.
        /// </summary>
        /// <value>The header border brush.</value>
        public Brush HeaderBorderBrush
        {
            get
            {
                return (Brush)GetValue(HeaderBorderBrushProperty);
            }

            set
            {
                SetValue(HeaderBorderBrushProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="HeaderBorderBrush"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty HeaderBorderBrushProperty =
      DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnHeaderBorderBrushPropertyChanged));

        /// <summary>
        /// Gets or sets the caption fore ground.
        /// </summary>
        /// <value>The caption fore ground.</value>
        public Brush CaptionForeGround
        {
            get
            {
                return (Brush)GetValue(CaptionForeGroundProperty);
            }

            set
            {
                SetValue(CaptionForeGroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionForeGround"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty CaptionForeGroundProperty =
      DependencyProperty.Register("CaptionForeGround", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnCaptionForeGroundPropertyChanged));

        /// <summary>
        /// Gets or sets the float window background.
        /// </summary>
        /// <value>The float window background.</value>
        public Brush FloatWindowBackground
        {
            get
            {
                return (Brush)GetValue(FloatWindowBackgroundProperty);
            }

            set
            {
                SetValue(FloatWindowBackgroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowBackground"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowBackgroundProperty =
      DependencyProperty.Register("FloatWindowBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 213, 228, 242)), OnFloatWindowBackGroundChanged));

        /// <summary>
        /// Called when [float window back ground changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatWindowBackGroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnFloatWindowBackGroundChanged(e);

        }

        /// <summary>
        /// Raises the <see cref="E:FloatWindowBackGroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFloatWindowBackGroundChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplythemeStyle();
        }

        /// <summary>
        /// Gets or sets the size of the caption font.
        /// </summary>
        /// <value>The size of the caption font.</value>
        public double CaptionFontSize
        {
            get
            {
                return (double)GetValue(CaptionFontSizeProperty);
            }

            set
            {
                SetValue(CaptionFontSizeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionFontSize"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty CaptionFontSizeProperty =
      DependencyProperty.Register("CaptionFontSize", typeof(double), typeof(DockingManager), new PropertyMetadata(12d, OnCaptionFontSizePropertyChanged));

        /// <summary>
        /// Gets or sets the caption font family.
        /// </summary>
        /// <value>The caption font family.</value>
        public FontFamily CaptionFontFamily
        {
            get
            {
                return (FontFamily)GetValue(CaptionFontFamilyProperty);
            }

            set
            {
                SetValue(CaptionFontFamilyProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionFontFamily"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty CaptionFontFamilyProperty =
      DependencyProperty.Register("CaptionFontFamily", typeof(FontFamily), typeof(DockingManager), new PropertyMetadata(new FontFamily("Tahoma"), OnCaptionFontFamilyPropertyChanged));

        /// <summary>
        /// Gets or sets the caption margin.
        /// </summary>
        /// <value>The caption margin.</value>
        public Thickness CaptionMargin
        {
            get
            {
                return (Thickness)GetValue(CaptionMarginProperty);
            }

            set
            {
                SetValue(CaptionMarginProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionMargin"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty CaptionMarginProperty =
      DependencyProperty.Register("CaptionMargin", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(5, 0, 0, 0), OnCaptionMarginPropertyChanged));

        /// <summary>
        /// Called when [caption fore ground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCaptionForeGroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnCaptionForeGroundPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:CaptionForeGroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCaptionForeGroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].CaptionForeGround = newValue;
                }
            }
        }

        /// <summary>
        /// Called when [caption active foreground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnActiveForeGroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnActiveForeGroundPropertyChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnActiveForeGroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null && this.ActiveWindow != null)
            {
                this.ActiveWindow.ActiveForeground = newValue;
            }
        }

        /// <summary>
        /// Called when [caption font size property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCaptionFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnCaptionFontSizePropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:CaptionFontSizePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCaptionFontSizePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            if (newValue != 0)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].CaptionFontSize = newValue;
                }
            }
        }

        /// <summary>
        /// Called when [caption font family property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCaptionFontFamilyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnCaptionFontFamilyPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:CaptionFontFamilyPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCaptionFontFamilyPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            FontFamily newValue = (FontFamily)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].CaptionFontFamily = newValue;
                }
            }
        }

        /// <summary>
        /// Called when [caption margin property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCaptionMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnCaptionMarginPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:CaptionMarginPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCaptionMarginPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Thickness newValue = (Thickness)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].CaptionMargin = newValue;
                }
            }
        }

        /// <summary>
        /// Called when [header border brush property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeaderBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnHeaderBorderBrushPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:HeaderBorderBrushPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnHeaderBorderBrushPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].HeaderBorderBrush = newValue;
                }
            }
        }

        /// <summary>
        /// Called when [window border brush property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowBorderBrushPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowBorderBrushPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowBorderBrushPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].WindowBorderBrush = newValue;
                }
            }
        }

        /// <summary>
        /// Called when [window border thickness property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowBorderThicknessPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowBorderThicknessPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowBorderThicknessPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Thickness newValue = (Thickness)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].WindowBorderThickness = newValue;
                }
            }
        }

        /// <summary>
        /// Called when [window corner radius property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowCornerRadiusPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowCornerRadiusPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowCornerRadiusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            CornerRadius newValue = (CornerRadius)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].WindowCornerRadius = newValue;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the active window.
        /// </summary>
        /// <value>The color of the active window.</value>
        public Brush ActiveWindowColor
        {
            get
            {
                return (Brush)GetValue(ActiveWindowColorProperty);
            }

            set
            {
                SetValue(ActiveWindowColorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ActiveWindowColor"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty ActiveWindowColorProperty =
        DependencyProperty.Register("ActiveWindowColor", typeof(Brush), typeof(DockingManager), new PropertyMetadata(DefaultActiveHeaderbackground()));

        /// <summary>
        /// Gets or sets the color of the active window foreground.
        /// </summary>
        /// <value>The color of the active foreground.</value>
        public Brush ActiveForeground
        {
            get
            {
                return (Brush)GetValue(ActiveForegroundProperty);
            }

            set
            {
                SetValue(ActiveForegroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ActiveForeground"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty ActiveForegroundProperty =
        DependencyProperty.Register("ActiveForeground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnActiveForeGroundPropertyChanged));

        /// <summary>
        /// Gets or sets the color of the pop up.
        /// </summary>
        /// <value>The color of the pop up.</value>
        public Brush PopUpColor
        {
            get
            {
                return (Brush)GetValue(PopUpColorProperty);
            }

            set
            {
                SetValue(PopUpColorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PopUpColor"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty PopUpColorProperty =
        DependencyProperty.Register("PopUpColor", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        /// <summary>
        /// Gets or sets the color of the splitter back ground.
        /// </summary>
        /// <value>The color of the splitter back ground.</value>
        public Brush SplitterBackGroundColor
        {
            get
            {
                return (Brush)GetValue(SplitterBackGroundColorProperty);
            }

            set
            {
                SetValue(SplitterBackGroundColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SplitterBackGroundColor"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty SplitterBackGroundColorProperty =
        DependencyProperty.Register("SplitterBackGroundColor", typeof(Brush), typeof(DockingManager), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the tab item font size selected.
        /// </summary>
        /// <value>The tab item font size selected.</value>
        public double TabItemFontSizeSelected
        {
            get
            {
                return (double)GetValue(TabItemFontSizeSelectedProperty);
            }

            set
            {
                SetValue(TabItemFontSizeSelectedProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="TabItemFontSizeSelected"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty TabItemFontSizeSelectedProperty =
       DependencyProperty.Register("TabItemFontSizeSelected", typeof(double), typeof(DockingManager), new PropertyMetadata(12d, OnTabItemFontSizeSelectedPropertyChanged));

        /// <summary>
        /// Called when [tab item font size selected property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemFontSizeSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemFontSizeSelectedPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemFontSizeSelectedPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemFontSizeSelectedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            if (newValue != 0)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the option button fill.
        /// </summary>
        /// <value>The color of the option button fill.</value>
        public Brush OptionButtonFillColor
        {
            get
            {
                return (Brush)GetValue(OptionButtonFillColorProperty);
            }

            set
            {
                SetValue(OptionButtonFillColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="OptionButtonFillColor"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty OptionButtonFillColorProperty =
      DependencyProperty.Register("OptionButtonFillColor", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnOptionButtonFillColorPropertyChanged));

        /// <summary>
        /// Called when [option button fill color property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOptionButtonFillColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnOptionButtonFillColorPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:OptionButtonFillColorPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnOptionButtonFillColorPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newbrush = (Brush)e.NewValue;
            if (newbrush != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].OptionButtonFillColor = OptionButtonFillColor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the option button mouse hover.
        /// </summary>
        /// <value>The color of the option button mouse hover.</value>
        public Brush OptionButtonMouseHoverColor
        {
            get
            {
                return (Brush)GetValue(OptionButtonMouseHoverColorProperty);
            }

            set
            {
                SetValue(OptionButtonMouseHoverColorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="OptionButtonMouseHoverColor"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OptionButtonMouseHoverColorProperty =
      DependencyProperty.Register("OptionButtonMouseHoverColor", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnOptionButtonMouseHoverColorChanged));

        /// <summary>
        /// Called when [option button mouse hover color changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOptionButtonMouseHoverColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnOptionButtonMouseHoverColorChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:OptionButtonMouseHoverColorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnOptionButtonMouseHoverColorChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newbrush = (Brush)e.NewValue;
            if (newbrush != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].OptionButtonMouseHoverColor = OptionButtonMouseHoverColor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the TabItemBackgroundSelected
        /// TabItemBackgroundSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemBackgroundSelected value for the <see cref="DockingManager"/>. The default value of the TabItemBackgroundSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemBackgroundSelected dependency property defines background of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemBackgroundSelected property please see <see cref="SplitterBackGroundColor"/> property example.
        /// </example>
        public Brush TabItemBackgroundUnSelected
        {
            get
            {
                return (Brush)GetValue(TabItemBackgroundUnSelectedProperty);
            }

            set
            {
                SetValue(TabItemBackgroundUnSelectedProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemBackgroundUnSelected"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemBackgroundUnSelectedProperty =
        DependencyProperty.Register("TabItemBackgroundUnSelected", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xC1, 0xD8, 0xF6)), OnTabItemBackgroundUnSelectedPropertyChanged));

        /// <summary>
        /// Called when [tab item background un selected property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemBackgroundUnSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemBackgroundUnSelectedPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemBackgroundUnSelectedPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemBackgroundUnSelectedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the TabItemForeGroundSelected
        /// TabItemForegroundUnSelected property is used to store background value for selected
        /// tab control item of the dock window.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Provides TabItemForegroundUnSelected value for the <see cref="DockingManager"/>. The default value of the TabItemForegroundUnSelected property depends on selected theme or skin.
        /// </value>
        /// <remarks>
        /// TabItemForegroundUnSelected dependency property defines foreground of the selected tab item. 
        /// </remarks>
        /// <example>
        /// To set TabItemForegroundUnSelected property please see <see cref="TabItemForegroundUnSelected"/> property example.
        /// </example>
        public Brush TabItemForegroundUnSelected
        {
            get
            {
                return (Brush)GetValue(TabItemForegroundUnSelectedProperty);
            }

            set
            {
                SetValue(TabItemForegroundUnSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab item font size un selected.
        /// </summary>
        /// <value>The tab item font size un selected.</value>
        public double TabItemFontSizeUnSelected
        {
            get
            {
                return (double)GetValue(TabItemFontSizeUnSelectedProperty);
            }

            set
            {
                SetValue(TabItemFontSizeUnSelectedProperty, value);
            }
        }

        /// <summary>
        /// Called when [tab item foreground un selected changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemForegroundUnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemForegroundUnSelectedChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemForegroundUnSelectedChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemForegroundUnSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [tab item font size un selected property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemFontSizeUnSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemFontSizeUnSelectedPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemFontSizeUnSelectedPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemFontSizeUnSelectedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;
            if (newValue != 0)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemFontSizeUnSelected"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemFontSizeUnSelectedProperty =
       DependencyProperty.Register("TabItemFontSizeUnSelected", typeof(double), typeof(DockingManager), new PropertyMetadata(12d, OnTabItemFontSizeUnSelectedPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="TabItemForegroundUnSelected"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemForegroundUnSelectedProperty =
       DependencyProperty.Register("TabItemForegroundUnSelected", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnTabItemForegroundUnSelectedChanged));

        /// <summary>
        /// Gets or sets the tab item inner border thickness.
        /// </summary>
        /// <value>The tab item inner border thickness.</value>
        public Thickness TabItemInnerBorderThickness
        {
            get
            {
                return (Thickness)GetValue(TabItemInnerBorderThicknessProperty);
            }

            set
            {
                SetValue(TabItemInnerBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemInnerBorderThickness"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemInnerBorderThicknessProperty =
      DependencyProperty.Register("TabItemInnerBorderThickness", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(0), OnTabItemInnerBorderThicknessPropertyChanged));

        /// <summary>
        /// Called when [tab item inner border thickness property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemInnerBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemInnerBorderThicknessPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemInnerBorderThicknessPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemInnerBorderThicknessPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].CustomTabControl != null)
                {
                    for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                    {
                        ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab item outer border thickness.
        /// </summary>
        /// <value>The tab item outer border thickness.</value>
        public Thickness TabItemOuterBorderThickness
        {
            get
            {
                return (Thickness)GetValue(TabItemOuterBorderThicknessProperty);
            }

            set
            {
                SetValue(TabItemOuterBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemOuterBorderThickness"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemOuterBorderThicknessProperty =
      DependencyProperty.Register("TabItemOuterBorderThickness", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(1), OnTabItemOuterBorderThicknessPropertyChanged));

        /// <summary>
        /// Called when [tab item outer border thickness property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemOuterBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemOuterBorderThicknessPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemOuterBorderThicknessPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemOuterBorderThicknessPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].CustomTabControl != null)
                {
                    for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                    {
                        ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab item inner border brush.
        /// </summary>
        /// <value>The tab item inner border brush.</value>
        public Brush TabItemInnerBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabItemInnerBorderBrushProperty);
            }

            set
            {
                SetValue(TabItemInnerBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemInnerBorderBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemInnerBorderBrushProperty =
        DependencyProperty.Register("TabItemInnerBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnTabItemInnerBorderBrushPropertyChanged));

        /// <summary>
        /// Called when [tab item inner border brush property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemInnerBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemInnerBorderBrushPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemInnerBorderBrushPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemInnerBorderBrushPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab item outer border brush.
        /// </summary>
        /// <value>The tab item outer border brush.</value>
        public Brush TabItemOuterBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabItemOuterBorderBrushProperty);
            }

            set
            {
                SetValue(TabItemOuterBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemOuterBorderBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemOuterBorderBrushProperty =
        DependencyProperty.Register("TabItemOuterBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xA3, 0xAE, 0xB9)), OnTabItemOuterBorderBrushPropertyChanged));

        /// <summary>
        /// Called when [tab item outer border brush property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabItemOuterBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemOuterBorderBrushPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemOuterBorderBrushPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemOuterBorderBrushPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab panel background.
        /// </summary>
        /// <value>The tab panel background.</value>
        public Brush TabPanelBackground
        {
            get
            {
                return (Brush)GetValue(TabPanelBackgroundProperty);
            }

            set
            {
                SetValue(TabPanelBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabPanelBackground"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabPanelBackgroundProperty =
      DependencyProperty.Register("TabPanelBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xCA, 0xDE, 0xF7)), OnTabPanelBackgroundPropertyChanged));

        /// <summary>
        /// Called when [tab panel background property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabPanelBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabPanelBackgroundPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:TabPanelBackgroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabPanelBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                        {
                            ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the side items background.
        /// </summary>
        /// <value>The side items background.</value>
        public Brush SideItemsBackground
        {
            get
            {
                return (Brush)GetValue(SideItemsBackgroundProperty);
            }

            set
            {
                SetValue(SideItemsBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SideItemsBackground"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SideItemsBackgroundProperty =
          DependencyProperty.Register("SideItemsBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata((new SolidColorBrush(Color.FromArgb(0xFF, 0xC1, 0xD8, 0xF6))), OnSideItemsBackgroundPropertyChanged));

        /// <summary>
        /// Called when [side items background property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSideItemsBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnSideItemsBackgroundPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SideItemsBackgroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSideItemsBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                ApplySideButtonStyle();
            }
        }

        /// <summary>
        /// Gets or sets the side items border brush.
        /// </summary>
        /// <value>The side items border brush.</value>
        public Brush SideItemsBorderBrush
        {
            get
            {
                return (Brush)GetValue(SideItemsBorderBrushProperty);
            }

            set
            {
                SetValue(SideItemsBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SideItemsBorderBrush"/> Dependency PRoperty.
        /// </summary>
        public static readonly DependencyProperty SideItemsBorderBrushProperty =
          DependencyProperty.Register("SideItemsBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Gray), OnSideItemsBorderBrushPropertyChanged));

        /// <summary>
        /// Gets or sets the tab items mouse hover brush.
        /// </summary>
        /// <value>The tab items mouse hover brush.</value>
        public Brush TabItemsMouseHoverBrush
        {
            get
            {
                return (Brush)GetValue(TabItemsMouseHoverBrushProperty);
            }

            set
            {
                SetValue(TabItemsMouseHoverBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemsMouseHoverBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemsMouseHoverBrushProperty =
          DependencyProperty.Register("TabItemsMouseHoverBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        /// <summary>
        /// Gets or sets the tab items mouse hover border brush.
        /// </summary>
        /// <value>The tab items mouse hover border brush.</value>
        public Brush TabItemMouseHoverBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabItemMouseHoverBorderBrushProperty);
            }

            set
            {
                SetValue(TabItemMouseHoverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemMouseHoverBorderBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemMouseHoverBorderBrushProperty =
          DependencyProperty.Register("TabItemMouseHoverBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        /// <summary>
        /// Gets or sets the tab items mouse out brush.
        /// </summary>
        /// <value>The tab items mouse out brush.</value>
        public Brush TabItemsMouseOutBrush
        {
            get
            {
                return (Brush)GetValue(TabItemsMouseOutBrushProperty);
            }

            set
            {
                SetValue(TabItemsMouseOutBrushProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TabItemsMouseOutBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabItemsMouseOutBrushProperty =
          DependencyProperty.Register("TabItemsMouseOutBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Called when [side items border brush property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSideItemsBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnSideItemsBorderBrushPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SideItemsBorderBrushPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSideItemsBorderBrushPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                ApplySideButtonStyle();
            }
        }

        /// <summary>
        /// Gets or sets the side items border thickness.
        /// </summary>
        /// <value>The side items border thickness.</value>
        public Thickness SideItemsBorderThickness
        {
            get
            {
                return (Thickness)GetValue(SideItemsBorderThicknessProperty);
            }

            set
            {
                SetValue(SideItemsBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SideItemsBorderThickness"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SideItemsBorderThicknessProperty =
          DependencyProperty.Register("SideItemsBorderThickness", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(1), OnSideItemsBorderThicknessPropertyChanged));

        /// <summary>
        /// Called when [side items border thickness property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSideItemsBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnSideItemsBorderThicknessPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SideItemsBorderThicknessPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSideItemsBorderThicknessPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Thickness newValue = (Thickness)e.NewValue;
            ApplySideButtonStyle();
        }

        /// <summary>
        /// Gets or sets the side items foreground.
        /// </summary>
        /// <value>The side items foreground.</value>
        public Brush SideItemsForeground
        {
            get
            {
                return (Brush)GetValue(SideItemsForegroundProperty);
            }

            set
            {
                SetValue(SideItemsForegroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SideItemsForeground"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SideItemsForegroundProperty =
          DependencyProperty.Register("SideItemsForeground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnSideItemsForegroundPropertyChanged));

        /// <summary>
        /// Gets or sets the float window header background.
        /// </summary>
        /// <value>The float window header background.</value>
        public Brush FloatWindowHeaderBackground
        {
            get
            {
                return (Brush)GetValue(FloatWindowHeaderBackgroundProperty);
            }

            set
            {
                SetValue(FloatWindowHeaderBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="FloatWindowHeaderBackground"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowHeaderBackgroundProperty =
          DependencyProperty.Register("FloatWindowHeaderBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(DefaultFloatHeaderbackground()));

        /// <summary>
        /// Gets or sets the float window border brush.
        /// </summary>
        /// <value>The float window border brush.</value>
        public Brush FloatWindowBorderBrush
        {
            get
            {
                return (Brush)GetValue(FloatWindowBorderBrushProperty);
            }

            set
            {
                SetValue(FloatWindowBorderBrushProperty, value);
            }
        }


        /// <summary>
        /// Identifies the <see cref="FloatWindowBorderBrush"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowBorderBrushProperty =
          DependencyProperty.Register("FloatWindowBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x96, 0xC8, 0xF2))));

        /// <summary>
        /// Gets or sets the float window active header background.
        /// </summary>
        /// <value>The float window active header background.</value>
        public Brush FloatWindowActiveHeaderBackground
        {
            get
            {
                return (Brush)GetValue(FloatWindowActiveHeaderBackgroundProperty);
            }

            set
            {
                SetValue(FloatWindowActiveHeaderBackgroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowActiveHeaderBackground"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowActiveHeaderBackgroundProperty =
          DependencyProperty.Register("FloatWindowActiveHeaderBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(DefaultFloataActiveHeaderbackground()));

        /// <summary>
        /// Gets or sets the float window active border brush.
        /// </summary>
        /// <value>The float window active border brush.</value>
        public Brush FloatWindowActiveBorderBrush
        {
            get
            {
                return (Brush)GetValue(FloatWindowActiveBorderBrushProperty);
            }

            set
            {
                SetValue(FloatWindowActiveBorderBrushProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowActiveBorderBrush"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowActiveBorderBrushProperty =
          DependencyProperty.Register("FloatWindowActiveBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x96, 0xC8, 0xF2))));

        /// <summary>
        /// Gets or sets the float window border thickness.
        /// </summary>
        /// <value>The float window border thickness.</value>
        public Thickness FloatWindowBorderThickness
        {
            get
            {
                return (Thickness)GetValue(FloatWindowBorderThicknessProperty);
            }

            set
            {
                SetValue(FloatWindowBorderThicknessProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowBorderThickness"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowBorderThicknessProperty =
          DependencyProperty.Register("FloatWindowBorderThickness", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(2), OnFloatWindowBorderThicknessPropertyChanged));



        /// <summary>
        /// Called when [float window border thickness property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatWindowBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnFloatWindowBorderThicknessPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:FloatWindowBorderThicknessPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFloatWindowBorderThicknessPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (base.Children.Contains(this.WindowCollection[i]) || this.WindowCollection[i].DockState == DockState.Float)
                {
                    ApplyStyleFloatWindow(this.WindowCollection[i]);
                }
                //if (this.WindowCollection[i].CustomTabControl != null)
                //{
                //    for (int m = 0; m < this.WindowCollection[i].CustomTabControl.Items.Count; m++)
                //    {
                //        ApplyStyle((CustomTabItem)this.WindowCollection[i].CustomTabControl.Items[m]);
                //    }
                //}
            }
        }



        /// <summary>
        /// Gets or sets the float window corner radius.
        /// </summary>
        /// <value>The float window corner radius.</value>
        public CornerRadius FloatWindowCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(FloatWindowCornerRadiusProperty);
            }

            set
            {
                SetValue(FloatWindowCornerRadiusProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowCornerRadius"/> Dependecy Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowCornerRadiusProperty =
          DependencyProperty.Register("FloatWindowCornerRadius", typeof(CornerRadius), typeof(DockingManager), new PropertyMetadata(new CornerRadius(1), OnFloatWindowCornerRadiusPropertyChanged));



        /// <summary>
        /// Called when [float window corner radius property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatWindowCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnFloatWindowCornerRadiusPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:FloatWindowCornerRadiusPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFloatWindowCornerRadiusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (base.Children.Contains(this.WindowCollection[i]) || this.WindowCollection[i].DockState == DockState.Float)
                {
                    ApplyStyleFloatWindow(this.WindowCollection[i]);
                }
            }
        }



        /// <summary>
        /// Called when [side items foreground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSideItemsForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnSideItemsForegroundPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SideItemsForegroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSideItemsForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                ApplySideButtonStyle();
            }
        }

        /// <summary>
        /// Gets or sets the context list box style.
        /// </summary>
        /// <value>The context list box style.</value>
        private Style ContextListBoxStyle
        {
            get
            {
                return (Style)GetValue(ContextListBoxStyleProperty);
            }

            set
            {
                SetValue(ContextListBoxStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the context list item style.
        /// </summary>
        /// <value>The context list item style.</value>
        private Style ContextListItemStyle
        {
            get
            {
                return (Style)GetValue(ContextListItemStyleProperty);
            }

            set
            {
                SetValue(ContextListItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the close button template.
        /// </summary>
        /// <value>The close button template.</value>
        public Style CloseButtonTemplate
        {
            get
            {
                return (Style)GetValue(CloseButtonTemplateProperty);
            }

            set
            {
                SetValue(CloseButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a control template for menu button. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides MenuButtonTemplate value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize control template of the menu button and set MenuButtonTemplate property in XAML like you set <see cref="CloseButtonTemplate"/> property.
        /// </example>
        public Style MenuButtonTemplate
        {
            get
            {
                return (Style)GetValue(MenuButtonTemplateProperty);
            }

            set
            {
                SetValue(MenuButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets  the style for maximize button template
        /// </summary>
        public Style MaximizeButtonTemplate
        {
            get
            {
                return (Style)GetValue(MaximizeButtonTemplateProperty);
            }
            set
            {
                SetValue(MaximizeButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the context menu style
        /// </summary>
        public Style ContextMenuStyle
        {
            get
            {
                return (Style)GetValue(ContextMenuStyleProperty);
            }
            set
            {
                SetValue(ContextMenuStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the ContextMenuItem Style
        /// </summary>
        public Style ContextMenuItemStyle
        {
            get
            {
                return (Style)GetValue(ContextMenuItemStyleProperty);
            }
            set
            {
                SetValue(ContextMenuItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a control template for autohide button. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Provides AwlButtonTemplate value for the <see cref="DockingManager"/>.
        /// </value>
        /// <example>
        /// You can initialize control template of the auto-hide button and set AwlButtonTemplate property in XAML like you set <see cref="CloseButtonTemplate"/> property.
        /// </example>
        public Style AwlButtonTemplate
        {
            get
            {
                return (Style)GetValue(AwlButtonTemplateProperty);
            }

            set
            {
                SetValue(AwlButtonTemplateProperty, value);
            }
        }

        ///// <summary>
        ///// Gets or sets the theme.
        ///// </summary>
        ///// <value>The theme.</value>
        //internal Theme Theme
        //{
        //    get
        //    {
        //        return (Theme)GetValue(ThemeProperty);
        //    }

        //    set
        //    {
        //        SetValue(ThemeProperty, value);
        //    }
        //}
        ///// <summary>
        ///// Identifies the <see cref="Theme"/> Dependecy Property.
        ///// </summary>
        //internal static readonly DependencyProperty ThemeProperty =
        //   DependencyProperty.Register("Theme", typeof(Theme), typeof(DockingManager), new PropertyMetadata(Theme.Default, OnThemePropertyChanged));

        ///// <summary>
        ///// Called when [theme property changed].
        ///// </summary>
        ///// <param name="d">The d.</param>
        ///// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        //private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    DockingManager instance = (DockingManager)d;
        //    instance.OnThemePropertyChanged(e);
        //}

        ///// <summary>
        ///// Called when [apply theme].
        ///// </summary>
        ///// <param name="theme">The theme.</param>
        //protected internal void OnApplyTheme(Theme theme)
        //{
        //    string key = string.Empty;
        //    //VisualStyle visualStyle = VisualStyle.Office2007Blue;
        //    //switch (theme)
        //    //{
        //    //    case Theme.Default:
        //    //        key = "DockingManagerDefault";
        //    //        visualStyle = VisualStyle.Default;
        //    //        break;

        //    //    case Theme.Blend:
        //    //        key = "DockingManagerBlend";
        //    //        visualStyle = VisualStyle.Blend;
        //    //        break;

        //    //    case Theme.Office2007Black:
        //    //        key = "DockingManagerBlack";
        //    //        visualStyle = VisualStyle.Office2007Black;
        //    //        break;

        //    //    case Theme.Office2007Blue:
        //    //        key = "DockingManagerBlue";
        //    //        visualStyle = VisualStyle.Office2007Blue;
        //    //        break;

        //    //    case Theme.Office2007Silver:
        //    //        key = "DockingManagerSilver";
        //    //        visualStyle = VisualStyle.Office2007Silver;
        //    //        break;
        //    //}

        //    if (ResourceDictionary != null)
        //    {
        //        Dictionary<string, object> r_coll = ResourceDictionary.ResourceCollection;
        //        if (r_coll.ContainsKey("DockingManagerSkins"))
        //        {
        //            // this.Style = (Style)r_coll[key];
        //            //SkinList sl = (SkinList)r_coll["DockingManagerSkins"];
        //            //SkinManager.SetSkinList(this, sl);
        //            //SkinManager.SetVisualStyle(this, visualStyle);
        //            //SkinManager.ApplyStyle(this, visualStyle);
        //        }
        //    }
        //    DockingGrid dockingGrid = GetParentDockManager();
        //    DockManager dm = null;
        //    if (dockingGrid != null && dockingGrid.Parent != null)
        //    {
        //        dm = dockingGrid.Parent as DockManager;
        //    }

        //    if (ActiveWindow != null)
        //    {
        //        if (ActiveWindow.DockState == DockState.Float || ActiveWindow.DockState == DockState.Hidden)
        //        {
        //            ActiveWindow.HeaderBackgroud = FloatWindowActiveHeaderBackground;
        //            ActiveWindow.WindowBorderBrush = FloatWindowActiveBorderBrush;
        //        }
        //        else
        //        {
        //            ActiveWindow.HeaderBackgroud = ActiveWindowColor;
        //            ActiveWindow.WindowBorderBrush = WindowBorderBrush;
        //        }
        //    }

        //    ApplythemeStyle();
        //    if (dm != null)
        //    {
        //        if (!DockFill)
        //        {
        //            DockingGrid gr = dockingGrid;
        //            gr.rootWindow.Background = DocumentBackGround;
        //            gr.rootWindow.WindowBorderBrush = DocumentBorderBrush;
        //            Thickness tempThcikness = WindowBorderThickness;
        //            tempThcikness.Top = tempThcikness.Top + 1;
        //            gr.rootWindow.WindowBorderThickness = tempThcikness;

        //            if (gr.rootWindow.ContentGrid != null)
        //            {
        //                (gr.rootWindow.ContentGrid.Parent as Border).Background = DocumentBackGround;
        //                ((gr.rootWindow.ContentGrid.Parent as Border).Parent as Border).Background = DocumentBackGround;
        //                gr.rootWindow.contentpresenter.Background = DocumentBackGround;
        //                clientGrid.Background = DocumentBackGround;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Raises the <see cref="E:ThemePropertyChanged"/> event.
        ///// </summary>
        ///// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        //protected virtual void OnThemePropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    Theme theme = (Theme)e.NewValue;
        //    OnApplyTheme(theme);
        //}

        /// <summary>
        /// Applythemes the style.
        /// </summary>
        protected internal void ApplythemeStyle()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].DockState == DockState.Dock || this.WindowCollection[i].DockState == DockState.AutoHidden)
                {
                }
                else if (this.WindowCollection[i].DockState == DockState.Float || this.WindowCollection[i].DockState == DockState.Hidden)
                {
                    if (this.WindowCollection[i] != ActiveWindow)
                    {
                        this.WindowCollection[i].HeaderBackgroud = FloatWindowHeaderBackground;
                        this.WindowCollection[i].WindowBorderBrush = FloatWindowBorderBrush;
                    }
                    else
                    {
                        ActiveWindow.HeaderBackgroud = FloatWindowActiveHeaderBackground;
                        ActiveWindow.WindowBorderBrush = FloatWindowActiveBorderBrush;
                    }

                    this.WindowCollection[i].WindowBorderThickness = FloatWindowBorderThickness;
                    this.WindowCollection[i].WindowCornerRadius = FloatWindowCornerRadius;
                    this.WindowCollection[i].WindowBackGround = FloatWindowBackground;
                    if (this.WindowCollection[i].CustomTabControl != null && this.WindowCollection[i].CustomTabControl.Items.Count == 1)
                    {
                        this.WindowCollection[i].CustomTabControl.WindowContentBackground = FloatWindowContentBackground;
                        this.WindowCollection[i].CustomTabControl.WindowContentMargin = FloatWindowContentMargin;
                        this.WindowCollection[i].CustomTabControl.WindowContentBorderBrush = FloatWindowContentBorderBrush;
                        this.WindowCollection[i].CustomTabControl.WindowContentBorderThickness = FloatWindowContentBorderThickness;
                        this.WindowCollection[i].CustomTabControl.WindowBackground = FloatWindowBackground;
                    }
                }
                this.WindowCollection[i].ApplyBorderForFloatWindow();
            }
            foreach (UIElement ele in base.Children)
            {
                if (ele is Window)
                {
                    if ((ele as Window)._Caption == string.Empty)
                    {
                        ((ele) as Window).HeaderBackgroud = FloatWindowHeaderBackground;
                        ((ele) as Window).HeaderBorderBrush = new SolidColorBrush(Colors.Transparent);
                        ((ele) as Window).Background = WindowBackground;
                        if (((ele) as Window).WindowContainer != null)
                        {
                            ((ele) as Window).WindowContainer.Background = WindowBackground;
                        }
                        ((ele) as Window).WindowBackGround = WindowBackground;
                        ((ele) as Window).WindowBorderBrush = WindowBorderBrush;

                        if (((ele) as Window).closeButton != null)
                        {
                            ((ele) as Window).closeButton.Style = CloseButtonTemplate;
                        }
                        if (((ele) as Window).optionsButton != null)
                        {
                            ((ele) as Window).optionsButton.Style = MenuButtonTemplate;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies the CloseButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ContextListBoxStyleProperty =
            DependencyProperty.Register("ContextListBoxStyle", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnContextListBoxStylePropertyChanged));

        /// <summary>
        /// Identifies the CloseButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ContextListItemStyleProperty =
            DependencyProperty.Register("ContextListItemStyle", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnContextListItemStylePropertyChanged));

        /// <summary>
        /// Identifies the CloseButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty CloseButtonTemplateProperty =
            DependencyProperty.Register("CloseButtonTemplate", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnCloseButtonTemplatePropertyChanged));

        /// <summary>
        /// Identifies the MenuButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty MenuButtonTemplateProperty =
            DependencyProperty.Register("MenuButtonTemplate", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnMenuButtonTemplatePropertyChanged));

        /// <summary>
        /// Identifies the AwlButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty AwlButtonTemplateProperty =
            DependencyProperty.Register("AwlButtonTemplate", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnAwlButtonTemplatePropertyChanged));

        /// <summary>
        /// Identifies the MaximizeButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonTemplateProperty =
            DependencyProperty.Register("MaximizeButtonTemplate", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnMaximizeButtonTemplatePropertyChanged));

        /// <summary>
        /// Identifies the ContextMenuStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ContextMenuStyleProperty =
            DependencyProperty.Register("ContextMenuStyle", typeof(Style), typeof(DockingManager), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ContextMenuItemStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ContextMenuItemStyleProperty =
            DependencyProperty.Register("ContextMenuItemStyle", typeof(Style), typeof(DockingManager), new PropertyMetadata(null));

        /// <summary>
        /// Called when [close button template property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCloseButtonTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnCloseButtonTemplatePropertyChanged(e);
        }

        /// <summary>
        /// Called when [context list box style property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContextListBoxStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnContextListBoxStylePropertyChanged(e);
        }

        /// <summary>
        /// Called when [context list item style property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContextListItemStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnContextListItemStylePropertyChanged(e);
        }

        /// <summary>
        /// Gets or sets the window content background.
        /// </summary>
        /// <value>The window content background.</value>
        public Brush WindowContentBackground
        {
            get
            {
                return (Brush)GetValue(WindowContentBackgroundProperty);
            }

            set
            {
                SetValue(WindowContentBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentBackground"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty WindowContentBackgroundProperty =
      DependencyProperty.Register("WindowContentBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnWindowContentBackgroundPropertyChanged));

        /// <summary>
        /// Gets or sets the float window content background.
        /// </summary>
        /// <value>The float window content background.</value>
        public Brush FloatWindowContentBackground
        {
            get
            {
                return (Brush)GetValue(FloatWindowContentBackgroundProperty);
            }

            set
            {
                SetValue(FloatWindowContentBackgroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowContentBackground"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowContentBackgroundProperty =
      DependencyProperty.Register("FloatWindowContentBackground", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Called when [window content background property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowContentBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowContentBackgroundPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowContentBackgroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowContentBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].CustomTabControl.WindowContentBackground = newValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the window content margin.
        /// </summary>
        /// <value>The window content margin.</value>
        public Thickness WindowContentMargin
        {
            get
            {
                return (Thickness)GetValue(WindowContentMarginProperty);
            }

            set
            {
                SetValue(WindowContentMarginProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="WindowContentMargin"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty WindowContentMarginProperty =
      DependencyProperty.Register("WindowContentMargin", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(0), OnWindowContentMarginPropertyChanged));

        /// <summary>
        /// Called when [window content margin property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowContentMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowContentMarginPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowContentMarginPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowContentMarginPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Thickness newValue = (Thickness)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].CustomTabControl.WindowContentMargin = newValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the window content border brush.
        /// </summary>
        /// <value>The window content border brush.</value>
        public Brush WindowContentBorderBrush
        {
            get
            {
                return (Brush)GetValue(WindowContentBorderBrushProperty);
            }

            set
            {
                SetValue(WindowContentBorderBrushProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="WindowContentMargin"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty WindowContentBorderBrushProperty =
      DependencyProperty.Register("WindowContentBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnWindowContentBorderBrushPropertyChanged));

        /// <summary>
        /// Called when [window content border brush property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowContentBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowContentBorderBrushPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowContentBorderBrushPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowContentBorderBrushPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].CustomTabControl.WindowContentBorderBrush = newValue;
                        if (this.WindowCollection[i].CustomTabControl.TabPanelBorder != null)
                        {
                            this.WindowCollection[i].CustomTabControl.TabPanelBorder.BorderBrush = newValue;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Gets or sets the window content conrner radius.
        /// </summary>
        /// <value>The window content conrner radius.</value>
        public CornerRadius WindowContentConrnerRadius
        {
            get
            {
                return (CornerRadius)GetValue(WindowContentConrnerRadiusProperty);
            }

            set
            {
                SetValue(WindowContentConrnerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentConrnerRadius"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty WindowContentConrnerRadiusProperty =
      DependencyProperty.Register("WindowContentConrnerRadius", typeof(CornerRadius), typeof(DockingManager), new PropertyMetadata(new CornerRadius(0), OnWindowContentConrnerRadiusPropertyChanged));

        /// <summary>
        /// Called when [window content conrner radius property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowContentConrnerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowContentConrnerRadiusPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowContentConrnerRadiusPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowContentConrnerRadiusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            CornerRadius newValue = (CornerRadius)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].CustomTabControl.WindowContentConrnerRadius = newValue;
                    }
                }
            }
        }












        /// <summary>
        /// Gets or sets the window content border thickness.
        /// </summary>
        /// <value>The window content border thickness.</value>
        public Thickness WindowContentBorderThickness
        {
            get
            {
                return (Thickness)GetValue(WindowContentBorderThicknessProperty);
            }

            set
            {
                SetValue(WindowContentBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WindowContentBorderThickness"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty WindowContentBorderThicknessProperty =
      DependencyProperty.Register("WindowContentBorderThickness", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(0, 1, 0, 1), OnWindowContentBorderThicknessPropertyChanaged));

        /// <summary>
        /// Called when [window content border thickness property chanaged].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowContentBorderThicknessPropertyChanaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowContentBorderThicknessPropertyChanaged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowContentBorderThicknessPropertyChanaged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowContentBorderThicknessPropertyChanaged(DependencyPropertyChangedEventArgs e)
        {
            Thickness newValue = (Thickness)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].CustomTabControl != null)
                    {
                        this.WindowCollection[i].CustomTabControl.WindowContentBorderThickness = newValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the float window content margin.
        /// </summary>
        /// <value>The float window content margin.</value>
        public Thickness FloatWindowContentMargin
        {
            get
            {
                return (Thickness)GetValue(FloatWindowContentMarginProperty);
            }

            set
            {
                SetValue(FloatWindowContentMarginProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="FloatWindowContentMargin"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowContentMarginProperty =
      DependencyProperty.Register("FloatWindowContentMargin", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(0), OnFloatWindowContentMarginPropertyChanged));


        /// <summary>
        /// Called when [float window content margin property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatWindowContentMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnFloatWindowContentMarginPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:FloatWindowContentMarginPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFloatWindowContentMarginPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (base.Children.Contains(this.WindowCollection[i]) || this.WindowCollection[i].DockState == DockState.Float)
                {
                    ApplyStyleFloatWindow(this.WindowCollection[i]);
                }
            }
        }


        /// <summary>
        /// Gets or sets the float window content border brush.
        /// </summary>
        /// <value>The float window content border brush.</value>
        public Brush FloatWindowContentBorderBrush
        {
            get
            {
                return (Brush)GetValue(FloatWindowContentBorderBrushProperty);
            }

            set
            {
                SetValue(FloatWindowContentBorderBrushProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowContentBorderBrush"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowContentBorderBrushProperty =
      DependencyProperty.Register("FloatWindowContentBorderBrush", typeof(Brush), typeof(DockingManager), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the float window content border thickness.
        /// </summary>
        /// <value>The float window content border thickness.</value>
        public Thickness FloatWindowContentBorderThickness
        {
            get
            {
                return (Thickness)GetValue(FloatWindowContentBorderThicknessProperty);
            }

            set
            {
                SetValue(FloatWindowContentBorderThicknessProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="FloatWindowContentBorderThickness"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty FloatWindowContentBorderThicknessProperty =
      DependencyProperty.Register("FloatWindowContentBorderThickness", typeof(Thickness), typeof(DockingManager), new PropertyMetadata(new Thickness(1), OnFloatWindowContentBorderThicknessPropertyChanged));


        /// <summary>
        /// Called when [float window content border thickness property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFloatWindowContentBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnFloatWindowContentBorderThicknessPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:FloatWindowContentBorderThicknessPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFloatWindowContentBorderThicknessPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (base.Children.Contains(this.WindowCollection[i]) || this.WindowCollection[i].DockState == DockState.Float)
                {
                    ApplyStyleFloatWindow(this.WindowCollection[i]);
                }
            }
        }

        /// <summary>
        /// Gets or sets the window fore ground.
        /// </summary>
        /// <value>The window fore ground.</value>
        public Brush WindowForeGround
        {
            get
            {
                return (Brush)GetValue(WindowForeGroundProperty);
            }

            set
            {
                SetValue(WindowForeGroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="WindowForeGround"/> Dependency Property.
        /// </summary>
        public static readonly DependencyProperty WindowForeGroundProperty =
          DependencyProperty.Register("WindowForeGround", typeof(Brush), typeof(DockingManager), new PropertyMetadata(null, OnWindowForeGroundPropertyChanged));

        /// <summary>
        /// Called when [window fore ground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowForeGroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnWindowForeGroundPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:WindowForeGroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWindowForeGroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newValue = (Brush)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    this.WindowCollection[i].Foreground = newValue;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ContextListBoxStylePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnContextListBoxStylePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    ////    if (this.WindowCollection[i].DockState == DockState.Dock)
                    ////    {
                    this.WindowCollection[i].contextListBoxStyle = newValue;
                    //// }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ContextListItemStylePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnContextListItemStylePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    ////    if (this.WindowCollection[i].DockState == DockState.Dock)
                    ////    {
                    this.WindowCollection[i].contextListItemStyle = newValue;
                    //// }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CloseButtonTemplatePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCloseButtonTemplatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].closeButton != null)
                    {
                        this.WindowCollection[i].closeButton.Style = newValue;
                        //this.WindowCollection[i].CloseButtonTemplate = newValue;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [menu button template property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMenuButtonTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnMenuButtonTemplatePropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MenuButtonTemplatePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMenuButtonTemplatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    ////this.WindowCollection[i].optionsButton.Style = null;
                    if (this.WindowCollection[i].optionsButton != null)
                    {
                        this.WindowCollection[i].optionsButton.Style = newValue;
                        //this.WindowCollection[i].MenuButtonTemplate = newValue;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [OnMaximizeButtonTemplate Property Changedchanged].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMaximizeButtonTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnMaximizeButtonTemplatePropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MenuButtonTemplatePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMaximizeButtonTemplatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].maximizeButton != null)
                    {
                        this.WindowCollection[i].maximizeButton.Style = newValue;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [awl button template property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAwlButtonTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnAwlButtonTemplatePropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:AwlButtonTemplatePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAwlButtonTemplatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Style newValue = (Style)e.NewValue;
            if (newValue != null)
            {
                for (int i = 1; i <= this.WindowCollection.Count; i++)
                {
                    if (this.WindowCollection[i].dockToggle != null)
                    {
                        this.WindowCollection[i].dockToggle.Style = newValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the items collection.
        /// </summary>
        /// <value>The items collection.</value>
        protected internal ItemsCollection ItemsCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> Dependency Property.
        /// </summary>
        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
           typeof(DockingManager), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
        /// <summary>
        /// Called when [items source changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnItemsSourcePropertyChanged(e);
        }
        /// <summary>
        /// Raises the <see cref="E:ItemsSourcePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemsSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ItemsCollection.ItemsSource = ItemsSource;//then u can procedd...
                for (int i = 0; i < ItemsCollection.Items.Count; i++)
                {
                    base.Children.Add((UIElement)ItemsCollection.Items[i]);
                }
            }
            catch (Exception)
            {
                throw new ArgumentNullException("element");
            }
        }

        /// <summary>
        /// Gets or sets the children count.
        /// </summary>
        /// <value>The children count.</value>
        protected internal int ChildrenCount
        {
            get
            {
                return (int)GetValue(ChildrenCountProperty);
            }

            set
            {
                SetValue(ChildrenCountProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ChildrenCount"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty ChildrenCountProperty =
               DependencyProperty.Register("ChildrenCount", typeof(int), typeof(DockingManager), new PropertyMetadata(OnChildrenCountChanged));

        /// <summary>
        /// Called when [children count changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChildrenCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnChildrenCountChanged(e);
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(UIElement element)
        {
            if (element != null)
            {
                if (!base.Children.Contains(element))
                {
                    base.Children.Add(element);
                }
            }
            if (elemCollection.Count == 0 && _loaded)//&& element== null)
            {
                foreach (UIElement elem in base.Children)
                {
                    if (elem.GetType() != typeof(Window) && elem.GetType() != typeof(DockManager) && elem.GetType() != typeof(Popup))
                    {
                        if (elem == m_leftSideGrid)
                        {
                            continue;
                        }
                        else if (elem == m_rightSideGrid)
                        {
                            continue;
                        }
                        else if (elem == m_topSideGrid)
                        {
                            continue;
                        }
                        else if (elem == m_bottomSideGrid)
                        {
                            continue;
                        }
                        else
                        {
                            if (((FrameworkElement)elem).Parent is DockingManager && (Dock)GetSideInDockedMode(elem) != Dock.None)
                            {
                                if (!elemCollection.Contains(elem))
                                    elemCollection.Add(elem);
                            }
                            else if (elem.GetType() != typeof(Window) && (Dock)GetSideInDockedMode(elem) == Dock.None)
                            {
                                ClientElementCollection.Add(ClientElementCollection.Count() + 1, elem);

                            }
                        }
                    }

                }
                if (element != null)
                {
                    if (element.GetType() != typeof(Window) && (Dock)GetSideInDockedMode(element) == Dock.None)
                    {
                        if (base.Children.Contains(element))
                        {
                            base.Children.Remove(element);
                        }
                    }
                    if (clientGrid.Children.Count > 0 && !clientGrid.Children.Contains(element) && ((FrameworkElement)element).Parent == null && clientGrid != null)
                    {
                        clientGrid.Children.Add(element);
                    }
                }
                bool alreadyExisting = false;
                foreach (UIElement elem in elemCollection)
                {
                    if (((FrameworkElement)elem).Parent is DockingManager && (Dock)GetSideInDockedMode(elem) != Dock.None)
                    {
                        if (elem.GetType() != typeof(Window) && (Dock)GetSideInDockedMode(elem) != Dock.None)
                        {
                            if (GetWindow(DockingManager.GetWindowName(elem)) == null)
                            {
                                Window w = new Window();
                                tempkey = this.WindowCollection.Count() + 1;
                                if (!externalWindow.Contains(w))
                                {
                                    externalWindow.Add(w);
                                }
                                w.DockingManager = this;
                                w._Caption = DockingManager.GetWindowName(elem);
                                if (w._Caption == string.Empty)
                                {
                                    w._Caption = "element " + tempkey + 1;
                                }
                                w.Caption = GetHeader(elem).ToString();
                                if (DockingManager.GetDockState(elem) != DockState.AutoHidden)
                                {
                                    w.DockState = DockingManager.GetDockState(elem);
                                }
                                w.WindowChildElement = (UIElement)elem;
                                w.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                                w.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                                if (wm != null)
                                {
                                    wm.ShowWindow(w, new Point(0, 0));
                                }
                                else
                                {
                                    ParentDockingManager = (DockingManager)this;
                                    wm = new WindowsManager((DockingManager)this);
                                }
                                bool allowSizeContent = DockingManager.GetSizeToContent(this);
                                if (allowSizeContent)
                                {
                                    allowSizeContent = DockingManager.GetSizeToContent(elem);
                                }
                                if (!allowSizeContent)
                                {
                                    if (DockingManager.GetFloatingWindowRect(elem) != Rect.Empty)
                                    {
                                        Rect floatingRect = DockingManager.GetFloatingWindowRect(elem);
                                        if (floatingRect.Height > 0 && floatingRect.Width > 0)
                                        {
                                            w.FloatHeight = floatingRect.Height;
                                            w.FloatWidth = floatingRect.Width;
                                        }
                                        else
                                        {
                                            w.FloatHeight = 200;
                                            w.FloatWidth = 200;
                                        }
                                        w.LeftPosition = floatingRect.Left;
                                        w.TopPosition = floatingRect.Top;
                                    }
                                }
                                else
                                {
                                    if ((elem as FrameworkElement).Height != 0.0 && (elem as FrameworkElement).Height.ToString() != "NaN")
                                    {
                                        w.FloatHeight = (elem as FrameworkElement).Height;
                                    }
                                    else
                                    {
                                        w.FloatHeight = 200;
                                    }
                                    if ((elem as FrameworkElement).Width != 0.0 && (elem as FrameworkElement).Width.ToString() != "NaN")
                                    {
                                        w.FloatWidth = (elem as FrameworkElement).Width;
                                    }
                                    else
                                    {
                                        w.FloatWidth = 200;
                                    }
                                }

                                (elem as FrameworkElement).Height = double.NaN;
                                (elem as FrameworkElement).Width = double.NaN;


                                if (!WindowCollection.ContainsKey(tempkey))
                                {
                                    WindowCollection.Add(tempkey++, w);
                                }

                                w.DockingManager = this;
                                if ((Dock)GetSideInDockedMode(w.WindowChildElement) != Dock.Tabbed)
                                {
                                    w.DockPosition = (Dock)GetSideInDockedMode(w.WindowChildElement);
                                }

                                w.WindowDockSide = (Dock)GetSideInDockedMode(w.WindowChildElement);
                                w.PaneWidth = (double)GetDesiredWidthInDockedMode(w.WindowChildElement);
                                w.PaneHeight = (double)GetDesiredHeightInDockedMode(w.WindowChildElement);
                                if (GetHeader(elem).ToString() == String.Empty)
                                {
                                    //w.Caption = tempkey.ToString();
                                }
                            }
                            else if (elemCollection.Contains(elem))
                            {
                                alreadyExisting = true;
                                //elemCollection.Remove(elem);
                            }
                        }
                    }
                }
                if (alreadyExisting)
                {
                    if (elemCollection.Contains(element))
                    {
                        elemCollection.Remove(element);
                        if (base.Children.Contains(element))
                        {
                            base.Children.Remove(element);
                        }
                        throw new InvalidOperationException("WindowName is already Existing");
                    }
                }
                if (elemCollection.Count > 0)
                {
                    TarGetNameInDockSide();
                    for (int i = ClientElementCollection.Count; i > 0; i--)
                    {
                        if (base.Children.Contains(ClientElementCollection[i]))
                        {
                            base.Children.Remove(ClientElementCollection[i]);
                            DockingGrid dockingGrid = GetParentDockManager();
                            DockManager dm = null;
                            if (dockingGrid != null && dockingGrid.Parent != null)
                            {
                                dm = dockingGrid.Parent as DockManager;
                            }
                            if (dm != null)
                            {
                                if (dockingGrid.rootWindow.contentpresenter != null)
                                {
                                    dockingGrid.rootWindow.contentpresenter.Children.Add(ClientElementCollection[i]);
                                    ClientElementCollection.Remove(i);
                                }
                            }
                        }
                    }

                    foreach (UIElement elem in elemCollection)
                    {
                        base.Children.Remove(elem);

                    }
                    LayoutUpdatedAfterChildrenAdded();
                }
            }
        }

        private List<UIElement> elemCollection = new List<UIElement>();
        private List<Window> externalWindow = new List<Window>();
        /// <summary>
        /// Raises the <see cref="E:ChildrenCountChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnChildrenCountChanged(DependencyPropertyChangedEventArgs e)
        {
            if (elemCollection.Count == 0 && (int)e.NewValue != 0)
            {
                Add(null);
            }
        }


        /// <summary>
        /// Gets or sets the header template.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DockingManager), new PropertyMetadata(OnHeaderTemplatePropertyChanged));


        /// <summary>
        /// Gets or sets the color of the menu highlighting.
        /// </summary>
        /// <value>The color of the menu highlighting.</value>
        public Brush MenuHighlightingColor
        {
            get
            {
                return (Brush)GetValue(MenuHighlightingColorProperty);
            }

            set
            {
                SetValue(MenuHighlightingColorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the MenuHighlightingColor dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty MenuHighlightingColorProperty =
            DependencyProperty.Register("MenuHighlightingColor", typeof(Brush), typeof(DockingManager), new PropertyMetadata(OnMenuHighlightingColorPropertyChanged));



        /// <summary>
        /// Gets or sets the side button template.
        /// </summary>
        /// <value>The side button template.</value>
        public Style SideButtonTemplate
        {
            get
            {
                return (Style)GetValue(SideButtonTemplateProperty);
            }

            set
            {
                SetValue(SideButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the SideButtonTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty SideButtonTemplateProperty =
            DependencyProperty.Register("SideButtonTemplate", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnSideButtonTemplatePropertyChanged));


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSideButtonTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnSideButtonPropertyChanged(e);
        }





        /// <summary>
        /// Gets or sets the tab item template.
        /// </summary>
        /// <value>The tab item template.</value>
        public Style TabItemTemplate
        {
            get
            {
                return (Style)GetValue(TabItemTemplateProperty);
            }

            set
            {
                SetValue(TabItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the TabItemTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty TabItemTemplateProperty =
            DependencyProperty.Register("TabItemTemplate", typeof(Style), typeof(DockingManager), new PropertyMetadata(null, OnTabItemTemplatePropertyChanged));


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnTabItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnTabItemTemplatePropertyChanged(e);
        }











        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnHeaderPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnMenuHighlightingColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnMenuHighlightingColorPropertyChanged(e);
        }

        /// <summary>
        /// Adds the auto hide tab button.
        /// </summary>
        /// <param name="sideButtonCollection">The side button collection.</param>
        /// <param name="panel">The panel.</param>
        private void AddAutoHideTabButton(ref List<SideButton> sideButtonCollection, SidePanel panel)
        {
            panel.Children.Clear();
            foreach (SideButton sb in sideButtonCollection)
            {
                panel.Children.Add(sb);
            }
            sideButtonCollection.Clear();
        }

        /// <summary>
        /// Raises the <see cref="E:TabItemTemplatePropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabItemTemplatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].CustomTabControl != null)
                {
                    foreach (CustomTabItem cstabItem in this.WindowCollection[i].CustomTabControl.Items)
                    {
                        if (e.NewValue != null)
                        {
                            cstabItem.Style = e.NewValue as Style;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSideButtonPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (btnPaneRight != null)
            {
                List<SideButton> sideButtonCollection = new List<SideButton>();
                foreach (SideButton sb in btnPaneTop.Children)
                {
                    SideButton sb1 = GeneratedSidePanelAsWindowCaption(sb.PinnedWindow, sb.OwnWindow);
                    sb1.Style = SideButtonTemplate;
                    sideButtonCollection.Add(sb1);
                }
                AddAutoHideTabButton(ref sideButtonCollection, btnPaneTop);
                foreach (SideButton sb in btnPaneLeft.Children)
                {
                    //sb.Style = SideButtonTemplate;
                    SideButton sb1 = GeneratedSidePanelAsWindowCaption(sb.PinnedWindow, sb.OwnWindow);
                    sb1.Style = SideButtonTemplate;
                    sideButtonCollection.Add(sb1);
                }
                AddAutoHideTabButton(ref sideButtonCollection, btnPaneLeft);
                foreach (SideButton sb in btnPaneBottom.Children)
                {
                    //sb.Style = SideButtonTemplate;
                    SideButton sb1 = GeneratedSidePanelAsWindowCaption(sb.PinnedWindow, sb.OwnWindow);
                    sb1.Style = SideButtonTemplate;
                    sideButtonCollection.Add(sb1);
                }
                AddAutoHideTabButton(ref sideButtonCollection, btnPaneBottom);
                foreach (SideButton sb in btnPaneRight.Children)
                {
                    //sb.Style = SideButtonTemplate;
                    SideButton sb1 = GeneratedSidePanelAsWindowCaption(sb.PinnedWindow, sb.OwnWindow);
                    sb1.Style = SideButtonTemplate;
                    sideButtonCollection.Add(sb1);
                }
                AddAutoHideTabButton(ref sideButtonCollection, btnPaneRight);
                //UpdateSidePanelLayout();
            }
        }


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnMenuHighlightingColorPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                this.WindowCollection[i].MenuHighlightingColor = this.MenuHighlightingColor;
                //if (this.WindowCollection[i].menuAdv != null)
                //{
                //    this.WindowCollection[i].menuAdv.HighlightingColor = this.MenuHighlightingColor;
                //}
            }
            foreach (UIElement ele in base.Children)
            {
                if (((FrameworkElement)ele).GetType() == typeof(Window))
                {
                    (((FrameworkElement)ele) as Window).MenuHighlightingColor = this.MenuHighlightingColor;
                    //if ((((FrameworkElement)ele) as Window).menuAdv != null)
                    //{
                    //    (((FrameworkElement)ele) as Window).menuAdv.HighlightingColor = this.MenuHighlightingColor;
                    //}
                }
            }
        }


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnHeaderPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyHeaderTemplateToWindow();
        }

        /// <summary>
        /// Applies the header template to window.
        /// </summary>
        protected internal void ApplyHeaderTemplateToWindow()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                this.WindowCollection[i].HeaderTemplate = this.HeaderTemplate;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show menu button].
        /// </summary>
        /// <value><c>true</c> if [show menu button]; otherwise, <c>false</c>.</value>
        public bool ShowMenuButton
        {
            get
            {
                return (bool)GetValue(ShowMenuButtonProperty);
            }

            set
            {
                SetValue(ShowMenuButtonProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ShowMenuButton dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ShowMenuButtonProperty =
            DependencyProperty.Register("ShowMenuButton", typeof(bool), typeof(DockingManager), new PropertyMetadata(true, OnShowMenuButtonPropertyChanged));


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowMenuButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnShowMenuButtonChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnShowMenuButtonChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyVisibilityToWindowMenuButton();
        }
        /// <summary>
        /// Applies the visibility to window menu button.
        /// </summary>
        protected internal void ApplyVisibilityToWindowMenuButton()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].optionsButton != null)
                {
                    if (ShowMenuButton)
                    {
                        this.WindowCollection[i].optionsButton.Visibility = Visibility.Visible;
                        if (this.WindowCollection[i].optionsButton != null)
                        {
                            this.WindowCollection[i].optionsButton.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        this.WindowCollection[i].optionsButton.Visibility = Visibility.Collapsed;
                        if (this.WindowCollection[i].optionsButton != null)
                        {
                            this.WindowCollection[i].optionsButton.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            foreach (UIElement ele in base.Children)
            {
                if (ele.GetType() == typeof(Window))
                {
                    Window w = (Window)ele;
                    if (w.closeButton != null)
                    {
                        if (ShowMenuButton)
                        {
                            w.optionsButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            w.optionsButton.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show close button].
        /// </summary>
        /// <value><c>true</c> if [show close button]; otherwise, <c>false</c>.</value>
        public bool ShowCloseButton
        {
            get
            {
                return (bool)GetValue(ShowCloseButtonProperty);
            }

            set
            {
                SetValue(ShowCloseButtonProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ShowCloseButton dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(DockingManager), new PropertyMetadata(true, OnShowCloseButtonPropertyChanged));


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowCloseButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnShowCloseButtonChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnShowCloseButtonChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyVisibilityToWindowCloseButton();
        }
        /// <summary>
        /// Applies the visibility to window close button.
        /// </summary>
        protected internal void ApplyVisibilityToWindowCloseButton()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].closeButton != null && this.WindowCollection[i].WindowChildElement != null)
                {
                    if (ShowCloseButton && DockingManager.GetCloseButtonVisible(this.WindowCollection[i].WindowChildElement))
                    {
                        this.WindowCollection[i].closeButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.WindowCollection[i].closeButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
            foreach (UIElement ele in base.Children)
            {
                if (ele.GetType() == typeof(Window))
                {
                    Window w = (Window)ele;
                    if (w.closeButton != null && w.WindowChildElement != null)
                    {
                        if (ShowCloseButton && DockingManager.GetCloseButtonVisible(w.WindowChildElement))
                        {
                            w.closeButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            w.closeButton.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value whether to show maximize button.
        /// </summary>
        public bool ShowMaximizeButton
        {
            get
            {
               return (bool)GetValue(ShowMaximizeButtonProperty);
            }
            set
            {
                SetValue(ShowMaximizeButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value whether to show maximize button.
        /// </summary>
        public static readonly DependencyProperty ShowMaximizeButtonProperty = DependencyProperty.Register("ShowMaximizeButton", typeof(bool), typeof(DockingManager), new PropertyMetadata(true, OnShowMaximizeButtonPropertyChanged));

        /// <summary>
        /// This method is called when ShowMaximizeButton property is changed
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        protected static void OnShowMaximizeButtonPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)dependencyObject;
            instance.OnShowMaximizeButtonChanged(e);
        }

        /// <summary>
        /// This method is called when ShowMaximizeButton property is changed
        /// </summary>
        /// <param name="e"></param>
        protected void OnShowMaximizeButtonChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyVisibilityForMaximizeButton();
        }

        /// <summary>
        /// Sets the visibility of the maximize button
        /// </summary>
        private void ApplyVisibilityForMaximizeButton()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].DockState != DockState.AutoHidden && ShowMaximizeButton)
                {
                    this.WindowCollection[i].maximizeButton.Visibility = Visibility.Visible;
                    if (this.WindowCollection[i].WindowChildElement != null)
                        DockingManager.SetMaximizeButtonVisible(this.WindowCollection[i].WindowChildElement, true);
                }
                else
                {
                    this.WindowCollection[i].maximizeButton.Visibility = Visibility.Collapsed;
                    if (this.WindowCollection[i].WindowChildElement != null)
                        DockingManager.SetMaximizeButtonVisible(this.WindowCollection[i].WindowChildElement, false);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show awl button].
        /// </summary>
        /// <value><c>true</c> if [show awl button]; otherwise, <c>false</c>.</value>
        public bool ShowAwlButton
        {
            get
            {
                return (bool)GetValue(ShowAwlButtonProperty);
            }

            set
            {
                SetValue(ShowAwlButtonProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ShowAwlButton dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ShowAwlButtonProperty =
            DependencyProperty.Register("ShowAwlButton", typeof(bool), typeof(DockingManager), new PropertyMetadata(true, OnShowAwlButtonPropertyChanged));


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowAwlButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnShowAwlButtonChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnShowAwlButtonChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyVisibilityToWindowAwlButton();
        }
        /// <summary>
        /// Applies the visibility to window awl button.
        /// </summary>
        protected internal void ApplyVisibilityToWindowAwlButton()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].dockToggle != null)
                {
                    if (ShowAwlButton && this.WindowCollection[i].DockState != DockState.Float && WindowCollection[i].WindowChildElement != null && DockingManager.GetAwlButtonVisible(WindowCollection[i].WindowChildElement))
                    {
                        this.WindowCollection[i].dockToggle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.WindowCollection[i].dockToggle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }









        /// <summary>
        /// Gets or sets a value indicating whether [show window title bar].
        /// </summary>
        /// <value><c>true</c> if [show window title bar]; otherwise, <c>false</c>.</value>
        public bool ShowWindowTitleBar
        {
            get
            {
                return (bool)GetValue(ShowWindowTitleBarProperty);
            }

            set
            {
                SetValue(ShowWindowTitleBarProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ShowWindowTitleBar dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ShowWindowTitleBarProperty =
            DependencyProperty.Register("ShowWindowTitleBar", typeof(bool), typeof(DockingManager), new PropertyMetadata(true, OnShowWindowTitleBarPropertyChanged));


        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowWindowTitleBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnShowWindowTitleBarChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DockState"/> property is changed.
        /// </summary>       
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnShowWindowTitleBarChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyVisibilityToWindowTitleBar();
        }
        /// <summary>
        /// Applies the visibility to window title bar.
        /// </summary>
        protected internal void ApplyVisibilityToWindowTitleBar()
        {
            for (int i = 1; i <= this.WindowCollection.Count; i++)
            {
                if (this.WindowCollection[i].partInnerGrid != null)
                {
                    if (ShowWindowTitleBar)
                    {
                        //this.WindowCollection[i].partInnerGrid.Visibility = Visibility.Visible;
                        this.WindowCollection[i].NoHeaderVisibility(false, DockingManager.GetHeaderHeight(this.WindowCollection[i].WindowChildElement));
                    }
                    else
                    {
                        //this.WindowCollection[i].partInnerGrid.Visibility = Visibility.Collapsed;
                        this.WindowCollection[i].NoHeaderVisibility(true, DockingManager.GetHeaderHeight(this.WindowCollection[i].WindowChildElement));
                    }
                }
            }
            foreach (UIElement ele in base.Children)
            {
                if (ele.GetType() == typeof(Window))
                {
                    if (ShowWindowTitleBar)
                    {
                        //this.WindowCollection[i].partInnerGrid.Visibility = Visibility.Visible;
                        (ele as Window).NoHeaderVisibility(false, DockingManager.GetHeaderHeight(ele));
                    }
                    else
                    {
                        //this.WindowCollection[i].partInnerGrid.Visibility = Visibility.Collapsed;
                        (ele as Window).NoHeaderVisibility(true, DockingManager.GetHeaderHeight(ele));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Identifies the DockPin Enumeration.
    /// </summary>
    public enum DockPin
    {
        /// <summary>
        /// Control is pinned of it's container.
        /// </summary>
        Pinned,

        /// <summary>
        /// Control is Unpinned of it's container.
        /// </summary>
        UnPinned,

        /// <summary>
        /// Control is None of it's container.
        /// </summary>
        None
    }

    /// <summary>
    /// Describes how control is docked to it's container.
    /// </summary>
    public enum DockSide
    {
        /// <summary>
        /// Control is docked to the left side of it's container.
        /// </summary>
        Left,

        /// <summary>
        /// Control is docked to the right side of it's container.
        /// </summary>
        Top,

        /// <summary>
        /// Control is docked to the top side of it's container.
        /// </summary>
        Right,

        /// <summary>
        /// Control is docked to the bottom side of it's container.
        /// </summary>
        Bottom,

        /// <summary>
        /// Control is docked as a tab page of it's container.
        /// </summary>
        Tabbed,

        /// <summary>
        /// Control is not docked.
        /// </summary>
        None
    }

    /// <summary>
    /// Represents the Internal Dock State Enum.
    /// </summary>
    enum InternalDockState
    {
        /// <summary>
        /// Represents the DockWithHidden State
        /// </summary>
        DockWithHidden
    }

    /// <summary>
    /// Represents the Dockable State Enum.
    /// </summary>
    public enum DockableState
    {
        /// <summary>
        /// Represents the Dockable state.
        /// </summary>
        Dockable,
        /// <summary>
        /// Represents the Floating state.
        /// </summary>
        Floating,
        /// <summary>
        /// Represents the Tabbed state.
        /// </summary>
        Tabbed,
        /// <summary>
        /// Represents the AutoHide state.
        /// </summary>
        AutoHide,
        /// <summary>
        /// Represents the Hide state.
        /// </summary>
        Hide
    }

    /// <summary>
    /// Specifies the control state.
    /// </summary>
    public enum DockState
    {
        /// <summary>
        /// Control is docked to the docking manager's surface.
        /// </summary>
        Dock,

        /// <summary>
        /// Control is not docked to the docking manager's surface.
        /// </summary>
        Float,

        /// <summary>
        /// Control is not visible at all.
        /// </summary>
        Hidden,

        /// <summary>
        /// Control is hidden and will show if mouse move under tab.
        /// </summary>
        AutoHidden,

        /// <summary>
        /// Control is tabbed or MDI document.
        /// </summary>
        Document
    }

    /// <summary>
    /// Identifies the Theme Enumeration.
    /// </summary>
    public enum Theme
    {
        /// <summary>
        /// Blend Theme is applied
        /// </summary>  
        Blend,

        /// <summary>
        /// Default Theme is applied
        /// </summary>   
        Default,

        /// <summary>
        /// Office2007Black Theme is applied
        /// </summary>   
        Office2007Black,

        /// <summary>
        /// Office2007Blue Theme is applied
        /// </summary>  
        Office2007Blue,

        /// <summary>
        /// Office2007Silver Theme is applied
        /// </summary>  
        Office2007Silver,

        /// <summary>
        /// Office2003Blue Theme is applied
        /// </summary>  
        Office2003Blue
    }

    /// <summary>
    /// Specifies where user can dock an element inside other element using inner DragProviders
    /// </summary>
    [Flags]
    public enum DockAbility
    {
        /// <summary>
        /// User cannot dock anywhere
        /// </summary>
        None = 0,

        /// <summary>
        /// User can dock an element to the left
        /// </summary>
        Left = 1,

        /// <summary>
        /// User can dock an element to the right
        /// </summary>
        Right = 2,

        /// <summary>
        /// User can dock an element to the top
        /// </summary>
        Top = 4,

        /// <summary>
        /// User can dock an element to the bottom
        /// </summary>
        Bottom = 8,

        /// <summary>
        /// User can dock an element in tab group with other element
        /// </summary>
        Tabbed = 16,

        /// <summary>
        /// All DragProviders will be visible
        /// </summary>
        All = 32
    }

    /// <summary>
    /// Specifies where user can dock an element using outer DragProviders
    /// </summary>
    [Flags]
    public enum OuterDockAbility
    {
        /// <summary>
        /// User cannot dock anywhere
        /// </summary>
        None = 0,

        /// <summary>
        /// User can dock an element to the left
        /// </summary>
        Left = 1,

        /// <summary>
        /// User can dock an element to the right
        /// </summary>
        Right = 2,

        /// <summary>
        /// User can dock an element to the bottom
        /// </summary>
        Bottom = 4,

        /// <summary>
        /// User can dock an element to the top
        /// </summary>
        Top = 8,

        /// <summary>
        /// All outer drag providers will be visible
        /// </summary>
        All = 16
    }

    /// <summary>
    /// Specifies whether window is in maximized or restored state
    /// </summary>
    public enum MaximizedState
    {
        /// <summary>
        /// Indicates window is maximized
        /// </summary>
        Maximized,
        /// <summary>
        /// Indicates window is restored
        /// </summary>
        Restored
    }
}


