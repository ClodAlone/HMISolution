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
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
#if SILVERLIGHT
using Syncfusion.Windows.Controls.Theming;
#endif

#if WPF
using Syncfusion.Licensing;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
# if SILVERLIGHT
    /// <summary>
    /// Represents the Split ButtonAdv Control
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Blend;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Default;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(DropDown), XamlResource = "/Syncfusion.Theming.Office2010Black;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/SplitButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/SplitButton.xaml")]
[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Metro;component/SplitButton.xaml")]
[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
   Type = typeof(SplitButtonAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/SplitButton.xaml")]
#endif

#if WPF
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/SplitButton.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
   Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/SplitButton/Themes/TransparentStyle.xaml")]
#endif

    public class SplitButtonAdv : DropDownButtonAdv, ICommandSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitButtonAdv"/> class.
        /// </summary>
        public SplitButtonAdv()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(SplitButtonAdv));
            }

            CommandManager.RequerySuggested += CanExecuteChanged;
            this.Unloaded += new RoutedEventHandler(SplitButtonAdv_Unloaded);
#endif
            DefaultStyleKey = typeof(SplitButtonAdv);
        }

        #if WPF
        void SplitButtonAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            CommandManager.RequerySuggested -= CanExecuteChanged;
        }
#endif

#if WPF
        static SplitButtonAdv()
        {
            //EnvironmentTest.ValidateLicense(typeof(SplitButtonAdv));
        }
#endif
        void _dropdownbutton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            IsDropDownOpen = true;
            VisualStateManager.GoToState(this, "DropDownPressed", true);
                        

#else
            IsDropDownPressed = true;
            if (IsDropDownOpen)
                isopened = true;
            else
                isopened = false;
#endif
            OnIsDropDownOpenChanged();
            //base.OnMouseLeftButtonDown(e);
        }

        void _button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
         
#if SILVERLIGHT
            VisualStateManager.GoToState(this, "ButtonPressed", true);
#else
            IsPressed = true;
#endif
        }

#if SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="SplitButtonAdv"/> class.
        /// </summary>
        static SplitButtonAdv()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
#endif

        private Border _button;

        private Border _dropdownbutton;

        private Border _buttonNormal;

        private Border _dropdownbuttonNormal;

         private new Popup _dropdown;

        // Summary:
        //     Occurs when a Syncfusion.Windows.Tools.Controls.RibbonSplitButton is clicked.
        /// <summary>
        /// Occurs when [click].
        /// </summary>
        public event RoutedEventHandler Click;
      
        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnClick()
        {
            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }

            if (Command != null)
            {
                if (Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        [Description("Gets or sets the command when the Button is pressed")]
        [Category("Common Properties")]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButtonAdv), new PropertyMetadata(null, new PropertyChangedCallback(CommandChanged)));

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        [Description("Gets or sets the parameter to pass the Command property")]
        [Category("Common Properties")]
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButtonAdv), new PropertyMetadata(null));

