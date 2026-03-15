#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents BackStageTabItem class.
    /// </summary>
    public class BackstageTabItem : ContentControl
    {
        #region Properties

        SystemGesture msystemGesture;

        /// <summary>
        /// Dependency property for isSelected
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(BackstageTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnIsSelectedChanged));

        /// <summary>
        /// Gets or sets whether the tab is selected
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)base.GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets parent Back Stage Element.
        /// </summary>
        internal Backstage BackStageParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as Backstage);
            }
        }

        /// <summary>
        /// Gets or sets tab items text
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }



        /// <summary>
        /// Gets or Sets Tab Item Text field. It is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object),
            typeof(BackstageTabItem), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static BackstageTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(typeof(BackstageTabItem)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BackstageTabItem()
        {

        }

        #endregion

        #region Overrides

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            if (e.Source is BackstageTabItem && this.BackStageParent!=null)
            {
                this.BackStageParent.SelectedItem = e.Source as BackstageTabItem;
                this.BackStageParent.SelectedTabContent = (e.Source as BackstageTabItem).Content;
            }
        }

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (IsSelected && BackStageParent != null)
            {
                BackStageParent.SelectedTabContent = newContent;
            }
        }


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (this.BackStageParent != null && this.BackStageParent.parentRibbon != null && !this.BackStageParent.parentRibbon.EnableTouch))
            {
                if (((e.Source == this) || !IsSelected))
                {
                    if (BackStageParent != null && BackStageParent.SelectedItem is BackstageTabItem)
                        ((BackstageTabItem)BackStageParent.SelectedItem).IsSelected = false;
                    else
                    {
                        if (BackStageParent != null)
                        {
                            BackstageTabItem bTabItem = BackStageParent.ItemContainerGenerator.ContainerFromItem(BackStageParent.SelectedItem) as BackstageTabItem;
                            if (bTabItem != null)
                                bTabItem.IsSelected = false;
                        }
                    }

                    IsSelected = true;
                    Focus();
                }
                e.Handled = true;
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchDown(TouchEventArgs e)
        {            
            if (this.BackStageParent!=null&& this.BackStageParent.parentRibbon!=null && this.BackStageParent.parentRibbon.EnableTouch)
            {
                if (((e.Source == this) || !IsSelected))
                {
                    if (BackStageParent != null && BackStageParent.SelectedItem is BackstageTabItem)
                        ((BackstageTabItem)BackStageParent.SelectedItem).IsSelected = false;
                    else
                    {
                        if (BackStageParent != null)
                        {
                            BackstageTabItem bTabItem = BackStageParent.ItemContainerGenerator.ContainerFromItem(BackStageParent.SelectedItem) as BackstageTabItem;
                            if (bTabItem != null)
                                bTabItem.IsSelected = false;
                        }
                    }

                    IsSelected = true;
                    Focus();
                }
                e.Handled = true;
                base.OnTouchDown(e);
            }
        }

#endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        #endregion

        #region Private methods

        // Handles IsSelected changed
        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackstageTabItem container = (BackstageTabItem)d;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                Backstage backstage = container.BackStageParent as Backstage;
                if ((backstage != null) && (backstage.SelectedItem != container))
                {
                    if (backstage.parentRibbon != null && ((backstage.parentRibbon.WFHvisible != null && backstage.parentRibbon.WFHvisible.Value) || (backstage.parentRibbon.browserVisible != null && backstage.parentRibbon.browserVisible.Value)))
                        {
                            if ((VisualUtils.FindDescendant(container, typeof(WindowsFormsHost)) as WindowsFormsHost) != null && (VisualUtils.FindDescendant(container, typeof(WindowsFormsHost)) as WindowsFormsHost).Visibility != Visibility.Visible)
                            {
                                (VisualUtils.FindDescendant(container, typeof(WindowsFormsHost)) as WindowsFormsHost).Visibility = Visibility.Visible;
                            }
                            else if ((VisualUtils.FindDescendant(container, typeof(HwndHost)) as HwndHost) != null && (VisualUtils.FindDescendant(container, typeof(HwndHost)) as HwndHost).Visibility != Visibility.Visible)
                            {
                                (VisualUtils.FindDescendant(container, typeof(HwndHost)) as HwndHost).Visibility = Visibility.Visible;
                            }
                            WindowsFormsHost wfh = null;
                            HwndHost hwndHost = null;
                            if (container.Content != null)
                            {
                                if (container.Content is WindowsFormsHost)
                                {
                                    wfh = container.Content as WindowsFormsHost;
                                }
                                else if (container.Content is Visual && (VisualUtils.FindDescendant((Visual)container.Content, typeof(WindowsFormsHost)) != null))
                                {
                                    wfh = VisualUtils.FindDescendant((Visual)container.Content, typeof(WindowsFormsHost)) as WindowsFormsHost;
                                }
                                if (wfh != null && wfh.Visibility != Visibility.Visible)
                                {
                                    wfh.Visibility = Visibility.Visible;
                                }

                                if (container.Content is HwndHost && !(container.Content is WindowsFormsHost))
                                {
                                    hwndHost = container.Content as HwndHost;
                                }
                                else if (container.Content is Visual && (VisualUtils.FindDescendant((Visual)container.Content, typeof(HwndHost)) != null))
                                {
                                    hwndHost = (VisualUtils.FindDescendant((Visual)container.Content, typeof(HwndHost)) as HwndHost);
                                }

                                if (hwndHost != null && hwndHost.Visibility != Visibility.Visible)
                                {
                                    hwndHost.Visibility = Visibility.Visible;
                                }
                            }
                        }
                    
                    if (backstage.SelectedItem is BackstageTabItem) (backstage.SelectedItem as BackstageTabItem).IsSelected = false;
                    else
                    {
                        BackstageTabItem bTabItem = backstage.ItemContainerGenerator.ContainerFromItem(backstage.SelectedItem) as BackstageTabItem;
                        if (bTabItem != null)
                        {
                            bTabItem.IsSelected = false;
                        }
                    }
                    backstage.SelectedItem = container;
                }
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
        }

        /// <summary>
        /// Handles selected event
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(e);
        }

        /// <summary>
        /// Handles unselected event
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(e);
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Handles IsSelected changed
        /// </summary>
        /// <param name="e">The event data.</param>
        private void HandleIsSelectedChanged(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion


        #region Overrides

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BackStageTabItemAutomationPeer(this);
        }

        #endregion
    }   
}