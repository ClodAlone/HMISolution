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
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    public class ApplicationMenuButton : ButtonBase
    {
        public ApplicationMenuButton()
        {
            this.DefaultStyleKey = typeof(ApplicationMenuButton);
            this.KeyDown += new KeyEventHandler(ApplicationMenuButton_KeyDown);
        }

        #region Fileds
        /// <summary>
        /// represents the application menu
        /// </summary>
        ApplicationMenu appmenu = null;
        internal ApplicationMenu parentApplicationMenu = null;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines the description of the ApplicationMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ApplicationMenuButton), new PropertyMetadata(null, new PropertyChangedCallback(OnDescriptionChanged)));

        /// <summary>
        /// Defines the text that labels ApplicationMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(ApplicationMenuButton), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelChanged)));

        /// <summary>
        ///  Defines the icon that appears in ApplicationMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ApplicationMenuButton), new PropertyMetadata(null, new PropertyChangedCallback(OnIconChanged)));

        /// <summary>
        ///  Defines the iconsize that appears in ApplicationMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(Size), typeof(ApplicationMenuButton), new PropertyMetadata(new Size(20d, 20d), new PropertyChangedCallback(OnIconSizeChanged)));

        public bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(ApplicationMenuButton), new PropertyMetadata(false, OnMouseOverChanged));

        public static void OnMouseOverChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderItem = (ApplicationMenuButton)sender;
            senderItem.GotoVisualStates();
        }

        private void GotoVisualStates()
        {
            if (IsMouseOver)
                VisualStateManager.GoToState(this, "MouseOver", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);
        }

        #endregion

        #region Properties

        public double DescriptionWidth
        {
            get { return (double)GetValue(DescriptionWidthProperty); }
            set { SetValue(DescriptionWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DescriptionWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionWidthProperty =
            DependencyProperty.Register("DescriptionWidth", typeof(double), typeof(ApplicationMenuButton), new PropertyMetadata(100d));

        /// <summary>
        /// Gets or sets the description of the ApplicationMenuButton.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// String value that will be displayed in the ApplicationMenuButton.
        /// </value>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }

            set { SetValue(DescriptionProperty, value); }
        }

        public object ToolTip
        {
            get { return (object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        public FontWeight LabelFontWeight
        {
            get { return (FontWeight)GetValue(LabelFontWeightProperty); }
            set { SetValue(LabelFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelFontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(ApplicationMenuButton), new PropertyMetadata(FontWeights.Normal));

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(object), typeof(ApplicationMenuButton), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the text that labels ApplicationMenuButton.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that labels the ApplicationMenuButton. The default is empty string.
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
        /// Gets or sets the icon that appears in ApplicationMenuButton.
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
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Description property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionChanged;

        /// <summary>
        /// Event that is raised when Icon property is changed.
        /// </summary>
        public event PropertyChangedCallback IconChanged;

        /// <summary>
        /// Event that is raised when Iconsize property is changed.
        /// </summary>
        public event PropertyChangedCallback IconSizeChanged;

        /// <summary>
        /// Event that is raised when Label property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelChanged;

        #endregion

        #region Implementation

        /// <summary>
        /// Calls OnDescriptionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationMenuButton instance = (ApplicationMenuButton)d;
            instance.OnDescriptionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises DescriptionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Description != null && this.Description.Length > 0)
                this.LabelFontWeight = FontWeights.Bold;
            if (DescriptionChanged != null)
            {
                DescriptionChanged(this, e);
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
            ApplicationMenuButton instance = (ApplicationMenuButton)d;
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
            ApplicationMenuButton instance = (ApplicationMenuButton)d;
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
        /// Called when [icon size changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationMenuButton instance = (ApplicationMenuButton)d;
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

        private void ApplicationMenuButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OnClick();
            else if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Home || e.Key == Key.End)
            {
                var parentItem = ((ApplicationMenuButton)this).Parent;
                FindAndFocusItem(parentItem, e.Key);
            }
            else if (e.Key == Key.Left)
                HidePopUp();

            else if (e.Key == Key.Escape)
                if (this.parentApplicationMenu != null)
                    this.parentApplicationMenu.PART_RibbonDropDown.IsOpen = false;
        }

        private void HidePopUp()
        {
            if (this.Parent.GetType().Name == "RibbonMenuGroup")
            {
                RibbonMenuGroup menuGroupParent = this.Parent as RibbonMenuGroup;
                if (menuGroupParent.Parent.GetType().Name == "ApplicationSplitButton")
                {
                    ApplicationSplitButton splitButton = menuGroupParent.Parent as ApplicationSplitButton;
                    splitButton.HidePopup();
                    menuGroupParent.HandleLoadedEvent();
                    splitButton.Focus();
                    splitButton.IsMouseOver = true;
                }
            }
        }

        private void FindAndFocusItem(object parentItem, Key enteredKey)
        {
            ItemsControl parent = null;
            if (parentItem is ItemsControl)
                if (parentItem.GetType().Name == "RibbonItemsControl")
                    parent = parentItem as RibbonItemsControl;
                else if (parentItem.GetType().Name == "RibbonMenuGroup")
                    parent = parentItem as DropDownMenuGroup;

            int count = parent.Items.Count;
            int startingIndex = 0;

            HideMouseOverItems(parent);

            if (this != null)
                startingIndex = parent.ItemContainerGenerator.IndexFromContainer(this);

            var firstItem = parent.ItemContainerGenerator.ContainerFromIndex(0);
            var rootParent = ((FrameworkElement)firstItem).Parent;

            int index = startingIndex;
            do
            {
                if (enteredKey == Key.Up)
                {
                    index = (index + count - 1) % count;
                    if (rootParent is DropDownMenuGroup)
                        ((DropDownMenuGroup)rootParent).menuButtonDownCount++;
                    else if (rootParent is RibbonItemsControl)
                        ((RibbonItemsControl)this.Parent).downCount++;
                }

                else if (enteredKey == Key.Down)
                {
                    if (rootParent is DropDownMenuGroup)
                    {
                        if (((DropDownMenuGroup)rootParent).menuButtonDownCount == 0)
                        { index = 0; ((DropDownMenuGroup)rootParent).menuButtonDownCount++; }
                        else
                            index = (index + count + 1) % count;
                    }
                    else if (this.Parent is RibbonItemsControl && ((RibbonItemsControl)this.Parent).downCount == 0)
                    { index = 0; ((RibbonItemsControl)this.Parent).downCount++; }

                    else
                        index = (index + count + 1) % count;
                }
                else if (enteredKey == Key.Home)
                {
                    index = 0;
                    if (rootParent is DropDownMenuGroup)
                        ((DropDownMenuGroup)rootParent).menuButtonDownCount++;
                    else if (rootParent is RibbonItemsControl)
                        ((RibbonItemsControl)this.Parent).downCount++;
                }
                else
                {
                    index = count - 1;
                    if (rootParent is DropDownMenuGroup)
                        ((DropDownMenuGroup)rootParent).menuButtonDownCount++;
                    else if (rootParent is RibbonItemsControl)
                        ((RibbonItemsControl)this.Parent).downCount++;
                }

                var containerObj = parent.ItemContainerGenerator.ContainerFromIndex(index) as object;
                if (containerObj.GetType().Name == "ApplicationMenuButton")
                {
                    ApplicationMenuButton container = containerObj as ApplicationMenuButton;
                    if (null != container)
                    {
                        if (container.IsEnabled)
                        {
                            container.IsMouseOver = true;
                            container.Focus();
                            break;
                        }
                    }
                }
                else if (containerObj.GetType().Name == "ApplicationSplitButton")
                {
                    ApplicationSplitButton container = containerObj as ApplicationSplitButton;
                    if (null != container)
                    {
                        if (container.IsEnabled)
                        {
                            container.IsMouseOver = true;
                            container.Focus();
                            break;
                        }
                    }
                }
            }
            while (index != startingIndex);
        }

        private void HideMouseOverItems(object parentObj)
        {
            if (parentObj.GetType().Name == "RibbonItemsControl")
            {
                var parent = parentObj as RibbonItemsControl;
                foreach (var item in parent.Items)
                {
                    if (item.GetType().Name == "ApplicationSplitButton")
                    {
                        var menuItem = item as ApplicationSplitButton;
                        if (menuItem != null)
                            if (menuItem.IsMouseOver)
                                menuItem.IsMouseOver = false;
                    }

                    else if (item.GetType().Name == "ApplicationMenuButton")
                    {
                        var menuItem = item as ApplicationMenuButton;
                        if (menuItem != null)
                            if (menuItem.IsMouseOver)
                                menuItem.IsMouseOver = false;
                    }
                }
            }

            else if (parentObj.GetType().Name == "RibbonMenuGroup")
            {
                var parent = parentObj as DropDownMenuGroup;
                foreach (var item in parent.Items)
                {
                    var menuItem = item as ApplicationMenuButton;
                    if (menuItem != null)
                    {
                        if (menuItem.IsMouseOver)
                            menuItem.IsMouseOver = false;
                    }
                }
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
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
                        Icon = RibbonCommandManager.CommandDictionary[Command].SmallIcon;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].Label != null)
                    {
                        Label = RibbonCommandManager.CommandDictionary[Command].Label;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].ToolTip != null)
                    {
                        ToolTip = RibbonCommandManager.CommandDictionary[Command].ToolTip;
                    }
                }
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            HideMouseOverItems(this.Parent);
            this.Focus();
            this.IsMouseOver = true;
            VisualStateManager.GoToState(this, "MouseOver", false);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
            VisualStateManager.GoToState(this, "Normal", false);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> routed event.
        /// </summary>
        protected override void OnClick()
        {
            var parentrbnMenuGrp = VisualUtils.FindAncestor(this, typeof(RibbonMenuGroup));
            if (parentrbnMenuGrp != null && (parentrbnMenuGrp as RibbonMenuGroup).parentApplicationMenu != null && this.parentApplicationMenu == null)
            {
                this.parentApplicationMenu = (parentrbnMenuGrp as RibbonMenuGroup).parentApplicationMenu;
            }

            if (parentApplicationMenu != null)
            {
                if (parentApplicationMenu.PART_RibbonDropDown != null)
                {
                    parentApplicationMenu.PART_RibbonDropDown.IsOpen = false;
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
            base.OnClick();
        }

        /// <summary>
        /// Handles the Closed event of the AppMenuPopUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void AppMenuPopUp_Closed(object sender, EventArgs e)
        {
            //base.OnClick();
            //appmenu.AppMenuPopUp.Closed -= new EventHandler(AppMenuPopUp_Closed);
        }
        #endregion
    }
}