#if WPF 

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SplitButtonAdv), new UIPropertyMetadata(null));



        public bool IsDropDownPressed
        {
            get { return (bool)GetValue(IsDropDownPressedProperty); }
            protected internal set { SetValue(IsDropDownPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDownPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownPressedProperty =
            DependencyProperty.Register("IsDropDownPressed", typeof(bool), typeof(SplitButtonAdv), new UIPropertyMetadata(false));        

#endif


        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
#if WPF
            if ((e.Source as DropDownMenuItem) != null)
            {
#endif
                if (_dropdown != null)
                {
                    if (_dropdown.IsOpen)
                        _dropdown.IsOpen = false;
                }
#if WPF
            }
#endif
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //Do Nothing
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //Do Nothing
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="s"></param>
#if WPF 
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //Do Nothing
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            //Do Nothing
        }
#endif

#if SILVERLIGHT
        public new Color GetColorFromHexString(string s)
        {

            byte a = System.Convert.ToByte("FF", 16);//Alpha should be 255
            byte r = System.Convert.ToByte(s.Substring(0, 2), 16);
            byte g = System.Convert.ToByte(s.Substring(2, 2), 16);
            byte b = System.Convert.ToByte(s.Substring(4, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingfrom"></param>
        /// <returns></returns>
        public new string FindAncestorVisualStyle(DependencyObject startingfrom)
        {
            var item = VisualTreeHelper.GetParent(startingfrom);

            if (item != null && item is DependencyObject)
            {
                while (item != null && !(item.GetType().ToString() == "Syncfusion.Windows.Tools.Controls.Ribbon"))
                {
                    if (item is DependencyObject)
                    {
                        item = VisualTreeHelper.GetParent(item);
                    }
                    else
                    {
                        break;
                    }
                }
                if (item == null)
                    return null;
            }

            return SkinManager.GetVisualStyle(item).ToString();

        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //Do Nothing
            TextBlock txt;
            StackPanel st;
            if (FindAncestorVisualStyle(this) == "Blend")
            {
                ItemsControl itct = GetTemplateChild("PART_TextAreaLarge") as ItemsControl;
                if (itct != null)
                {
                    foreach (object obj in itct.Items)
                    {
                        if (obj is TextBlock)
                        {
                            txt = (TextBlock)obj;
                            Color c = GetColorFromHexString("333333");
                            txt.Foreground = new SolidColorBrush(c);
                        }
                        else if (obj is StackPanel)
                        {
                            st = (StackPanel)obj;

                            foreach (object obj1 in st.GetVisualChildren())
                            {
                                if (obj1 is TextBlock)
                                {
                                    txt = (TextBlock)obj1;
                                    Color c = GetColorFromHexString("333333");
                                    txt.Foreground = new SolidColorBrush(c);
                                }
                            }
                        }

                    }
                }
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            //Do Nothing
            TextBlock txt;
            StackPanel st;
            if (FindAncestorVisualStyle(this) == "Blend")
            {
                ItemsControl itct = GetTemplateChild("PART_TextAreaLarge") as ItemsControl;
                if(itct!=null)
                {
                    foreach (object obj in itct.Items)
                    {
                        if (obj is TextBlock)
                        {
                            txt = (TextBlock)obj;
                            txt.Foreground = this.Foreground;
                        }
                        else if (obj is StackPanel)
                        {
                            st = (StackPanel)obj;

                            foreach (object obj1 in st.GetVisualChildren())
                            {
                                if (obj1 is TextBlock)
                                {
                                    txt = (TextBlock)obj1;
                                    txt.Foreground = this.Foreground;
                                }
                            }
                        }
                    }
                }
            }
            base.OnMouseLeave(e);
        }
#endif
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _button = GetTemplateChild("PART_Button") as Border;
            _dropdownbutton = GetTemplateChild("PART_DropDownButton") as Border;
            _buttonNormal = GetTemplateChild("PART_ButtonNormal") as Border;
            _dropdownbuttonNormal = GetTemplateChild("PART_DropDownButtonNormal") as Border;
            _dropdown = GetTemplateChild("PART_DropDown") as Popup;

            if (_dropdown != null)
            {
                _dropdown.Closed += new EventHandler(_dropdown_Closed);
            }

            if (_button != null)
            {
                _button.MouseLeftButtonDown += new MouseButtonEventHandler(_button_MouseLeftButtonDown);
                _button.MouseLeftButtonUp += new MouseButtonEventHandler(_button_MouseLeftButtonUp);
                _button.MouseEnter += new MouseEventHandler(_button_MouseEnter);
            }

            if (_dropdownbutton != null)
            {
                _dropdownbutton.MouseLeftButtonDown += new MouseButtonEventHandler(_dropdownbutton_MouseLeftButtonDown);
                _dropdownbutton.MouseLeftButtonUp += new MouseButtonEventHandler(_dropdownbutton_MouseLeftButtonUp);
                _dropdownbutton.MouseEnter += new MouseEventHandler(_dropdownbutton_MouseEnter);
            }

            if (_buttonNormal != null)
            {
                _buttonNormal.MouseLeftButtonDown += new MouseButtonEventHandler(_button_MouseLeftButtonDown);
                _buttonNormal.MouseLeftButtonUp += new MouseButtonEventHandler(_button_MouseLeftButtonUp);
                _buttonNormal.MouseEnter += new MouseEventHandler(_button_MouseEnter);
            }

            if (_dropdownbuttonNormal != null)
            {
                _dropdownbuttonNormal.MouseLeftButtonDown += new MouseButtonEventHandler(_dropdownbutton_MouseLeftButtonDown);
                _dropdownbuttonNormal.MouseLeftButtonUp += new MouseButtonEventHandler(_dropdownbutton_MouseLeftButtonUp);
                _dropdownbuttonNormal.MouseEnter += new MouseEventHandler(_dropdownbutton_MouseEnter);
            }
            base.OnApplyTemplate();
        }

 void _dropdown_Closed(object sender, EventArgs e)
        {
#if WPF
            IsDropDownPressed = false;
#endif
        }
        void _dropdownbutton_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "DropDownMouseOver", true);
        }

        void _button_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", true);
        }

        void _dropdownbutton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

#if SILVERLIGHT
            if (_dropdown != null)
            {
                if (_dropdown.IsOpen)
                    VisualStateManager.GoToState(this, "DropDownPressed", true);
                else
                    VisualStateManager.GoToState(this, "Normal", true);
            }
#else
            if (IsDropDownOpen)
            {
                IsPressed = false;
                IsDropDownOpen = false;
            }
            else
            {
                IsPressed = true;
                IsDropDownOpen = true;
            }
            if (_dropdown.IsOpen)
                IsDropDownPressed = true;
            else
                IsDropDownPressed = false;
#endif
        
        }

        void _button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnClick();
#if SILVERLIGHT
            VisualStateManager.GoToState(this, "Normal", true);
#else
            IsPressed = false;
            IsDropDownPressed = false;
#endif
        }


        #region Property Changed

        EventHandler canExecuteChangedEventHandler;

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButtonAdv cs = (SplitButtonAdv)d;
            cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }

        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = new EventHandler(CanExecuteChanged);
            canExecuteChangedEventHandler = handler;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedEventHandler;
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {

            if (this.Command != null)
            {
#if WPF

                var command = this.Command as RoutedCommand;

                // If a RoutedCommand. 
                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
                // If a not RoutedCommand. 
                else
                {
#endif
                    if (Command.CanExecute(CommandParameter))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
#if WPF

                }
#endif
            }
        }

        #endregion


    }
}
