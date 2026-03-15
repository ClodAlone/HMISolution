#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Resources;
using System.IO;
using System.Windows.Markup;
using System.Windows.Threading;
namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents a selectable item inside a Menu or ContextMenuAdv.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContextMenuItemAdv))]

    public class ContextMenuItemAdv : HeaderedItemsControl // , ICommandSource // ICommandSource not defined by Silverlight 4
    {
        /// <summary>
        /// Gets or sets the sub menu border thickness.
        /// </summary>
        /// <value>The sub menu border thickness.</value>
        public Thickness SubMenuBorderThickness
        {
            get { return (Thickness)GetValue(SubMenuBorderThicknessProperty); }
            set { SetValue(SubMenuBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the SubMenuBorderThicknessProperty.
        /// </summary>
        public static readonly DependencyProperty SubMenuBorderThicknessProperty =
            DependencyProperty.Register("SubMenuBorderThickness", typeof(Thickness), typeof(ContextMenuItemAdv), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Gets or sets the sub menu background.
        /// </summary>
        /// <value>The sub menu background.</value>
        public Brush SubMenuBackground
        {
            get { return (Brush)GetValue(SubMenuBackgroundProperty); }
            set { SetValue(SubMenuBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the SubMenuBackgroundProperty.
        /// </summary>
        public static readonly DependencyProperty SubMenuBackgroundProperty =
            DependencyProperty.Register("SubMenuBackground", typeof(Brush), typeof(ContextMenuItemAdv), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the sub menu border brush.
        /// </summary>
        /// <value>The sub menu border brush.</value>
        public Brush SubMenuBorderBrush
        {
            get { return (Brush)GetValue(SubMenuBorderBrushProperty); }
            set { SetValue(SubMenuBorderBrushProperty, value); }
        }
        /// <summary>
        /// Identifies the SubMenuBorderBrushProperty.
        /// </summary>
        public static readonly DependencyProperty SubMenuBorderBrushProperty =
            DependencyProperty.Register("SubMenuBorderBrush", typeof(Brush), typeof(ContextMenuItemAdv), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }
        /// <summary>
        /// Identifies the IsCheckableProperty.
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof(bool), typeof(ContextMenuItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckablePropertyChanged)));

        /// <summary>
        /// Called when [is checkable property changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsCheckablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((ContextMenuItemAdv)obj).IsCheckable == false)
            {
                ((ContextMenuItemAdv)obj).RadioButtonVisibility = Visibility.Collapsed;
                ((ContextMenuItemAdv)obj).CheckBoxVisibility = Visibility.Collapsed;
            }
            else
            {
                if (((ContextMenuItemAdv)obj).IsChecked)
                {
                    ((ContextMenuItemAdv)obj).CheckBoxVisibility = ((ContextMenuItemAdv)obj).CheckIconType == CheckIconType.CheckBox ? Visibility.Visible : Visibility.Collapsed;
                    ((ContextMenuItemAdv)obj).RadioButtonVisibility = ((ContextMenuItemAdv)obj).CheckIconType == CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsCheckedProperty.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ContextMenuItemAdv), new PropertyMetadata(false, OnIsCheckedPropertyChanged));

        /// <summary>
        /// Called when [is checked property changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsCheckedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((ContextMenuItemAdv)obj).IsCheckable == true)
            {
                ((ContextMenuItemAdv)obj).OnIsCheckedPropertyChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsCheckedPropertyChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsCheckedPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsChecked)
            {
                this.CheckBoxVisibility = this.CheckIconType == CheckIconType.CheckBox ? Visibility.Visible : Visibility.Collapsed;
                this.RadioButtonVisibility = this.CheckIconType == CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                if (null != Checked)
                {
                    Checked(this, new RoutedEventArgs());
                }
            }
            else
            {
                this.CheckBoxVisibility = Visibility.Collapsed;
                this.RadioButtonVisibility = Visibility.Collapsed;
                if (null != UnChecked)
                {
                    UnChecked(this, new RoutedEventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets the check box visibility.
        /// </summary>
        /// <value>The check box visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]        
        [BrowsableAttribute(false)] 
        public Visibility CheckBoxVisibility
        {
            get { return (Visibility)GetValue(CheckBoxVisibilityProperty); }
            set { SetValue(CheckBoxVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the CheckBoxVisibilityProperty.
        /// </summary>
        public static readonly DependencyProperty CheckBoxVisibilityProperty =
            DependencyProperty.Register("CheckBoxVisibility", typeof(Visibility), typeof(ContextMenuItemAdv), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the radio button visibility.
        /// </summary>
        /// <value>The radio button visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]        
        public Visibility RadioButtonVisibility        {
            get { return (Visibility)GetValue(RadioButtonVisibilityProperty); }
            set { SetValue(RadioButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the RadioButtonVisibilityProperty.
        /// </summary>
        public static readonly DependencyProperty RadioButtonVisibilityProperty =
            DependencyProperty.Register("RadioButtonVisibility", typeof(Visibility), typeof(ContextMenuItemAdv), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }
        /// <summary>
        /// Identifies the GroupNameProperty.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(ContextMenuItemAdv), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the type of the check icon.
        /// </summary>
        /// <value>The type of the check icon.</value>
        public CheckIconType CheckIconType
        {
            get { return (CheckIconType)GetValue(CheckIconTypeProperty); }
            set { SetValue(CheckIconTypeProperty, value); }
        }

        /// <summary>
        /// Identifies the CheckIconTypeProperty
        /// </summary>
        public static readonly DependencyProperty CheckIconTypeProperty =
            DependencyProperty.Register("CheckIconType", typeof(CheckIconType), typeof(ContextMenuItemAdv), new PropertyMetadata(CheckIconType.CheckBox, new PropertyChangedCallback(OnTypePropertyChanged)));

        /// <summary>
        /// Called when [type property changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((ContextMenuItemAdv)obj).IsCheckable == true)
            {
                if (((ContextMenuItemAdv)obj).IsChecked)
                {
                    ((ContextMenuItemAdv)obj).CheckBoxVisibility = ((ContextMenuItemAdv)obj).CheckIconType == CheckIconType.CheckBox ? Visibility.Visible : Visibility.Collapsed;
                    ((ContextMenuItemAdv)obj).RadioButtonVisibility = ((ContextMenuItemAdv)obj).CheckIconType == CheckIconType.RadioButton ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    ((ContextMenuItemAdv)obj).CheckBoxVisibility = Visibility.Collapsed;
                    ((ContextMenuItemAdv)obj).RadioButtonVisibility = Visibility.Collapsed;
                }
            }
        }


        /// <summary>
        /// Gets or sets the extend button visibility.
        /// </summary>
        /// <value>The extend button visibility.</value>
        public Visibility ExtendButtonVisibility
        {
            get { return (Visibility)GetValue(ExtendButtonVisibilityProperty); }
            set { SetValue(ExtendButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the ExtendButtonVisibilityProperty.
        /// </summary>
        public static readonly DependencyProperty ExtendButtonVisibilityProperty =
            DependencyProperty.Register("ExtendButtonVisibility", typeof(Visibility), typeof(ContextMenuItemAdv), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the parent menu item.
        /// </summary>
        /// <value>The parent menu item.</value>
        public ContextMenuItemAdv ParentMenuItem
        {
            get { return (ContextMenuItemAdv)GetValue(ParentMenuItemProperty); }
            set { SetValue(ParentMenuItemProperty, value); }
        }

        /// <summary>
        /// Identifies the ParentMenuItemProperty.
        /// </summary>
        public static readonly DependencyProperty ParentMenuItemProperty =
            DependencyProperty.Register("ParentMenuItem", typeof(ContextMenuItemAdv), typeof(ContextMenuItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]        
		public  String Theme        {
            get { return (string)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        /// <summary>
        /// Identifies the ThemeProperty.
        /// </summary>
        internal static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(string), typeof(ContextMenuItemAdv), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the IsOpenProperty.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ContextMenuItemAdv), new PropertyMetadata(false));

        /// <summary>
        /// Called when [is open changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (((ContextMenuItemAdv)obj).ParentMenuItem != null)
            {
                ((ContextMenuItemAdv)obj).ParentMenuItem.IsOpen = !((ContextMenuItemAdv)obj).ParentMenuItem.IsOpen;
            }
        }

        /// <summary>
        /// Occurs when a ContextMenuItemAdv is clicked.
        /// </summary>
        public event RoutedEventHandler Click;
        /// <summary>
        /// Occurs when [checked].
        /// </summary>
        public event RoutedEventHandler Checked;
        /// <summary>
        /// Occurs when [un checked].
        /// </summary>
        public event RoutedEventHandler UnChecked;

        /// <summary>
        /// Stores a value indicating whether this element has logical focus.
        /// </summary>
        private bool _isFocused;

        /// <summary>
        /// Gets or sets a reference to the MenuBase parent.
        /// </summary>
        //internal MenuBase ParentMenuBase { get; set; }
        internal ContextMenuAdv ParentMenuBase { get; set; }

        /// <summary>
        /// Gets or sets the command associated with the menu item.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ContextMenuItemAdv),
            new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Handles changes to the Command DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ContextMenuItemAdv)o).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnCommandChanged(ICommand oldValue, ICommand newValue)
        {
            if (null != oldValue)
            {
                oldValue.CanExecuteChanged -= new EventHandler(HandleCanExecuteChanged);
            }
            if (null != newValue)
            {
                newValue.CanExecuteChanged += new EventHandler(HandleCanExecuteChanged);
            }
            UpdateIsEnabled();
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the Command property of a ContextMenuItemAdv.
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(ContextMenuItemAdv),
            new PropertyMetadata(null, OnCommandParameterChanged));

        /// <summary>
        /// Handles changes to the CommandParameter DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnCommandParameterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ContextMenuItemAdv)o).OnCommandParameterChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Handles changes to the CommandParameter property.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnCommandParameterChanged(object oldValue, object newValue)
        {
            UpdateIsEnabled();
        }

        /// <summary>
        /// Gets or sets the icon that appears in a ContextMenuItemAdv.
        /// </summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Identifies the Icon dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(object),
            typeof(ContextMenuItemAdv),
            new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the ContextMenuItemAdv class.
        /// </summary>
        public ContextMenuItemAdv()
        {
            DefaultStyleKey = typeof(ContextMenuItemAdv);
            UpdateIsEnabled();
        }

        /// <summary>
        /// Invoked whenever an unhandled GotFocus event reaches this element in its route.
        /// </summary>
        /// <param name="e">A RoutedEventArgs that contains event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Raises the LostFocus routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">A RoutedEventArgs that contains event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Called whenever the mouse enters a ContextMenuItemAdv.
        /// </summary>
        /// <param name="e">The event data for the MouseEnter event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Focus();
            ChangeVisualState(true);
            if (ParentMenuBase != null)
            {
                if (!this.ParentMenuBase.OpenOnClick)
                {
                    this.HandleOpenClsoe();
                }
            }
        }

        /// <summary>
        /// Called whenever the mouse leaves a ContextMenuItemAdv.
        /// </summary>
        /// <param name="e">The event data for the MouseLeave event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (null != ParentMenuBase)
            {
                ParentMenuBase.Focus();
            }
            ChangeVisualState(true);
        }

        /// <summary>
        /// Called when the left mouse button is pressed.
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonUp event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                OnClick();
                e.Handled = true;
            }
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Called when the right mouse button is pressed.
        /// </summary>
        /// <param name="e">The event data for the MouseRightButtonUp event.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                OnClick();
                e.Handled = true;
            }
            base.OnMouseRightButtonUp(e);
        }

        /// <summary>
        /// Responds to the KeyDown event.
        /// </summary>
        /// <param name="e">The event data for the KeyDown event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled && (Key.Enter == e.Key))
            {
                OnClick();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Called when the Items property changes.
        /// </summary>
        /// <param name="e">The event data for the ItemsChanged event.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Called when a ContextMenuItemAdv is clicked and raises a Click event.
        /// </summary>
        protected virtual void OnClick()
        {
            if (this.IsCheckable == true)
            {
                if (this.CheckIconType == CheckIconType.CheckBox)
                {
                    this.IsChecked = !this.IsChecked;
                    //this.CheckBoxVisibility = this.CheckBoxVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    if (this.GroupName.Equals(string.Empty))
                        this.IsChecked = true;//this.RadioButtonVisibility = Visibility.Visible;//this.RadioButtonVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                    else
                    {
                        this.IsChecked = true;//this.RadioButtonVisibility = Visibility.Visible;//this.RadioButtonVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                        this.UnCheck(this);
                    }
                }
            }

            ContextMenuAdv contextMenu = ParentMenuBase as ContextMenuAdv;
            if (null != contextMenu)
            {
                if (this.Items.Count == 0)
                {
                    contextMenu.ChildMenuItemClicked();
                }
                else
                {
                    this.HandleOpenClsoe();
                }
            }
            
            // Wrapping the remaining code in a call to Dispatcher.BeginInvoke provides
            // WPF-compatibility by allowing the ContextMenuAdv to close before the command
            // executes. However, it breaks the Clipboard.SetText scenario because the
            // call to SetText is no longer in direct response to user input.
            RoutedEventHandler handler = Click;
            if (null != handler)
            {
                handler(this, new RoutedEventArgs());
            }
            if ((null != Command) && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }
        private void RegisterEvents()
        {
            if (panel != null)
            {
                panel.LayoutUpdated -= new EventHandler(panel_LayoutUpdated);
                panel.LayoutUpdated += new EventHandler(panel_LayoutUpdated);
            }
            if (this.PART_TopScroll != null)
            {
                if(ScrollerTick != null)
                    ScrollerTick.Tick -= new EventHandler(ScrollUp_Tick);
                ScrollerTick = new DispatcherTimer();
                this.PART_TopScroll.MouseEnter -= new MouseEventHandler(PART_TopScroll_MouseEnter);
                this.PART_TopScroll.MouseLeave -= new MouseEventHandler(PART_TopScroll_MouseLeave);
                ScrollerTick.Tick += new EventHandler(ScrollUp_Tick);
                this.PART_TopScroll.MouseEnter += new MouseEventHandler(PART_TopScroll_MouseEnter);
                this.PART_TopScroll.MouseLeave += new MouseEventHandler(PART_TopScroll_MouseLeave);
            }

            if (this.PART_BottomScroll != null)
            {
                this.PART_BottomScroll.MouseEnter -= new MouseEventHandler(PART_BottomScroll_MouseEnter);
                this.PART_BottomScroll.MouseLeave -= new MouseEventHandler(PART_BottomScroll_MouseLeave);
                this.PART_BottomScroll.MouseEnter += new MouseEventHandler(PART_BottomScroll_MouseEnter);
                this.PART_BottomScroll.MouseLeave += new MouseEventHandler(PART_BottomScroll_MouseLeave);
            }

            if (this.PART_Popup != null)
            {
                this.PART_Popup.LayoutUpdated -= new EventHandler(PART_Popup_LayoutUpdated);
                this.PART_Popup.LayoutUpdated += new EventHandler(PART_Popup_LayoutUpdated);
            }
        }

        private void PART_TopScroll_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollerTick.Stop();
            if (this.IsOpen == false)
                VisualStateManager.GoToState(PART_TopScroll, "Normal", false);
        }

        private void PART_BottomScroll_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollerTick.Stop();
            if (this.IsOpen == false)
            {
                VisualStateManager.GoToState(PART_BottomScroll, "Normal", false);
            }
        }

        private void PART_BottomScroll_MouseEnter(object sender, MouseEventArgs e)
        {
            ScrollerTick.Start();

            scrollableValue = 1.5;
        }

        private void PART_TopScroll_MouseEnter(object sender, MouseEventArgs e)
        {
            ScrollerTick.Start();

            scrollableValue = -1.5;
        }
        private void UnCheck(ContextMenuItemAdv menuitem)
        {
            if (this.ParentMenuItem != null)
            {
                for (int i = 0; i < this.ParentMenuItem.Items.Count; i++)
                {
                    if (this.ParentMenuItem.Items[i] is ContextMenuItemAdv)
                    {
                        ContextMenuItemAdv _item = (ContextMenuItemAdv)this.ParentMenuItem.Items[i];
                        if (_item is ContextMenuItemAdv && _item != menuitem && !_item.GroupName.Equals(string.Empty))
                        {
                            if (_item.IsCheckable == true && _item.CheckIconType == CheckIconType.RadioButton && _item.GroupName.Equals(menuitem.GroupName))
                            {
                                _item.IsChecked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.ParentMenuBase.Items.Count; i++)
                {
                    if (this.ParentMenuBase.Items[i] is ContextMenuItemAdv)
                    {
                        ContextMenuItemAdv _item = (ContextMenuItemAdv)this.ParentMenuBase.Items[i];
                        if (_item is ContextMenuItemAdv && _item != menuitem && !_item.GroupName.Equals(string.Empty))
                        {
                            if (_item.IsCheckable == true && _item.CheckIconType == CheckIconType.RadioButton)
                            {
                                _item.IsChecked = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the CanExecuteChanged event of the Command property.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateIsEnabled();
        }

        /// <summary>
        /// Updates the IsEnabled property.
        /// </summary>
        /// <remarks>
        /// WPF overrides the local value of IsEnabled according to ICommand, so Silverlight does, too.
        /// </remarks>
        private void UpdateIsEnabled()
        {
            IsEnabled = (null == Command) || Command.CanExecute(CommandParameter);
            ChangeVisualState(true);
        }

        /// <summary>
        /// Changes to the correct visual state(s) for the control.
        /// </summary>
        /// <param name="useTransitions">True to use transitions; otherwise false.</param>
        protected virtual void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, "Disabled", useTransitions);
                ChangeTonormalState();
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
                ChangeTonormalState();
            }

            if (_isFocused && IsEnabled)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
                ChangeToMouseOverState();
            }
            else
            {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
                ChangeTonormalState();
            }

            if (IsOpen == true && _isFocused==false)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
                ChangeToMouseOverState();
            }
            //else
            //{
            //    VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            //}
        }

        /// <summary>
        /// Method implementation for ChangetoMouseoverstate
        /// </summary>
        public void ChangeToMouseOverState()
        {
            //if (Theme == "Blend")
            //{
            //    this.Foreground = new SolidColorBrush(FromHex("#FF353535"));
            //    if (this.ExtendBtnPath != null)
            //    {
            //        this.ExtendBtnPath.Fill = new SolidColorBrush(FromHex("#FF353535"));
            //        this.ExtendBtnPath.Stroke = new SolidColorBrush(FromHex("#FF353535"));
            //        this.CheckBoxBorder.Background = new SolidColorBrush(FromHex("#FFAAAAAA"));
            //        this.CheckBoxBorder.BorderBrush = new SolidColorBrush(FromHex("#FFAAAAAA"));
            //        this.CheckBoxTick.Fill = new SolidColorBrush(FromHex("#FF353535"));
            //        this.RadioBtnBorder.Background = new SolidColorBrush(FromHex("#FFAAAAAA"));
            //        this.RadioBtnBorder.BorderBrush = new SolidColorBrush(FromHex("#FFAAAAAA"));
            //        this.RadioBtn.Fill = new SolidColorBrush(FromHex("#FF353535"));
            //    }
            //}
        }

        /// <summary>
        /// Method implemetation for ChangeTonormalState
        /// </summary>
        public void ChangeTonormalState()
        {
        //    if (Theme == "Blend")
        //    {
        //        this.Foreground = new SolidColorBrush(FromHex("#FFFFFFFF"));
        //        if (this.ExtendBtnPath != null)
        //        {
        //            this.ExtendBtnPath.Fill = new SolidColorBrush(FromHex("#FFFFFFFF"));
        //            this.ExtendBtnPath.Stroke = new SolidColorBrush(FromHex("#FFFFFFFF"));
        //            this.CheckBoxBorder.Background = new SolidColorBrush(FromHex("#FF3A3A3A"));
        //            this.CheckBoxBorder.BorderBrush = new SolidColorBrush(FromHex("#FF3A3A3A"));
        //            this.CheckBoxTick.Fill = new SolidColorBrush(FromHex("#FFFFFFFF"));
        //            this.RadioBtnBorder.Background = new SolidColorBrush(FromHex("#FF3A3A3A"));
        //            this.RadioBtnBorder.BorderBrush = new SolidColorBrush(FromHex("#FF3A3A3A"));
        //            this.RadioBtn.Fill = new SolidColorBrush(FromHex("#FFFFFFFF"));
        //        }
        //    }
        }

        /// <summary>
        /// Return color value from the string
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public Color FromHex(string hex)
        {
            string v = hex.TrimStart('#');
            if (v.Length > 8)
                return Colors.Blue;
            if (v.Length == 6)
                v = "FF" + v;
            if (v.Length < 6)
                v = "FF" + v;
            while (v.Length < 8)
                v += "0";
            Color c = new Color();
            c.A = (byte)System.Convert.ToInt32(v.Substring(0, 2), 16);
            c.R = (byte)System.Convert.ToInt32(v.Substring(2, 2), 16);
            c.G = (byte)System.Convert.ToInt32(v.Substring(4, 2), 16);
            c.B = (byte)System.Convert.ToInt32(v.Substring(6, 2), 16);
            return c;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The container element used to display the specified item.</param><param name="item">The content to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ContextMenuItemAdv menuItem = element as ContextMenuItemAdv;
            if (null != menuItem)
            {
                menuItem.ParentMenuBase = this.ParentMenuBase;
                menuItem.ParentMenuItem = this;
                menuItem.SubMenuBackground = this.SubMenuBackground;
                menuItem.SubMenuBorderBrush = this.SubMenuBorderBrush;
                menuItem.SubMenuBorderThickness = this.SubMenuBorderThickness;

                if (menuItem != item)
                {
                    // Copy the ItemsControl properties from parent to child
                    DataTemplate itemTemplate = ItemTemplate;
                    Style itemContainerStyle = ItemContainerStyle;
                    if (itemTemplate != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);
                    }
                    if (itemContainerStyle != null && HasDefaultValue(menuItem, HeaderedItemsControl.ItemContainerStyleProperty))
                    {
                        menuItem.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, itemContainerStyle);
                    }

                    // Copy the Header properties from parent to child
                    if (HasDefaultValue(menuItem, HeaderedItemsControl.HeaderProperty))
                    {
                        menuItem.Header = item;
                    }
                    if (itemTemplate != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.HeaderTemplateProperty, itemTemplate);
                    }
                    if (itemContainerStyle != null)
                    {
                        menuItem.SetValue(HeaderedItemsControl.StyleProperty, itemContainerStyle);
                    }
                    HierarchicalDataTemplate hierarchicaltemplate = this.ItemTemplate as HierarchicalDataTemplate;
                    if (hierarchicaltemplate.ItemsSource != null)
                        menuItem.SetBinding(HeaderedItemsControl.ItemsSourceProperty, GetBinding(hierarchicaltemplate, menuItem));
                }
                if (menuItem.Items.Count > 0)
                {
                    menuItem.ExtendButtonVisibility = Visibility.Visible;
                }
                else
                {
                    menuItem.ExtendButtonVisibility = Visibility.Collapsed;
                }

                CheckGroupName(menuItem);
            }
        }

        internal void CheckGroupName(ContextMenuItemAdv menuitem)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is ContextMenuItemAdv && this.Items[i] == menuitem)
                {
                    ContextMenuItemAdv _item = (ContextMenuItemAdv)this.Items[i];
                    if (_item.IsCheckable == true && _item.CheckIconType == CheckIconType.RadioButton && !_item.GroupName.Equals(string.Empty))
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (this.Items[j] is ContextMenuItemAdv)
                            {
                                ContextMenuItemAdv _previouseitem = this.Items[j] as ContextMenuItemAdv;
                                if (_previouseitem.IsCheckable == true && _previouseitem.CheckIconType == CheckIconType.RadioButton && !_previouseitem.GroupName.Equals(string.Empty) && _item.GroupName.Equals(_previouseitem.GroupName) && _previouseitem.IsChecked)
                                {
                                    _item.IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private Binding GetBinding(HierarchicalDataTemplate template, ContextMenuItemAdv item)
        {
            Binding binding = new Binding();
            binding.Converter = template.ItemsSource.Converter;
            binding.ConverterCulture = template.ItemsSource.ConverterCulture;
            binding.ConverterParameter = template.ItemsSource.ConverterParameter;
            binding.Mode = template.ItemsSource.Mode;
            binding.NotifyOnValidationError = template.ItemsSource.NotifyOnValidationError;
            binding.Path = template.ItemsSource.Path;
            binding.Source = item.Header;
            binding.ValidatesOnExceptions = template.ItemsSource.ValidatesOnExceptions;
            return binding;
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        /// <param name="item">The item to check.</param>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is ContextMenuItemAdv) || (item is SeparatorAdv));
            //return item is ContextMenuItemAdv;
            //return base.IsItemItsOwnContainerOverride(item);
        }

        internal Button PART_BottomScroll;

        internal Button PART_TopScroll;

        internal ScrollViewer PART_ScrollViewer;

        private DispatcherTimer ScrollerTick;

        private double scrollableValue = 2;

        internal ContentPresenter Presenter;
        internal Popup PART_Popup;
        private bool ScrollabilityEnabled = false;
        internal bool IsBoundaryDetected = false;
        internal Border PopupBorder;
        internal Grid PopupGrid;
        private Border CheckBoxBorder;
        private System.Windows.Shapes.Path CheckBoxTick;
        private Border RadioBtnBorder;
        private Ellipse RadioBtn;
        private System.Windows.Shapes.Path ExtendBtnPath;
        private ContentControl PART_Content;
        StackPanel panel;
        private double PanelHeight;

        internal bool isloaded=false;
        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            panel = this.GetStackPanel();
           

            ChangeVisualState(false);
            if (PART_Popup != null)
            {
                PART_Popup.Child = null;
                PART_Popup = null;
            }
            
            if (PART_Content != null)
            {
                PART_Content.Content = null;
                PART_Content = null;
            }

            PART_Content = this.GetTemplateChild("PART_Content") as ContentControl;
            if (PART_Content != null)
            {
                this.PART_Content.Content = this.Icon;
            }
            Presenter = this.GetTemplateChild("Presenter") as ContentPresenter;
            PART_Popup = this.GetTemplateChild("PART_Popup") as Popup;
            PopupGrid = this.GetTemplateChild("PopupGrid") as Grid;
            PopupBorder = this.GetTemplateChild("PopupBorder") as Border;
            this.PART_TopScroll = this.GetTemplateChild("PART_TopScroll") as Button;
            this.PART_BottomScroll = this.GetTemplateChild("PART_BottomScroll") as Button;
            this.PART_ScrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            this.CheckBoxBorder = GetTemplateChild("GlyphPanel") as Border;
            this.CheckBoxTick = GetTemplateChild("Glyph") as System.Windows.Shapes.Path;
            this.RadioBtnBorder = GetTemplateChild("GlyphPanel1") as Border;
            this.RadioBtn = GetTemplateChild("Background") as Ellipse;
            this.ExtendBtnPath = GetTemplateChild("extendPath") as System.Windows.Shapes.Path;
            RegisterEvents();
            if (this.ParentMenuBase != null && !this.ParentMenuBase.OverrideVisualStyle)
            {
                if (this.ParentMenuItem != null)
                    this.Style = this.ParentMenuItem.Style;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.ItemsSource != null)
                    {
                        ContextMenuItemAdv m_treeViewItem = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as ContextMenuItemAdv;
                        if (m_treeViewItem != null && m_treeViewItem is ContextMenuItemAdv)
                        {
                            m_treeViewItem.Style = this.Style;
                        }
                    }
                    else
                    {
                        if (this.Items[i] is ContextMenuItemAdv)
                            (this.Items[i] as ContextMenuItemAdv).Style = this.Style;
                    }
                }
            }

            if (this.ParentMenuItem != null)
            {
                if (this.ItemsSource != null)
                {
                    int index = this.ParentMenuItem.Items.IndexOf(this.Header);
                    if (index == this.ParentMenuItem.Items.Count - 1)
                    {
                        this.ParentMenuItem.SetPopupPosition();
                    }
                }
                else
                {
                    int index = this.ParentMenuItem.Items.IndexOf(this);
                    if (index == this.ParentMenuItem.Items.Count - 1)
                    {
                        this.ParentMenuItem.SetPopupPosition();
                    }
                }
            }
            this.Loaded -= new RoutedEventHandler(ContextMenuItemAdv_Loaded);
            this.Loaded += new RoutedEventHandler(ContextMenuItemAdv_Loaded);
            this.Unloaded -= new RoutedEventHandler(ContextMenuItemAdv_Unloaded);
            this.Unloaded += new RoutedEventHandler(ContextMenuItemAdv_Unloaded);
        }

        void ContextMenuItemAdv_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents();
            if (this.Items.Count > 0 && this.IsOpen)
            {
                this.isloaded = true;
                CheckForBoundaryCollision();
                this.isloaded = false;
            }
        }

        private void ScrollUp_Tick(object sender, EventArgs e)
        {
            this.PART_ScrollViewer.ScrollToVerticalOffset(this.PART_ScrollViewer.VerticalOffset + scrollableValue);
        }
        void PART_Popup_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.ParentMenuBase != null )
            {
                if (this.ParentMenuItem != null && this.ParentMenuItem.PART_ScrollViewer!=null && this.ParentMenuItem.ScrollabilityEnabled)
                {
                    if ((this.ParentMenuItem.PART_ScrollViewer.ScrollableHeight - this.ParentMenuItem.PART_ScrollViewer.VerticalOffset) > 0)
                    {
                        this.ParentMenuItem.PART_BottomScroll.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.ParentMenuItem.PART_BottomScroll.Visibility = System.Windows.Visibility.Collapsed;
                        VisualStateManager.GoToState(this.ParentMenuItem.PART_BottomScroll, "Normal", false);

                        this.ParentMenuItem.ScrollerTick.Stop();

                    }

                    if (this.ParentMenuItem.PART_ScrollViewer.VerticalOffset > 0)
                    {
                        this.ParentMenuItem.PART_TopScroll.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.ParentMenuItem.PART_TopScroll.Visibility = System.Windows.Visibility.Collapsed;
                        VisualStateManager.GoToState(this.ParentMenuItem.PART_TopScroll, "Normal", false);

                        this.ParentMenuItem.ScrollerTick.Stop();

                    }


                }
            }
            else if (this.ParentMenuItem != null)
            {
                if (this.ParentMenuItem.PART_TopScroll.Visibility == System.Windows.Visibility.Visible)
                    this.ParentMenuItem.PART_TopScroll.Visibility = System.Windows.Visibility.Collapsed;

                if (this.ParentMenuItem.PART_BottomScroll.Visibility == System.Windows.Visibility.Visible)
                    this.ParentMenuItem.PART_BottomScroll.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void ContextMenuItemAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(ContextMenuItemAdv_Unloaded);
            if (panel != null)
            {
                panel.LayoutUpdated -= new EventHandler(panel_LayoutUpdated);
            }
            if (this.PART_TopScroll != null)
            {
                if (ScrollerTick != null)
                    ScrollerTick.Tick -= new EventHandler(ScrollUp_Tick);               
                this.PART_TopScroll.MouseEnter -= new MouseEventHandler(PART_TopScroll_MouseEnter);
                this.PART_TopScroll.MouseLeave -= new MouseEventHandler(PART_TopScroll_MouseLeave);
            }

            if (this.PART_BottomScroll != null)
            {
                this.PART_BottomScroll.MouseEnter -= new MouseEventHandler(PART_BottomScroll_MouseEnter);
                this.PART_BottomScroll.MouseLeave -= new MouseEventHandler(PART_BottomScroll_MouseLeave);
            }

            if (this.PART_Popup != null)
            {
                this.PART_Popup.LayoutUpdated -= new EventHandler(PART_Popup_LayoutUpdated);
            }
        }

        void panel_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.ParentMenuItem != null)
            {
                if (this.ParentMenuItem.PanelWidth != this.panel.ActualWidth)
                {
                    if (this.ParentMenuItem.IsOpen == true)
                    {
                        this.ParentMenuItem.PanelWidth = this.panel.ActualWidth;
                        this.ParentMenuItem.PanelHeight = this.panel.ActualHeight;
                    }
                    else
                    {
                    }
                    //this.ParentMenuItem.SetPopupPosition();
                }
            }
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContextMenuItemAdv();
            //return base.GetContainerForItemOverride();
        }

        /// <summary>
        /// Checks whether a control has the default value for a property.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>True if the property has the default value; false otherwise.</returns>
        private static bool HasDefaultValue(Control control, DependencyProperty property)
        {
            return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
        }

        internal void ClosePopup()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is ContextMenuItemAdv)
                {
                    if ((this.Items[i] as ContextMenuItemAdv) != null)
                    {
                        ((ContextMenuItemAdv)this.Items[i]).ClosePopup();
                    }
                }
                else
                {
                    ContextMenuItemAdv m_menuitem = this.ItemContainerGenerator.ContainerFromIndex(i) as ContextMenuItemAdv;
                    if (m_menuitem != null)
                    {
                        m_menuitem.ClosePopup();
                    }
                }
                this.IsOpen = false;
                this.ChangeVisualState(true);                    
            }
        }

        private void HandleOpenClsoe()
        {
            if (this.ParentMenuBase.FocusedElement == this)
            {
                this.ClosePopup();
                this.IsOpen = this.Items.Count > 0 ? true : false;
            }
            else if (this.ParentMenuBase.FocusedElement == null)
            {
                this.IsOpen = this.Items.Count > 0 ? true : false;//this.IsOpen = true;
            }
            else
            {
                if (this.ParentMenuItem == null)
                {
                    this.ParentMenuBase.ClsoeSubPopup();
                    this.IsOpen = this.Items.Count > 0 ? true : false;//this.IsOpen = true;
                }

                else if (this.ParentMenuBase.FocusedElement.ParentMenuItem == this.ParentMenuItem)
                {
                    this.ParentMenuBase.FocusedElement.ClosePopup();
                    this.IsOpen = this.Items.Count > 0 ? true : false;//this.IsOpen = true;
                }

                else
                {
                    ContextMenuItemAdv _menuitem = this;
                    bool flag = false;
                    while (_menuitem.ParentMenuItem != null)
                    {
                        if (this.ParentMenuBase.FocusedElement == _menuitem.ParentMenuItem)
                        {
                            flag = true;
                            break;
                        }
                        else
                        {
                            _menuitem = _menuitem.ParentMenuItem;
                        }
                    }
                    if (flag == true)
                    {
                        _menuitem.ClosePopup();
                        this.IsOpen = this.Items.Count > 0 ? true : false;//this.IsOpen = true;
                    }
                    else
                    {
                        _menuitem = this.ParentMenuBase.FocusedElement;
                        flag = false;
                        while (_menuitem.ParentMenuItem != null)
                        {
                            if (this.ParentMenuItem == _menuitem.ParentMenuItem)
                            {
                                flag = true;
                                break;
                            }
                            else
                            {
                                _menuitem = _menuitem.ParentMenuItem;
                            }
                        }
                        if (flag == true)
                        {
                            _menuitem.ClosePopup();
                            this.IsOpen = this.Items.Count > 0 ? true : false;//this.IsOpen = true;
                        }
                    }
                }
            }
            this.ParentMenuBase.FocusedElement = this;
            if (this.IsOpen == true)
            {
                this.SetPopupPosition();
            }
        }


        internal void SetPopupPosition()
        {
            //StackPanel panel = this.GetStackPanel();
            this.CheckForBoundaryCollision();
            //System.ComponentModel.DesignerProperties.IsInDesignTool
        }

        internal StackPanel GetStackPanel()
        {
            DependencyObject element = this;
            while (!(element is StackPanel) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }
            if (element != null)
            {
                return element as StackPanel;
            }
            return null;
        }



        internal double PanelWidth
        {
            get { return (double)GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PanelWidthProperty =
            DependencyProperty.Register("PanelWidth", typeof(double), typeof(ContextMenuItemAdv), new PropertyMetadata(OnPanelWidthChanged));

        internal static void OnPanelWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ContextMenuItemAdv m = (ContextMenuItemAdv)obj;
            if (m != null)
                m.OnPanelWidthChanged(args);
        }
        //To remove warnings
        //private bool _DetectBoundary = true;
        /// <summary>
        /// Called when PanelWidth property changed
        /// </summary>
        /// <param name="args"></param>
        protected void OnPanelWidthChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
            {
                this.CheckForBoundaryCollision();
            }
        }

        

        

        internal void CheckForBoundaryCollision()
        {
            if (this.PART_Popup != null && this.PopupBorder != null && this.PopupGrid != null)
            {
                GeneralTransform gt = this.PART_Popup.TransformToVisual(Application.Current.RootVisual as UIElement);
                Point offset = gt.Transform(new Point(0, 0));

                this.PART_Popup.VerticalOffset = 0;
                this.PART_Popup.HorizontalOffset = 0;

                double FromTop = offset.Y + this.PART_Popup.VerticalOffset;
                double FromLeft = offset.X + this.PART_Popup.HorizontalOffset;

                this.PopupBorder.Projection = new System.Windows.Media.PlaneProjection();

                this.ParentMenuBase.PanelHeight = this.PopupGrid.ActualHeight;

                int itemCount = 0;

                foreach (var item in this.Items)
                {
                    if (!(item is SeparatorAdv))
                    {
                        itemCount++;
                    }
                }

                    this.PopupGrid.Height = (itemCount * this.ActualHeight);
                    this.PanelHeight = this.PopupGrid.Height;

                double browserWidth = (Application.Current.Host.Content.ActualWidth / (Application.Current.Host.Content.ZoomFactor * 100)) * 100;
                double browserHeight = (Application.Current.Host.Content.ActualHeight / (Application.Current.Host.Content.ZoomFactor * 100)) * 100;

                if ((FromTop + (itemCount * this.ActualHeight)) > this.PanelHeight && this.PanelHeight > browserHeight && this.PanelHeight > 0)
                {
                    this.PopupGrid.Height = (itemCount * this.ActualHeight);
                    this.PART_Popup.VerticalOffset = (this.PanelHeight) - ((FromTop + (itemCount * this.ActualHeight)));
                    this.PART_Popup.HorizontalOffset = 0;
                }
                else
                {
                    if (this.PART_Popup != null)
                    {
                        this.PART_Popup.VerticalOffset = 0;
                        this.PART_Popup.HorizontalOffset = 0;
                        if ((FromTop + (this.Items.Count * this.ActualHeight)) > Application.Current.Host.Content.ActualHeight)
                        {
                            this.PART_Popup.VerticalOffset = this.PART_Popup.VerticalOffset - ((FromTop + (this.Items.Count * this.ActualHeight)) - Application.Current.Host.Content.ActualHeight);
                        }

                        if (FromLeft + panel.ActualWidth > Application.Current.Host.Content.ActualWidth)
                        {
                            this.PART_Popup.HorizontalOffset = this.PART_Popup.HorizontalOffset - panel.ActualWidth - this.PanelWidth;
                        }
                    }
                }

                if (FromLeft + panel.ActualWidth > browserWidth)
                {
                    Canvas.SetLeft(this, FromLeft - ParentMenuBase.ActualWidth);
                }
                if (FromTop + panel.ActualHeight > browserHeight)
                {
                    Canvas.SetTop(this, FromTop - ParentMenuBase.ActualHeight);
                }
                if (this.PopupGrid.Height > browserHeight)
                {
                    this.PopupGrid.Height = browserHeight - 25;
                    this.PART_Popup.VerticalOffset = -offset.Y;
                    this.ParentMenuBase.PanelHeight = this.PopupGrid.Height;
                    this.ScrollabilityEnabled = true;
                }
            }

          
        }
    }
}
