// <copyright file="NavigationToolbarItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Syncfusion.Licensing;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the NavigationToolbarItem UI element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class NavigationToolbarItem : Control
    {
        #region Constants
        /// <summary>
        /// Default tooltip height.
        /// </summary>
        private const double DEF_TOOLTIP_HEIGHT = 20;
        
        /// <summary>
        /// Default tooltip width.
        /// </summary>
        private const double DEF_TOOLTIP_WIDTH = double.NaN;
        #endregion

        #region Private members
        /// <summary>
        /// Presented FxGroupBarItem.
        /// </summary>
        private GroupBarItem m_groupBarItem;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty;
        
        /// <summary>
        /// Identifies <see cref="ImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(NavigationToolbarItem), new UIPropertyMetadata(null));
        
        /// <summary>
        /// Identifies <see cref="ShowInToolbar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowInToolbarProperty = DependencyProperty.Register("ShowInToolbar", typeof(bool), typeof(NavigationToolbarItem), new UIPropertyMetadata(true, OnShowInToolbarChanged));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [show in toolbar].
        /// </summary>
        /// <value><c>true</c> if [show in toolbar]; otherwise, <c>false</c>.</value>
        public bool ShowInToolbar
        {
            get
            {
                return (bool)GetValue(ShowInToolbarProperty);
            }

            set
            {
                SetValue(ShowInToolbarProperty, value);
            }
        }

        /// <summary>
        /// Gets the group bar item.
        /// </summary>
        /// <value>The group bar item.</value>
        public GroupBarItem GroupBarItem
        {
            get
            {
                return m_groupBarItem;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// true if this instance is selected; otherwise, false.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(NavigationToolbarItem.IsSelectedProperty);
            }

            set
            {
                SetValue(NavigationToolbarItem.IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        /// <value>The image source.</value>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }

            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets the logical parent.
        /// </summary>
        /// <value>The logical parent.</value>
        public NavigationToolbar LogicalParent
        {
            get
            {
                NavigationToolbar result = ItemsControl.ItemsControlFromItemContainer(this) as NavigationToolbar;

                ////if (result == null)
                ////{
                ////    throw new ApplicationException("Cannot find NavigationToolbar");
                ////}

                return result;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationToolbarItem"/> class.
        /// </summary>
        /// <param name="item">The item Navigation toolbar item.</param>
        public NavigationToolbarItem(GroupBarItem item)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !Syncfusion.Windows.Shared.BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

            if (item != null)
            {
                m_groupBarItem = item;
                GroupBarItemHeader header = item.Header as GroupBarItemHeader;

                if (header == null && item.HeaderImageSource != null)
                {
                    ImageSource = item.HeaderImageSource;
                }

                else if (header != null && header.ImageSource != null)
                {
                    ImageSource = header.ImageSource;
                }

                IsSelected = item.IsSelected;
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="NavigationToolbarItem"/> class.
        /// </summary>
        static NavigationToolbarItem()
        {
            EnvironmentTest.ValidateLicense(typeof(NavigationToolbarItem));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationToolbarItem), new FrameworkPropertyMetadata(typeof(NavigationToolbarItem)));

            NavigationToolbarItem.IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(NavigationToolbarItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(NavigationToolbarItem.OnIsSelectedChanged)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the value of <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem.ShowInGroupBarProperty"/> property is changed.
        /// </summary>
        /// <param name="d">NavigationToolbarItem object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnShowInToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationToolbarItem item = d as NavigationToolbarItem;
            if (d != null && e != null)
            {
                item.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// Called when the value of <see cref="IsSelected"/> property is changed.
        /// </summary>
        /// <param name="d">NavigationToolbarItem object.</param>
        /// <param name="e">The object containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationToolbarItem owner = d as NavigationToolbarItem;
            bool isSelected = (e == null) ? false : (bool)e.NewValue;

            if (owner != null && owner.IsInitialized)
            {
                if (isSelected)
                {
                    if (owner.LogicalParent != null)
                    {
                        foreach (NavigationToolbarItem item in owner.LogicalParent.Items)
                        {
                            if (item != owner)
                            {
                                item.IsSelected = false;
                            }
                        }

                        owner.LogicalParent.SetValue(NavigationToolbar.SelectedItemProperty, owner);
                        owner.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, owner));
                    }
                }
                else
                {
                    if (owner.LogicalParent != null)
                    {
                        owner.LogicalParent.SetValue(NavigationToolbar.SelectedItemProperty, null);
                        owner.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, owner));
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when item <see cref="IsSelected"/> property is changed.
        /// </summary>
        /// <param name="e">The object containing the event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
        }
        
        /// <summary>
        /// Called when item <see cref="IsSelected"/> property is changed.
        /// </summary>
        /// <param name="e">The object containing the event data.</param> 
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
        }
        
        /// <summary>
        /// Creates the tooltip.
        /// </summary>
        /// <param name="forItem">Item to create tooltip for.</param>
        /// <returns>
        /// Tooltip for the given FxGroupBarItem.
        /// </returns>
        private object CreateTooltip(GroupBarItem forItem)
        {
            if (forItem.LogicalParent.ItemsSource == null)
            {
                DockPanel panel = LogicalParent.CreateItemPanel(new Size(DEF_TOOLTIP_WIDTH, DEF_TOOLTIP_HEIGHT), forItem.HeaderText, forItem.HeaderImageSource);
                panel.Margin = new Thickness(5d, 3d, 5d, 3d);
                return panel;
            }
            else
            {
                Label label = new Label();
                label.Content = forItem.Header;

                if (forItem.HeaderTemplate != null)
                {
                    label.ContentTemplate = forItem.HeaderTemplate;
                    
                }
                else
                {
                    if (forItem.LogicalParent.ItemTemplate != null)
                    {
                        label.ContentTemplate = forItem.LogicalParent.ItemTemplate;
                    }
                }
                if (forItem.HeaderTemplateSelector != null)
                {
                    label.ContentTemplateSelector = forItem.HeaderTemplateSelector;
                }
                else
                {
                    label.ContentTemplateSelector = forItem.LogicalParent.ItemTemplateSelector;
                }
                return label;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            GroupBar.isNavigatonItemClicked = true;
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;
            IsSelected = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> property is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ToolTip = CreateTooltip(GroupBarItem);
        }
        #endregion
    }
}
