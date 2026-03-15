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
namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents a selectable item inside a Menu or ContextMenuControl.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContextMenuControlItem))]

    public class ContextMenuControlItem : HeaderedItemsControl // , ICommandSource // ICommandSource not defined by Silverlight 4
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
            DependencyProperty.Register("SubMenuBorderThickness", typeof(Thickness), typeof(ContextMenuControlItem), new PropertyMetadata(new Thickness(1)));

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
            DependencyProperty.Register("SubMenuBackground", typeof(Brush), typeof(ContextMenuControlItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

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
            DependencyProperty.Register("SubMenuBorderBrush", typeof(Brush), typeof(ContextMenuControlItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


      


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
            DependencyProperty.Register("ExtendButtonVisibility", typeof(Visibility), typeof(ContextMenuControlItem), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the parent menu item.
        /// </summary>
        /// <value>The parent menu item.</value>
        public ContextMenuControlItem ParentMenuItem
        {
            get { return (ContextMenuControlItem)GetValue(ParentMenuItemProperty); }
            set { SetValue(ParentMenuItemProperty, value); }
        }

        /// <summary>
        /// Identifies the ParentMenuItemProperty.
        /// </summary>
        public static readonly DependencyProperty ParentMenuItemProperty =
            DependencyProperty.Register("ParentMenuItem", typeof(ContextMenuControlItem), typeof(ContextMenuControlItem), new PropertyMetadata(null));

        ///// <summary>
        ///// Gets or sets the theme.
        ///// </summary>
        ///// <value>The theme.</value>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[BrowsableAttribute(false)]
        //public String Theme
        //{
        //    get { return (string)GetValue(ThemeProperty); }
        //    set { SetValue(ThemeProperty, value); }
        //}

        ///// <summary>
        ///// Identifies the ThemeProperty.
        ///// </summary>
        //internal static readonly DependencyProperty ThemeProperty =
        //    DependencyProperty.Register("Theme", typeof(string), typeof(ContextMenuControlItem), new PropertyMetadata(string.Empty));

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
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ContextMenuControlItem), new PropertyMetadata(false));

        /// <summary>
        /// Called when [is open changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (((ContextMenuControlItem)obj).ParentMenuItem != null)
            {
                ((ContextMenuControlItem)obj).ParentMenuItem.IsOpen = !((ContextMenuControlItem)obj).ParentMenuItem.IsOpen;
            }
        }

        /// <summary>
        /// Occurs when a ContextMenuControlItem is clicked.
        /// </summary>
        public event RoutedEventHandler Click;
        /*
        /// <summary>
        /// Occurs when [checked].
        /// </summary>
        //public event RoutedEventHandler Checked;
        /// <summary>
        /// Occurs when [un checked].
        /// </summary>
        //public event RoutedEventHandler UnChecked;
         */

        /// <summary>
        /// Stores a value indicating whether this element has logical focus.
        /// </summary>
        private bool _isFocused;

        /// <summary>
        /// Gets or sets a reference to the MenuBase parent.
        /// </summary>
        //internal MenuBase ParentMenuBase { get; set; }
        internal ContextMenuControl ParentMenuBase { get; set; }

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
            typeof(ContextMenuControlItem),
            new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Handles changes to the Command DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ContextMenuControlItem)o).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
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
        /// Gets or sets the parameter to pass to the Command property of a ContextMenuControlItem.
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
            typeof(ContextMenuControlItem),
            new PropertyMetadata(null, OnCommandParameterChanged));

        /// <summary>
        /// Handles changes to the CommandParameter DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnCommandParameterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ContextMenuControlItem)o).OnCommandParameterChanged(e.OldValue, e.NewValue);
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
        /// Gets or sets the icon that appears in a ContextMenuControlItem.
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
            typeof(ContextMenuControlItem),
            new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the ContextMenuControlItem class.
        /// </summary>
        public ContextMenuControlItem()
        {
            DefaultStyleKey = typeof(ContextMenuControlItem);
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
        /// Called whenever the mouse enters a ContextMenuControlItem.
        /// </summary>
        /// <param name="e">The event data for the MouseEnter event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Focus();
            ChangeVisualState(true);

            if (!this.ParentMenuBase.OpenOnClick)
            {
                this.HandleOpenClsoe();
            }
        }

        /// <summary>
        /// Called whenever the mouse leaves a ContextMenuControlItem.
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
        /// <param name="e">The event data for the MouseLeftButtonDown event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                OnClick();
                e.Handled = true;
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Called when the right mouse button is pressed.
        /// </summary>
        /// <param name="e">The event data for the MouseRightButtonDown event.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                OnClick();
                e.Handled = true;
            }
            base.OnMouseRightButtonDown(e);
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
        /// Called when a ContextMenuControlItem is clicked and raises a Click event.
        /// </summary>
        protected virtual void OnClick()
        {
           

            ContextMenuControl contextMenu = ParentMenuBase as ContextMenuControl;
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
            // WPF-compatibility by allowing the ContextMenuControl to close before the command
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
                //ChangeTonormalState();
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", useTransitions);
                //ChangeTonormalState();
            }

            if (_isFocused && IsEnabled)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
                //ChangeToMouseOverState();
            }
            else
            {
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
                //ChangeTonormalState();
            }

            if (IsOpen == true && _isFocused == false)
            {
                VisualStateManager.GoToState(this, "Focused", useTransitions);
                //ChangeToMouseOverState();
            }
            //else
            //{
            //    VisualStateManager.GoToState(this, "Unfocused", useTransitions);
            //}
        }



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

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ContextMenuControlItem menuItem = element as ContextMenuControlItem;
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

                //CheckGroupName(menuItem);
            }
        }

       

        private Binding GetBinding(HierarchicalDataTemplate template, ContextMenuControlItem item)
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

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is ContextMenuControlItem));
            //return item is ContextMenuControlItem;
            //return base.IsItemItsOwnContainerOverride(item);
        }

        internal ContentPresenter Presenter;
        internal Popup PART_Popup;
        private Border CheckBoxBorder;
        private System.Windows.Shapes.Path CheckBoxTick;
        private Border RadioBtnBorder;
        private Ellipse RadioBtn;
        private System.Windows.Shapes.Path ExtendBtnPath;
        private ContentControl PART_Content;
        StackPanel panel;
        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            panel = this.GetStackPanel();
            if (panel != null)
            {
                //panel.Loaded += new RoutedEventHandler(panel_Loaded);
                panel.LayoutUpdated += new EventHandler(panel_LayoutUpdated);
            }

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
            if (PART_Popup != null)
            {

            }

            this.CheckBoxBorder = GetTemplateChild("GlyphPanel") as Border;
            this.CheckBoxTick = GetTemplateChild("Glyph") as System.Windows.Shapes.Path;
            this.RadioBtnBorder = GetTemplateChild("GlyphPanel1") as Border;
            this.RadioBtn = GetTemplateChild("Background") as Ellipse;
            this.ExtendBtnPath = GetTemplateChild("extendPath") as System.Windows.Shapes.Path;

            if (this.ParentMenuBase != null && !this.ParentMenuBase.OverrideVisualStyle)
            {
                if (this.ParentMenuItem != null)
                    this.Style = this.ParentMenuItem.Style;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.ItemsSource != null)
                    {
                        ContextMenuControlItem m_treeViewItem = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as ContextMenuControlItem;
                        if (m_treeViewItem != null && m_treeViewItem is ContextMenuControlItem)
                        {
                            m_treeViewItem.Style = this.Style;
                        }
                    }
                    else
                    {
                        if (this.Items[i] is ContextMenuControlItem)
                            (this.Items[i] as ContextMenuControlItem).Style = this.Style;
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
                    }
                    else
                    {
                    }
                    //this.ParentMenuItem.SetPopupPosition();
                }
                panel.LayoutUpdated -= new EventHandler(panel_LayoutUpdated);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContextMenuControlItem();
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
                if (this.Items[i] is ContextMenuControlItem)
                {
                    if ((this.Items[i] as ContextMenuControlItem) != null)
                    {
                        ((ContextMenuControlItem)this.Items[i]).ClosePopup();
                    }
                }
                else
                {
                    ContextMenuControlItem m_menuitem = this.ItemContainerGenerator.ContainerFromIndex(i) as ContextMenuControlItem;
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
                    ContextMenuControlItem _menuitem = this;
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
            DependencyProperty.Register("PanelWidth", typeof(double), typeof(ContextMenuControlItem), new PropertyMetadata(OnPanelWidthChanged));

        internal static void OnPanelWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ContextMenuControlItem m = (ContextMenuControlItem)obj;
            if (m != null)
                m.OnPanelWidthChanged(args);
        }

        //private bool _DetectBoundary = true;
        protected void OnPanelWidthChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
            {
                this.CheckForBoundaryCollision();
            }
        }





        internal void CheckForBoundaryCollision()
        {
            if (this.PART_Popup != null)
            {
                //StackPanel panel = this.GetStackPanel();

                GeneralTransform gt = this.PART_Popup.TransformToVisual(Application.Current.RootVisual as UIElement);
                Point offset = gt.Transform(new Point(0, 0));

                this.PART_Popup.VerticalOffset = 0;
                this.PART_Popup.HorizontalOffset = 0;

                double FromTop = offset.Y + this.PART_Popup.VerticalOffset;
                double FromLeft = offset.X + this.PART_Popup.HorizontalOffset;

               

                if ((FromTop + (this.Items.Count * this.ActualHeight)) > Application.Current.Host.Content.ActualHeight)
                //if ((FromTop + height) > Application.Current.Host.Content.ActualHeight)
                {
                    this.PART_Popup.VerticalOffset = this.PART_Popup.VerticalOffset - ((FromTop + (this.Items.Count * this.ActualHeight)) - Application.Current.Host.Content.ActualHeight);//this.PART_Popup.VerticalOffset - panel.ActualHeight;
                    //this.PART_Popup.VerticalOffset = this.PART_Popup.VerticalOffset - ((FromTop + height) - Application.Current.Host.Content.ActualHeight);//this.PART_Popup.VerticalOffset - panel.ActualHeight;
                }

                if (FromLeft + panel.ActualWidth > Application.Current.Host.Content.ActualWidth)
                {
                    //if (FromLeft - panel.ActualWidth > 0)
                    //{
                    //this.PART_Popup.HorizontalOffset = this.PART_Popup.HorizontalOffset - panel.ActualWidth - panel.ActualWidth;
                    this.PART_Popup.HorizontalOffset = this.PART_Popup.HorizontalOffset - panel.ActualWidth - this.PanelWidth;
                    //}
                }

               
            }
           
        }
    }
}
