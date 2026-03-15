// <copyright file="MenuButtonBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is a base class for Ribbon MenuButton controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class MenuButtonBase : HeaderedItemsControl, IRibbonControl
    {
        #region Private members
        /// <summary>
        /// Application menu object.
        /// </summary>
        private ApplicationMenu m_applicationMenu;

        /// <summary>
        /// Top scroll viewer button. 
        /// </summary>
        private RepeatButton m_topButton;

        /// <summary>
        /// Bottom scroll viewer button. 
        /// </summary>
        private RepeatButton m_bottomButton;

        /// <summary>
        /// Represents the contextMenu handling
        /// </summary>
        private bool m_bIsContextMenuHandling = false;

        SystemGesture msystemGesture;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuButtonBase"/> class.
        /// </summary>
        public MenuButtonBase()
        {
            this.FocusVisualStyle = null;
        }
        #endregion

        #region Dependency properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines text label information.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(MenuButtonBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLabelChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines the icon for a button.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MenuButtonBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIconChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines whether menu is open.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMenuOpenProperty = DependencyProperty.Register("IsMenuOpen", typeof(bool), typeof(MenuButtonBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsMenuOpenChanged)));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the application menu.
        /// </summary>
        /// <value>The application menu.</value>
        protected ApplicationMenu ApplicationMenu
        {
            get
            {
                return m_applicationMenu;
            }

            set
            {
                m_applicationMenu = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the text that labels Menu button.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that labels the Menu button. The default is empty string.
        /// </value>        
        /// <seealso cref="string"/>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
        
            set
            {
                SetValue(LabelProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the icon that appears in menu button.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an icon. 
        /// </remarks>
        /// <seealso cref="ImageSource"/>
        public ImageSource Icon
        {
            get
            {
                return (ImageSource)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is menu open.
        /// </summary>
        public bool IsMenuOpen
        {
            get
            {
                return (bool)GetValue(IsMenuOpenProperty);
            }

            set
            {
                SetValue(IsMenuOpenProperty, value);
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Icon property is changed.
        /// </summary>
        public event PropertyChangedCallback IconChanged;

        /// <summary>
        /// Event that is raised when Label property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelChanged;

        /// <summary>
        /// Event that is raised when IsMenuOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsMenuOpenChanged;

        #endregion

        #region Implementation
        /// <summary>
        /// Gets application Menu handler.
        /// </summary>
        /// <param name="child">Element which Application Menu contains.</param>
        /// <returns>It returns the GetAppMenu</returns>
        protected ApplicationMenu GetAppMenu(MenuButtonBase child)
        {
            if (child != null)
            {
                if (child.Parent is ApplicationMenu)
                {
                    return child.Parent as ApplicationMenu;
                }
                else
                {
                    return GetAppMenu(child.Parent as MenuButtonBase);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Calls OnLabelChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuButtonBase instance = (MenuButtonBase)d;
            instance.OnLabelChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises LabelChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnLabelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LabelChanged != null)
            {
                LabelChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIconChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuButtonBase instance = (MenuButtonBase)d;
            instance.OnIconChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IconChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IconChanged != null)
            {
                IconChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsMenuOpenChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsMenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuButtonBase instance = (MenuButtonBase)d;
            instance.OnIsMenuOpenChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsMenuOpenChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsMenuOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMenuOpenChanged != null)
            {
                IsMenuOpenChanged(this, e);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Raises the MouseEnter event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnMouseEnter(e);

                if (ApplicationMenu != null)
                {
                    if (ApplicationMenu.SelectedItem == this)
                    {
                        IsMenuOpen = true;
                    }
                    else
                    {
                        if (Items.Count > 0)
                        {
                            ApplicationMenu.SelectedItem = this;
                        }
                    }
                }
                else
                {
                    IsMenuOpen = true;
                }
                this.Focus();
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchEnter(TouchEventArgs e)
        {
             var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;

             if (ribbonTouch != null && ribbonTouch.EnableTouch)
             {
                 base.OnTouchEnter(e);
                 if (ApplicationMenu != null)
                 {
                     if (ApplicationMenu.SelectedItem == this)
                     {
                         IsMenuOpen = true;
                     }
                     else
                     {
                         if (Items.Count > 0)
                         {
                             ApplicationMenu.SelectedItem = this;
                         }
                     }
                 }
                 else
                 {
                     IsMenuOpen = true;
                 }
                 this.Focus();
             }
        }
        #endif

        /// <summary>
        /// Raises the MouseLeave event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
             var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnMouseLeave(e);
                if (ApplicationMenu != null)
                {
                    if (ApplicationMenu.SelectedItem != null && !m_bIsContextMenuHandling)
                    {
                        if (Parent != ApplicationMenu.SelectedItem)
                        {
                            if (ApplicationMenu.SelectedItem.Parent == this.Parent && this.Parent is MenuButtonBase)
                            {
                                ApplicationMenu.SelectedItem = this.Parent as MenuButtonBase;
                            }
                            else
                            {
                                ApplicationMenu.SelectedItem = null;
                            }
                        }
                    }
                    else
                    {
                        m_bIsContextMenuHandling = false;
                    }
                }
                else
                {
                    IsMenuOpen = false;
                }
                this.Focus();
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchLeave(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch != null && ribbonTouch.EnableTouch)
            {
                base.OnTouchLeave(e);

                if (ApplicationMenu != null)
                {
                    if (ApplicationMenu.SelectedItem != null && !m_bIsContextMenuHandling)
                    {
                        if (Parent != ApplicationMenu.SelectedItem)
                        {
                            if (ApplicationMenu.SelectedItem.Parent == this.Parent && this.Parent is MenuButtonBase)
                            {
                                ApplicationMenu.SelectedItem = this.Parent as MenuButtonBase;
                            }
                            else
                            {
                                ApplicationMenu.SelectedItem = null;
                            }
                        }
                    }
                    else
                    {
                        m_bIsContextMenuHandling = false;
                    }
                }
                else
                {
                    IsMenuOpen = false;
                }
                this.Focus();
            }
        }
        #endif
      
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                m_bIsContextMenuHandling = true;
                base.OnMouseRightButtonUp(e);
            }
        }

#if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch!=null && ribbonTouch.EnableTouch && msystemGesture==SystemGesture.RightTap)
            {
                m_bIsContextMenuHandling = true;
                base.OnTouchUp(e);
            }
        }
#endif


        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplicationMenu = GetAppMenu(this);
            ScrollViewer scroller = (ScrollViewer)GetTemplateChild("PART_ScrollViwer");
            scroller.Loaded += new RoutedEventHandler(Scroller_Loaded);
            scroller.ScrollChanged += new ScrollChangedEventHandler(Scroller_ScrollChanged);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Handles the Loaded event of the scroller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Scroller_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Visual vis in VisualUtils.EnumChildrenOfType(sender as ScrollViewer, typeof(RepeatButton)))
            {
                RepeatButton button = vis as RepeatButton;

                if (button.Name == "PART_BottomButton")
                {
                    m_bottomButton = button;
                }

                if (button.Name == "PART_TopButton")
                {
                    m_topButton = button;
                }
            }
        }

        /// <summary>
        /// Handles the ScrollChanged event of the scroller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ScrollChangedEventArgs"/> instance containing the event data.</param>
        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (m_bottomButton != null && m_topButton != null)
            {
                if (e.VerticalOffset == (sender as ScrollViewer).ScrollableHeight)
                {
                    m_bottomButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    m_bottomButton.Visibility = Visibility.Visible;
                }

                if (e.VerticalOffset == 0)
                {
                    m_topButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    m_topButton.Visibility = Visibility.Visible;
                }
            }
        }
        #endregion

        #region IRibbonControl Members

        /// <summary>
        /// Gets the value of the Image property.
        /// </summary>
        /// <value></value>
        public ImageSource SmallIcon
        {
            get { return Icon; }
        }
        #endregion
    }
}
