#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Diagnostics;
    using Syncfusion.Windows.GridCommon;
    using System.Diagnostics;

    /// <summary>
    /// Defines the base control for the Drop-Down list control that is used with DropDownList cells.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [TemplatePart(Name = GridCellDropDownControlBase.TemplatePopup, Type = typeof(GridCellDropDownControlBase))]
    [TemplatePart(Name = GridCellDropDownControlBase.TemplatePopupContent, Type = typeof(GridCellDropDownControlBase))]
    [TemplatePart(Name = GridCellDropDownControlBase.TemplateTextbox, Type = typeof(GridCellDropDownControlBase))]
    public class GridCellDropDownControlBase : Control
    {
        public static double MaxDropDownHeight = SystemParameters.PrimaryScreenHeight / 3.0;
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            "IsDropDownOpen",
            typeof(bool),
            typeof(GridCellDropDownControlBase),
            new FrameworkPropertyMetadata(
                false,
                new PropertyChangedCallback(OnIsDropDownOpenChanged),
                new CoerceValueCallback(CoerceIsDropDownOpen)));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(GridCellDropDownControlBase),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty StaysOpenOnEditProperty = DependencyProperty.Register(
            "StaysOpenOnEdit",
            typeof(bool),
            typeof(GridCellDropDownControlBase),
            new FrameworkPropertyMetadata(false));
        private IEnumerable autoSuggestionList;
        public const string TemplatePopup = "PART_Popup";
        public const string TemplatePopupContent = "PART_Content";
        public const string TemplateTextbox = "PART_Textbox";
        public const string TemplateToggle = "PART_Togg";
       
        /// <summary>
        /// Initializes a new <see cref="GridCellDropDownControlBase"/>.
        /// </summary>
        /// 

        public IEnumerable AutoSuggestionList
        {
            get
            {
                return autoSuggestionList;
            }
            set
            {
                autoSuggestionList = value;
            }
        }

        public GridCellDropDownControlBase()
        {
        }

        static GridCellDropDownControlBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridCellDropDownControlBase), new FrameworkPropertyMetadata(typeof(GridCellDropDownControlBase)));
            // Disable tooltips on popup when it is open
            ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(GridCellDropDownControlBase), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceToolTipIsEnabled)));
        }

        private GridDropDownStyle _dropDownStyle;
        /// <summary>
        /// Specifies if user input is restricted to items from the ChoiceList or ItemsSource.
        /// </summary>
        public GridDropDownStyle DropDownStyle
        {
            get { return _dropDownStyle; }
            set { _dropDownStyle = value; }
        }

        private bool HasCapture
        {
            get
            {
                return Mouse.Captured == this;
            }
        }

        /// <summary>
        /// Specifies the edit part of the drop-down control.
        /// </summary>
        public TextBox TextBoxPart
        {
            get;
            private set;
        }

        public event EventHandler IsDropDownOpenChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the drop-down is opened.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the drop-down is opened; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)this.GetValue(GridCellDropDownControlBase.IsDropDownOpenProperty);
            }

            set
            {
                this.SetValue(GridCellDropDownControlBase.IsDropDownOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over inner text box.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse over inner text box; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOverInnerTextBox
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over popup host.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse over popup host; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOverPopupHost
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the drop down control's value is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the drop down control's value is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return (bool)this.GetValue(GridCellDropDownControlBase.IsReadOnlyProperty);
            }

            set
            {
                this.SetValue(GridCellDropDownControlBase.IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets the content of the drop down part.
        /// </summary>
        public ContentControl PopupContent
        {
            get;
            private set;
        }

        internal Popup PopupHost
        {
            get;
            set;
        }

        internal ToggleButton ToggleButton
        {
            get;
            set;
        }

        ///<summary>
        ///Gets or sets a value indicating whether the drop-down host stays open during edit.
        /// </summary>
        /// <value>
        /// <b>True</b> if ; otherwise, <b>false</b>.
        /// </value>
        public bool StaysOpenOnEdit
        {
            get
            {
                return (bool)this.GetValue(GridCellDropDownControlBase.StaysOpenOnEditProperty);
            }

            set
            {
                this.SetValue(GridCellDropDownControlBase.StaysOpenOnEditProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(GridCellDropDownControlBase));

        /// <summary>
        /// Gets or sets the text of the drop-down control.
        /// </summary>
        public string Text
        {
            get
            {
                return (string)this.GetValue(GridCellDropDownControlBase.TextProperty);
            }

            set
            {
                this.SetValue(GridCellDropDownControlBase.TextProperty, value);
            }
        }

        //public string Text
        //{
        //    get
        //    {
        //        if (this.TextBoxPart != null)
        //        {
        //            return this.TextBoxPart.Text;
        //        }

        //        return (text != null) ? text : "";
        //    }

        //    set
        //    {
        //        if (this.TextBoxPart == null)
        //        {
        //            text = value;
        //        }
        //        else if (this.TextBoxPart.Text != value)
        //        {
        //            this.TextBoxPart.Text = value;
        //            this.TextBoxPart.SelectAll();
        //        }
        //    }
        //}

        /// <summary>
        /// Closes the drop down.
        /// </summary>
        public virtual void Close()
        {
            if (this.IsDropDownOpen)
            {               
               
                if (this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = false;
                }
                if (this.TextBoxPart != null && !this.IsReadOnly)
                {
                    this.TextBoxPart.Focus();
                }
            }
        }

        private void ClosePopup()
        {
            if (this.IsDropDownOpen)
            {
                this.ClearValue(GridCellDropDownControlBase.IsDropDownOpenProperty);
            }
        }

        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            GridCellDropDownControlBase gcd = (GridCellDropDownControlBase)d;
            var isDropDownOpen = (bool)value;
            if (isDropDownOpen)
            {
                if (!gcd.IsLoaded)
                {
                    gcd.RegisterToOpenOnLoad();
                    return false;
                }
            }

            var result = gcd.OnIsDropDownOpenChanging(isDropDownOpen);
            if (result)
            {
                return false; // return !isDropDownOpen;
            }
            return value;
        }

        private static object CoerceToolTipIsEnabled(DependencyObject d, object value)
        {
            GridCellDropDownControlBase gcd = (GridCellDropDownControlBase)d;
            return gcd.IsDropDownOpen ? false : value;
        }

        protected virtual void NavigateNextLine()
        {
        }

        protected virtual void NavigatePreviousLine()
        {
        }

        //protected override void OnLostMouseCapture(MouseEventArgs e)
        //{
        //    if (Mouse.Captured != this)
        //    {
        //        if (e.OriginalSource == this && Mouse.Captured == null)
        //        {
        //            this.Close();
        //        }
        //    }
        //    else
        //    {
        //        this.Close();
        //    }
        //    base.OnLostMouseCapture(e);
        //}

        /// <summary>
        /// It is invoked when the application code or internal processes call <see cref="FrameworkElement.ApplyTemplate()"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.TextBoxPart != null)
            {
                this.TextBoxPart.LostFocus -= new RoutedEventHandler(textBoxPartLostFocus);
                this.TextBoxPart.GotFocus -= new RoutedEventHandler(textBoxPartGotFocus);
            }

            this.TextBoxPart = null;

            if (this.PopupContent != null && this.PopupContent.Content != null)
            {
                var uiElement = this.PopupContent.Content as UIElement;
                uiElement.LostFocus -= new RoutedEventHandler(popupContentAsUIElement_LostFocus);
            }

            this.PopupContent = null;

            base.OnApplyTemplate();
          
            this.ToggleButton = null;           
            this.ToggleButton = this.GetTemplateChild(GridCellDropDownControlBase.TemplateToggle) as ToggleButton;            

            this.TextBoxPart = this.GetTemplateChild(GridCellDropDownControlBase.TemplateTextbox) as TextBox;
            if (this.TextBoxPart != null)
            {
                //if (text != null)
                //    this.TextBoxPart.Text = text;
                //text = null;
                this.TextBoxPart.LostFocus += new RoutedEventHandler(textBoxPartLostFocus);
                this.TextBoxPart.GotFocus += new RoutedEventHandler(textBoxPartGotFocus);
                if (this.IsReadOnly)
                {
                    TextBoxPart.IsReadOnly = true;
#if !SyncfusionFramework3_5
                    TextBoxPart.IsReadOnlyCaretVisible = false;
#endif
                    TextBoxPart.IsHitTestVisible = false;
                }
            }

            this.PopupHost = this.GetTemplateChild(GridCellDropDownControlBase.TemplatePopup) as Popup;
            if (this.PopupHost != null)
            {

            }
            this.Loaded += (sender, args) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate(object param)
                {
                    CoerceValue(IsDropDownOpenProperty);
                    return null;
                }), null);
            };
            this.PopupContent = this.GetTemplateChild(GridCellDropDownControlBase.TemplatePopupContent) as ContentControl;
            if (this.PopupContent != null)
            {
                this.OnContentLoaded(this.PopupContent);
            }

            if (AppliedTemplate != null)
                AppliedTemplate(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs once the OnApplyTemplate() completes.
        /// </summary>
        public event EventHandler AppliedTemplate;

        protected virtual void OnContentLoaded(ContentControl popupContent)
        {
            if (popupContent.Content != null)
            {
                UIElement uiElement = popupContent.Content as UIElement;
                if (uiElement != null)
                {
                    uiElement.LostFocus += new RoutedEventHandler(popupContentAsUIElement_LostFocus);
                }
            }
        }

        void textBoxPartGotFocus(object sender, RoutedEventArgs e)
        {
            //This code has been comented on the behavior mentioned in the following issue
            //SD9600 - Pressing delete key opens the Combo Box drop down list
            //if (this.IsReadOnly)
            //{
            //    this.ShowPopup();
            //}
            var textBox = sender as TextBox;
            textBox.Select(0, textBox.Text.Length);
            e.Handled = true;
        }

        private void textBoxPartLostFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsDropDownOpen && !this.IsMouseOverPopupHost)
            {
                this.Close();
            }
        }

        void popupContentAsUIElement_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.IsDropDownOpen && (!this.IsMouseOverInnerTextBox || !this.IsMouseOverPopupHost || !this.StaysOpenOnEdit))
            {
                this.Close();
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (!e.Handled && this.TextBoxPart != null)
            {
                this.TextBoxPart.Select(0, this.TextBoxPart.Text.Length > 0 ? this.TextBoxPart.Text.Length - 1 : 0);
                e.Handled = true;
            }
            base.OnGotFocus(e);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridCellDropDownControlBase gdc = d as GridCellDropDownControlBase;
            bool newValue = (bool)args.NewValue;
            // bool oldValue = !newValue; Unused local variable
            if (!newValue)
            {
                if (gdc.HasCapture)
                {
                    Mouse.Capture(null);
                }
            }
            else
            {
                if (gdc != null)
                    gdc.OnDropDownOpened();
            }

            gdc.OnIsDropDownOpenChanged();
            gdc.CoerceValue(ToolTipService.IsEnabledProperty);

            GridControlBase grid = GridControlBase.GetCellsControl(gdc.PopupHost) as GridControlBase;
            if (grid != null)
                grid.CurrentCell.IsDroppedDown = newValue;
        }

        internal bool IsMouseTrackingEnabled
        {
            get;
            set;
        }
        bool enableTrack = false;
        
        protected virtual void OnDropDownOpened()
        {
            enableTrack = !this.IsMouseTrackingEnabled;
        }

        protected virtual void OnIsDropDownOpenChanged()
        {
            var handler = this.IsDropDownOpenChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event GridCellShowingDropDownEventHandler IsDropDownChanging;

        protected virtual bool OnIsDropDownOpenChanging(bool isShowing)
        {
            var handler = this.IsDropDownChanging;
            if (handler != null)
            {
                var e = new GridCellShowingDropDownEventArgs(isShowing);
                handler(this, e);
                return e.Cancel;
            }

            return false;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.PopupHost != null && !this.PopupHost.IsMouseCaptured && !this.IsMouseOver)
            {
                this.IsDropDownOpen = false;
            }

            base.OnLostFocus(e);
        }


        //This codes are commented because if Grid is placed in side the Docking Manager ComboBox closed.Refer SD11949
        //protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        //{
        //    DependencyObject popupContent = null;
        //    if (PopupContent != null)
        //        popupContent = PopupContent.Content as DependencyObject;
        //    DependencyObject newFocus = e.NewFocus as DependencyObject;

        //    if (newFocus != null && GridUtil.IsObjectDescendantOfParent(popupContent, newFocus))
        //        return;

        //    if ((!this.IsMouseOverPopupHost && !this.IsMouseOverInnerTextBox && this.IsMouseOver) || !this.StaysOpenOnEdit)
        //    {
        //        this.Close();
        //    }

        //    base.OnLostKeyboardFocus(e);
        //}

        

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                this.IsMouseOverInnerTextBox = false;
                this.IsMouseOverPopupHost = false;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if ((this.IsDropDownOpen && !this.IsMouseOverPopupHost && !this.IsMouseOverInnerTextBox) || !this.StaysOpenOnEdit)
            {
                this.Close();
            }

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                var flag = this.PopupHost != null ? this.PopupHost.IsMouseOver : false;
                this.IsMouseOverPopupHost = flag;

                flag = this.TextBoxPart != null ? this.TextBoxPart.IsMouseOver : false;
                this.IsMouseOverInnerTextBox = flag;
                //#if DEBUG
                //                Debug.WriteLine("IsMouseOverPopupHost -> " + this.IsMouseOverPopupHost);
                //                Debug.WriteLine("IsMouseOverInnerTextBox -> " + this.IsMouseOverInnerTextBox);
                //#endif
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (this.IsKeyboardFocusWithin)
            {
                if (!this.IsDropDownOpen)
                {
                    if (e.Delta < 0)
                    {
                        // moving down
                        this.NavigateNextLine();
                    }
                    else
                    {
                        // moving up
                        this.NavigatePreviousLine();
                    }
                }
            }
            else if (this.IsDropDownOpen)
            {
                e.Handled = true;
            }
            base.OnMouseWheel(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (this.IsReadOnly)
            {
                Visual originalSource = e.OriginalSource as Visual;
                Visual editableTextBox = this.TextBoxPart;
                if (((originalSource != null) && (editableTextBox != null)) && editableTextBox.IsAncestorOf(originalSource))
                {
                    if (this.IsDropDownOpen && !this.StaysOpenOnEdit)
                    {
                        this.Close();
                    }
                    else if (!this.IsKeyboardFocusWithin)
                    {
                        this.Focus();
                        e.Handled = true;
                    }
                }
            }
            base.OnPreviewMouseDown(e);
        }

        private void RegisterToOpenOnLoad()
        {
            try
            {                
                //this.Loaded += (sender, args) =>
                //{
                //        Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate(object param)
                //        {
                //            CoerceValue(IsDropDownOpenProperty);
                //            return null;
                //        }), null);
                   
                //    // Open popup after it has rendered (Loaded is fired before 1st render) 
                    
                //};
            }
            catch
            {

            }
            
        }

        void GridCellDropDownControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        protected void ShowPopup()
        {
            if (!this.IsDropDownOpen)
            {
                this.IsDropDownOpen = true;
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (Mouse.Captured != this)
            {
                if (e.OriginalSource == this && Mouse.Captured == null)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
            base.OnLostMouseCapture(e);
        }
    }

    public delegate void GridCellShowingDropDownEventHandler(object sender, GridCellShowingDropDownEventArgs e);

    public class GridCellShowingDropDownEventArgs : CancelEventArgs
    {
        public GridCellShowingDropDownEventArgs(bool isDropDownOpen)
        {
            this.isDropDownOpen = isDropDownOpen;
        }

        private bool isDropDownOpen = false;
        public bool IsDropDownOpen
        {
            get
            {
                return this.isDropDownOpen;
            }
        }
    }
}
