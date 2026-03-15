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
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
#if SILVERLIGHT
using Syncfusion.Windows.Controls.Theming;
#endif

#if WPF
using Syncfusion.Licensing;
#endif

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Represents the DropDownButtonAdv Control.
    /// </summary>
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Disabled")]


# if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
     Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Blend;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Default;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/DropDownButton.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/DropDownButton.xaml")]
     [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
        Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Metro;component/DropDownButton.xaml")]
     [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
         Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/DropDownButton.xaml")]
     [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
          Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/DropDownButton.xaml")]
#endif

#if WPF
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/DropDownButton.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
   Type = typeof(DropDownButtonAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/TransparentStyle.xaml")]
#endif

    public class DropDownButtonAdv : ContentControl, IButtonAdv
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownButtonAdv"/> class.
        /// </summary>
        public DropDownButtonAdv()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(DropDownButtonAdv));
            }
#endif
            DefaultStyleKey = typeof(DropDownButtonAdv);
            Initialize();

        }

        
#if SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="DropDownButtonAdv"/> class.
        /// </summary>
        static DropDownButtonAdv()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
        
#endif
        #endregion

        #region Private variables
        private bool _ismouseover = false;

        private Border _normal;

        private Border _large;

        private Border SmallBorder;

        private Border LargeBorder;

        private TextBlock _textNormal;

        internal  ItemsControl _multilineTextLarge;

#if SILVERLIGHT
        internal DropDown _dropdown;
#else
        internal Popup _dropdown;
#endif
#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        public bool _istempsize = false;
#endif

        #endregion

        #region Dependency Properties
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        [Category("Common Properties")]
        [Description("The Label Property of this element can be set to any string value")]
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(DropDownButtonAdv), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the small icon.
        /// </summary>
        /// <value>The small icon.</value>
        [Category("Common Properties")]
        [Description("Represents the Image displayed in the element, when size form is Small or Normal")]
        public ImageSource SmallIcon
        {
            get { return (ImageSource)GetValue(SmallIconProperty); }
            set { SetValue(SmallIconProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty =
            DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(DropDownButtonAdv), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the large icon.
        /// </summary>
        /// <value>The large icon.</value>
        [Category("Common Properties")]
        [Description("Represents the Image displayed in the element, when size form is Large")]
        public ImageSource LargeIcon
        {
            get { return (ImageSource)GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LargeIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(DropDownButtonAdv), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the size mode.
        /// </summary>
        /// <value>The size mode.</value>
        [Category("Appearance")]
        [Description("Represents the Size of the element, which may be Normal, Small or Large")]
        public SizeMode SizeMode
        {
            get { return (SizeMode)GetValue(SizeModeProperty); }
            set { SetValue(SizeModeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeForm.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeModeProperty =
            DependencyProperty.Register("SizeMode", typeof(SizeMode), typeof(DropDownButtonAdv), new PropertyMetadata(SizeMode.Normal, new PropertyChangedCallback(OnSizeFormChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi line.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is multi line; otherwise, <c>false</c>.
        /// </value>
        [Category("Appearance")]
        [Description("Represents the value, whether the text in the element can be multilined or not")]
        public bool IsMultiLine
        {
            get { return (bool)GetValue(IsMultiLineProperty); }
            set { SetValue(IsMultiLineProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMultiLine.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMultiLineProperty =
            DependencyProperty.Register("IsMultiLine", typeof(bool), typeof(DropDownButtonAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnIsMultiLineChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        [Description("Represents the value, whether the drop down menu is open or not")]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownButtonAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <summary>
        /// Gets or sets the drop direction.
        /// </summary>
        /// <value>The drop direction.</value>
        [Category("Common Properties")]
        [Description("Represents the direction, in which the drop down of this element has to be displayed.")]
        public DropDirection DropDirection
        {
            get { return (DropDirection)GetValue(DropDirectionProperty); }
            set { SetValue(DropDirectionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDirection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDirectionProperty =
            DependencyProperty.Register("DropDirection", typeof(DropDirection), typeof(DropDownButtonAdv), new PropertyMetadata(DropDirection.BottomLeft));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            protected internal set { SetValue(IsPressedProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(DropDownButtonAdv), new PropertyMetadata(false));


        #endregion

        /// <summary>
        /// Occurs when [drop down opening].
        /// </summary>
        public event CancelEventHandler DropDownOpening;

        /// <summary>
        /// Occurs when [drop down opened].
        /// </summary>
        public event RoutedEventHandler DropDownOpened;

        /// <summary>
        /// Occurs when [drop down closing].
        /// </summary>
        public event CancelEventHandler DropDownClosing;

        /// <summary>
        /// Occurs when [drop down closed].
        /// </summary>
        public event RoutedEventHandler DropDownClosed;

        #region Implementation
        internal string VisualState
        {
            get
            {
                if (this.IsEnabled)
                {
                    if (this._ismouseover)
                    {
                        if (this.IsPressed)
                        {
                            return "Pressed";
                        }

                        return "MouseOver";
                    }

                    return "Normal";
                }

                return "Disabled";
            }
        }

        internal void UpdateVisualState()
        {
            VisualStateManager.GoToState(this, this.VisualState, true);
        }

    
        private void Initialize()
        {
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
        }

        private void InitSizeForm()
        {
            SmallBorder = GetTemplateChild("PART_ImageBorder") as Border;
            LargeBorder = GetTemplateChild("PART_ImageBorderLarge") as Border;
            _large = GetTemplateChild("ItemBorder1") as Border;
            _normal = GetTemplateChild("ItemBorder") as Border;
            _textNormal = GetTemplateChild("PART_Text") as TextBlock;
            UpdateSizeForm(SizeMode);
        }

        private void InitMultiLine()
        {
            _multilineTextLarge = GetTemplateChild("PART_TextAreaLarge") as ItemsControl;
            OnIsMultiLineChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeform"></param>
        public void UpdateSizeForm(SizeMode sizeform)
        {
            if (sizeform == Controls.SizeMode.Normal)
            {
                if (_normal != null)
                    _normal.Visibility = Visibility.Visible;
                if (_large != null)
                    _large.Visibility = Visibility.Collapsed;
                if (_textNormal != null)
                    _textNormal.Visibility = Visibility.Visible;
            }
            else if (sizeform == Controls.SizeMode.Large)
            {
                if (_normal != null)
                    _normal.Visibility = Visibility.Collapsed;
                if (_large != null)
                    _large.Visibility = Visibility.Visible;
            }
            else
            {
                if (_textNormal != null)
                    _textNormal.Visibility = Visibility.Collapsed;
                if (_large != null)
                    _large.Visibility = Visibility.Collapsed;
                if (_normal != null)
                    _normal.Visibility = Visibility.Visible;
            }
            if (this.SmallIcon == null)
            {
                if (SmallBorder != null)
                {
                    SmallBorder.Visibility = Visibility.Collapsed;
                }
            }
            if (this.LargeIcon == null)
            {
                if (this.LargeBorder != null)
                {
                    LargeBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                _ismouseover = false;
                IsPressed = false;
            }
            UpdateVisualState();
        }

       
        private static void OnSizeFormChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as DropDownButtonAdv;
            sender.OnSizeFormChanged();
        }

        private void OnSizeFormChanged()
        {
            UpdateSizeForm(SizeMode);
        }

        private static void OnDropDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as DropDownButtonAdv;
            sender.OnDropDirectionChanged();
        }

        private void OnDropDirectionChanged()
        {
            UpdateDropDirection();
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as DropDownButtonAdv;
            sender.OnIsDropDownOpenChanged();
        }

        internal void OnIsDropDownOpenChanged()
        {
            if (_dropdown != null)
            {
                UpdateDropDirection();
                _dropdown.IsOpen = IsDropDownOpen;
#if SILVERLIGHT
                if (IsDropDownOpen)
                {
                    if (((this.Content) as FrameworkElement) != null)
                    {
                        try
                        {
                            (((this.Content) as FrameworkElement)).UpdateLayout();

                            GeneralTransform objTransform = this.TransformToVisual(Application.Current.RootVisual as UIElement);
                            Point point = objTransform.Transform(new Point(0, 0));
                            double left = point.X;
                            double top = point.Y;
                            double totalheight = top + ((FrameworkElement)this.Content).RenderSize.Height;
                            double totalwidth = left + ((FrameworkElement)this.Content).RenderSize.Width;
                            if (totalheight > Application.Current.Host.Content.ActualHeight)
                            {
                                _dropdown.VerticalOffset = -(((FrameworkElement)this.Content).RenderSize.Height);
                            }

                            if (totalwidth > Application.Current.Host.Content.ActualWidth)
                            {
                                _dropdown.HorizontalOffset = -(totalwidth - Application.Current.Host.Content.ActualWidth) - 3;
                            }
                        }
                        catch(Exception e)
                        {
                            if (e != null)
                            {
                            }
                        }
                    }

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


                }
                else{
                     TextBlock txt;
                StackPanel st;
                _ismouseover = false;
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
                }
#endif
            }
        }

        private static void OnIsMultiLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as DropDownButtonAdv;
            sender.OnIsMultiLineChanged();
        }

        private void OnIsMultiLineChanged()
        {
            if (_multilineTextLarge != null)
            {
                Binding binding = new Binding("Label");
                binding.Source = this;
                MultiLineConverter conv = new MultiLineConverter();
                binding.Converter = conv;
                binding.ConverterParameter = this;
                _multilineTextLarge.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            }
        }

        

        #endregion

        #region Overrides

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="s"></param>
#if SILVERLIGHT
        public Color GetColorFromHexString(string s)
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
        public string FindAncestorVisualStyle(DependencyObject startingfrom)
        {
            var item = VisualTreeHelper.GetParent(startingfrom);

            if (item != null && item is DependencyObject)
            {
                while (item != null && !(item.GetType().ToString() == "Syncfusion.Windows.Tools.Controls.Ribbon"))
                {
                    if ((item as FrameworkElement).Parent is Popup)
                        item = (item as FrameworkElement).Parent;
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
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (!IsDropDownOpen)
            {
                TextBlock txt;
                StackPanel st;
                _ismouseover = false;
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
                UpdateVisualState();
            }
            base.OnMouseLeave(e);
        }
        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            _ismouseover = true;
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
            UpdateVisualState();
            base.OnMouseEnter(e);
        }
#endif
#if WPF
         protected override void OnMouseEnter(MouseEventArgs e)
        {
            _ismouseover = true;
            UpdateVisualState();
            base.OnMouseEnter(e);
      
  }
         protected override void OnMouseLeave(MouseEventArgs e)
         {
             if (!IsDropDownOpen)
             {
                 _ismouseover = false;
                 UpdateVisualState();
             }
             base.OnMouseLeave(e);
         }

#endif

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _ismouseover = true;
            UpdateVisualState();
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
#if WPF
            switch (e.Key)
            {
                case Key.F4:
                    if (this.IsDropDownOpen == false)
                        this.IsDropDownOpen = true;
                    else
                        this.IsDropDownOpen = false;
                    break;
                case Key.Up:
                    break;
                case Key.Down:
                    break;
                case Key.Delete:
                    break;
                case Key.End:
                    break;
                case Key.Enter:
                    break;
                case Key.Escape:
                    if (this.IsDropDownOpen == true)
                        this.IsDropDownOpen = false;
                    break;
                case Key.Back:
                    break;
                case Key.Left:
                    break;
                case Key.None:
                    break;
                default:
                    break;
            }   
#endif
            base.OnKeyDown(e);
        }

#if WPF
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (ModifierKeys.Alt == Keyboard.Modifiers)
            {
                if (e.SystemKey == Key.Down || e.SystemKey == Key.Up)
                {
                    if (this.IsDropDownOpen == false)
                        this.IsDropDownOpen = true;
                    else
                        this.IsDropDownOpen = false;
                }
            }
            base.OnPreviewKeyDown(e);
        }

#endif
        /// <summary>
        /// 
        /// </summary>
        protected internal bool isopened;
        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsPressed = true;
            if (IsDropDownOpen)
                isopened = true;
            else
                isopened = false;
            e.Handled = true;
#if SILVERLIGHT
            CancelEventArgs args = new CancelEventArgs();
            if (DropDownOpening != null)
            {
                DropDownOpening(this, args);
            }
            UpdateVisualState();

            if (!args.Cancel)
            {
                IsDropDownOpen = true;
                OnIsDropDownOpenChanged();
            }
            else
            {
                IsPressed = false;
            }
#endif
        }

        /// <summary>
        /// Updates the drop direction.
        /// </summary>
        private void UpdateDropDirection()
        {
            if (_dropdown != null)
            {
#if SILVERLIGHT
                switch (this.DropDirection)
                {
                    case Controls.DropDirection.BottomLeft:
                        {
                            _dropdown.HorizontalOffset = 0.0;
                            _dropdown.VerticalOffset = ActualHeight;
                            break;
                        }
                    case Controls.DropDirection.BottomRight:
                        {
                            _dropdown.HorizontalOffset = ActualWidth;
                            _dropdown.VerticalOffset = ActualHeight;
                            break;
                        }
                    case Controls.DropDirection.TopLeft:
                        {
                            _dropdown.HorizontalOffset = 0.0;
                            if (SizeMode == Controls.SizeMode.Large)
                                _dropdown.VerticalOffset = -ActualHeight;
                            if (SizeMode == Controls.SizeMode.Normal)
                                _dropdown.VerticalOffset = _dropdown.ActualHeight - ActualHeight;
                            break;
                        }
                    case Controls.DropDirection.TopRight:
                        {
                            _dropdown.HorizontalOffset = ActualWidth;
                            if(SizeMode == Controls.SizeMode.Large)
                            _dropdown.VerticalOffset = -ActualHeight;
                            if(SizeMode == Controls.SizeMode.Normal)
                                _dropdown.VerticalOffset = _dropdown.ActualHeight - ActualHeight;
                            break;
                        }
                    case Controls.DropDirection.Right:
                        {
                            _dropdown.HorizontalOffset = ActualWidth;
                            _dropdown.VerticalOffset = 0.0;
                            break;
                        }
                    case Controls.DropDirection.Left:
                        {
                            _dropdown.VerticalOffset = 0.0;
                            _dropdown.HorizontalOffset = -_dropdown.ActualWidth - (ActualWidth/2);
                            break;
                        }
                }
#else
                 switch (this.DropDirection)
                {
                    case Controls.DropDirection.BottomLeft:
                        {
                            _dropdown.HorizontalOffset = 0.0;
                            break;
                        }
                    case Controls.DropDirection.BottomRight:
                        {
                            _dropdown.HorizontalOffset = ActualWidth;
                            break;
                        }
                    case Controls.DropDirection.TopLeft:
                        {
                            _dropdown.HorizontalOffset = 0.0;
                            if (SizeMode == Controls.SizeMode.Large)
                                _dropdown.VerticalOffset = -ActualHeight;
                            if (SizeMode == Controls.SizeMode.Normal)
                                _dropdown.VerticalOffset = _dropdown.ActualHeight - ActualHeight;
                            break;
                        }
                    case Controls.DropDirection.TopRight:
                        {
                            _dropdown.HorizontalOffset = ActualWidth;
                            if(SizeMode == Controls.SizeMode.Large)
                            _dropdown.VerticalOffset = -ActualHeight;
                            if(SizeMode == Controls.SizeMode.Normal)
                                _dropdown.VerticalOffset = _dropdown.ActualHeight - ActualHeight;
                            break;
                        }
                    case Controls.DropDirection.Right:
                        {
                            _dropdown.Placement = PlacementMode.Right;
                            break;
                        }
                    case Controls.DropDirection.Left:
                        {
                            _dropdown.Placement = PlacementMode.Left;
                            break;
                        }
                }
#endif

            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_dropdown != null)
            {
#if SILVERLIGHT
                if (_dropdown.IsOpen)
                {
                    VisualStateManager.GoToState(this, "Pressed", true);

                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", true);

                }
#else 
                CancelEventArgs args = new CancelEventArgs();
                
                if (DropDownOpening != null)
                {
                    DropDownOpening(this, args);
                }
             
                    if (IsDropDownOpen)
                    {
                        IsPressed = false;
                        IsDropDownOpen = false;
                        
                    }
                    else
                    {
                        if (!isopened)
                        {
                            if (!args.Cancel)
                            {
                                if (((this.Content as DropDownMenuGroup !=null) && e.Source is DropDownButtonAdv && (this.Content as DropDownMenuGroup).Items.Count!=0) ||(!(this.Content as DropDownMenuGroup!=null)))
                                {
                                    IsPressed = true;
                                    IsDropDownOpen = true;
                                }
                                else
                                {
                                    IsPressed = false;
                                }
                            }
                            else
                            {
                                IsPressed = false;
                            }
                        }

                    }
                
#endif
            }
            e.Handled = true;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            InitSizeForm();
            InitMultiLine();
#if SILVERLIGHT
            _dropdown = GetTemplateChild("PART_DropDown") as DropDown;
            if (_dropdown != null)
            {
                _dropdown.IsOpenChanged +=new EventHandler(_dropdown_IsOpenChanged);
            }

#else
               _dropdown = GetTemplateChild("PART_DropDown") as Popup;
               if (_dropdown != null)
               {
                   _dropdown.Closed += new EventHandler(_dropdown_Closed);
                   _dropdown.Opened += new EventHandler(_dropdown_Opened);
               }
#endif
            OnIsDropDownOpenChanged();
            base.OnApplyTemplate();
            UpdateVisualState();
        }

        void _dropdown_Opened(object sender, EventArgs e)
        {
            if (DropDownOpened != null)
            {
                DropDownOpened(this, new RoutedEventArgs());
            }
        }

        private void _dropdown_Closed(object sender, EventArgs e)
        {
            CancelEventArgs args = new CancelEventArgs();
            if (DropDownClosing != null)
            {
                DropDownClosing(this, args);
            }

            IsDropDownOpen = false;
            IsPressed = false;

            if (DropDownClosed != null)
            {
                DropDownClosed(this, new RoutedEventArgs());
            }

        }

        private void _dropdown_IsOpenChanged(object sender, EventArgs e)
        {
            if (!_dropdown.IsOpen)
            {
                CancelEventArgs args = new CancelEventArgs();
                if (DropDownClosing != null)
                {
                    DropDownClosing(this, args);
                }
                if (!args.Cancel)
                {
                    IsDropDownOpen = false;
                    IsPressed = false;

                    _ismouseover = false;
                    UpdateVisualState();
                    if (DropDownClosed != null)
                    {
                        DropDownClosed(this, new RoutedEventArgs());
                    }
                }
                else
                {
                    IsDropDownOpen = true;
                    _dropdown.IsOpen = true;
                }
            }
            else
            {
                IsPressed = true;
                UpdateVisualState();
                if (DropDownOpened != null)
                {
                    DropDownOpened(this, new RoutedEventArgs());
                }

            }
           
        }
        #endregion
    }

    /// <summary>
    /// Represents the DropDirection Enumeration.
    /// </summary>
    public enum DropDirection
    {
        /// <summary>
        /// Drop down is opened below and to left side of the owner.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Drop down is opened below and to right side of the owner.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Drop down is opened above and to left side of the owner.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Drop down is opened above and to right side of the owner.
        /// </summary>
        TopRight,

        /// <summary>
        /// Drop down is opened to right side of the owner.
        /// </summary>
        Right,

        /// <summary>
        /// 
        /// </summary>
        Left
    }
}
