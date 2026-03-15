// <copyright file="RibbonMenuItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
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
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a selectable item inside a Menu.
    /// </summary>
    /// <remarks>
    /// A RibbonMenuItem can have submenus. The submenu of the RibbonMenuItem is made up of the objects within the 
    /// ItemCollection of a RibbonMenuItem. It is common for a RibbonMenuItem to contain other RibbonMenuItem objects to 
    /// create nested submenus.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonMenuItem : MenuItem
    {
        #region Events
        /// <summary>
        /// Event that is raised when FlowDirection property is changed. 
        /// </summary>
        public event PropertyChangedCallback FlowDirectionChanged;

        /// <summary>
        /// Event that is raised when Iconsize property is changed.
        /// </summary>
        public event PropertyChangedCallback IconSizeChanged;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether menu icon bar is enabled.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True if icon bar is enabled, false if disabled.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonMenuItem item;
        /// // ....
        /// item.IconBarEnabled = true;
        /// </code>
        /// </example>
        public bool IconBarEnabled
        {
            get
            {
                return (bool)GetValue(IconBarEnabledProperty);
            }

            set
            {
                SetValue(IconBarEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        /// <value>The size of the icon.</value>
        public Size IconSize
        {
            get
            {
                return (Size)GetValue(IconSizeProperty);
            }

            set
            {
                SetValue(IconSizeProperty, value);
            }
        }

        /// <summary>
        /// Checks Whether the RibbonMenuItem is already been clicked.
        /// </summary>
         static bool isClicked = false;

        #endregion

        #region DP Properties
        /// <summary>
        /// Specifies whether menu icon bar is enabled. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconBarEnabledProperty =
            DependencyProperty.Register("IconBarEnabled", typeof(bool), typeof(RibbonMenuItem), new UIPropertyMetadata(false));

        /// <summary>
        ///  Defines the iconsize that appears in SimpleMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(Size), typeof(RibbonMenuItem), new FrameworkPropertyMetadata(new Size(16d, 16d), new PropertyChangedCallback(OnIconSizeChanged)));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="RibbonMenuItem"/> class.
        /// </summary>
        static RibbonMenuItem()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonMenuItem));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenuItem), new FrameworkPropertyMetadata(typeof(RibbonMenuItem)));
            FlowDirectionProperty.OverrideMetadata(typeof(RibbonMenuItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlowDirectionChanged)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp routed event
        /// reaches an element in its route that is derived from this
        /// class. Implement this method to add class handling for this
        /// event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the
        /// event data. The event data reports that the
        /// left mouse button was released.</param>    
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            
            ribbonMenuItem = VisualUtils.FindSomeParent(this, typeof(RibbonMenuItem)) as RibbonMenuItem;


            if (DropdownButton != null)
            {
                DropdownButton.IsDropDownOpen = StaysOpenOnClick;
            }
            else
            {
                if (ribbonMenuItem != null)
                {
                    ribbonMenuItem.IsSubmenuOpen = StaysOpenOnClick;
                }
                else
                {
                    ribbonMenuItem = this.ParentMenuItem as RibbonMenuItem;
                    if (ribbonMenuItem != null)
                    {
                        if (ribbonMenuItem.DropdownButton != null)
                        {
                            ribbonMenuItem.DropdownButton.IsDropDownOpen = StaysOpenOnClick;
                        }
                    }
                    else
                    {
                        if (!StaysOpenOnClick)
                        {
                            e.Handled = StaysOpenOnClick;
                            isClicked = true;
                        }

                        if (isClicked && StaysOpenOnClick)
                        {
                            e.Handled = !StaysOpenOnClick;
                            isClicked = false;
                        }
                        else if (!isClicked && StaysOpenOnClick)
                        {
                            e.Handled = StaysOpenOnClick;
                        }
                    }
                }
            }
        }

        internal DropDownButton DropdownButton
        {
            get
            {
                return (DropDownButton)VisualUtils.FindSomeParent(this, typeof(DropDownButton));
            }
        }

        internal RibbonMenuItem ParentMenuItem
        {
            get;
            set;
        }

        RibbonMenuItem ribbonMenuItem = null;
        /// <summary>
        /// Raises the Click event.
        /// </summary>
        protected override void OnClick()
        {

            //dropdownButton = (DropDownButton)VisualUtils.FindSomeParent(this, typeof(DropDownButton));
            ribbonMenuItem = VisualUtils.FindSomeParent(this, typeof(RibbonMenuItem)) as RibbonMenuItem;

            if (ribbonMenuItem != null && ribbonMenuItem.IsSubmenuOpen)
            {
                ribbonMenuItem.IsSubmenuOpen = StaysOpenOnClick;
            }

            if (DropdownButton != null && DropdownButton.IsDropDownOpen)
            {
                DropdownButton.IsDropDownOpen = false;
                DropdownButton.MenuPopUp.Closed += new EventHandler(MenuPopUp_Closed);
            }
            else
            {
                base.OnClick();
            }
        

           // base.OnClick();
            if ((Tag as string) == "Remove")
            {
                RibbonCommands.RemoveItemFromQAT.Execute(this.CommandTarget, null);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnClick();
                e.Handled = true;
                if(DropdownButton != null)
                this.DropdownButton.IsDropDownOpen = false;
            }

            base.OnPreviewKeyDown(e);
        }

        void MenuPopUp_Closed(object sender, EventArgs e)
        {
            base.OnClick();
            if(DropdownButton != null)
            DropdownButton.MenuPopUp.Closed -= new EventHandler(MenuPopUp_Closed);
        }


        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Command != null)
            {
                if (RibbonCommandManager.CommandDictionary.ContainsKey(Command))
                {
                    if (RibbonCommandManager.CommandDictionary[Command].SmallIcon != null)
                    {
                        Image img = new Image();
                        img.Source = RibbonCommandManager.CommandDictionary[Command].SmallIcon;
                        Icon = img;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].Label != null)
                    {
                        Header = RibbonCommandManager.CommandDictionary[Command].Label;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].ToolTip != null)
                    {
                        ToolTip = RibbonCommandManager.CommandDictionary[Command].ToolTip;
                    }
                }
            }
        }

        /// <summary>
        /// Called whenever the mouse enters a <see cref="T:System.Windows.Controls.MenuItem"/>.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (HasItems)
            {
                IsSubmenuOpen = true;
            }

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Called when the right mouse button is released.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/> event.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            if ((e.Source is RibbonMenuItem) && this.ContextMenu == null)
            {
                RibbonContextMenu.CreateContextMenu(this);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Calls OnFlowDirectionChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonMenuItem instance = (RibbonMenuItem)d;
            instance.OnFlowDirectionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// FlowDirectionChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFlowDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FlowDirectionChanged != null)
            {
                FlowDirectionChanged(this, e);
            }

            ScaleTransform transform = new ScaleTransform();
            transform.ScaleX = 1;

            RenderTransform = transform;
            RenderTransformOrigin = new Point(0.5, 0.5);
        }

        /// <summary>
        /// Called when [icon size changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonMenuItem instance = (RibbonMenuItem)d;
            instance.OnIconSizeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IconSizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIconSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IconSizeChanged != null)
            {
                IconSizeChanged(this, e);
            }
        }
        bool isseparator = false;

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own ItemContainer.
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <returns>
        /// true if the item is its own ItemContainer; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is Separator)
                isseparator = true;
            return item is RibbonMenuItem;
        }

        /// <summary>
        /// Creates or identifies the element used to display a specified item.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Syncfusion.Windows.Controls.RibbonMenuItem"/>.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            if (!isseparator)
            {
                RibbonMenuItem menuItem = new RibbonMenuItem() { ParentMenuItem = this };
                return menuItem;
            }
            else
            {
                isseparator = false;
                return new Separator();
            }
        }
        #endregion
    }
}
