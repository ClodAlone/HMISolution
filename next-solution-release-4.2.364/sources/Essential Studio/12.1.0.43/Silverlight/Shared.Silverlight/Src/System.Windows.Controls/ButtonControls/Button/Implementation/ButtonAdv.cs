#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
#if SILVERLIGHT
using Syncfusion.Windows.Controls.Theming;
#endif
#if WPF
using Syncfusion.Licensing;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Button Control.
    /// </summary>
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Checked")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "UnChecked")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Disabled")]
# if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Blend;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Default;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
        Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Metro;component/ButtonAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
       Type = typeof(ButtonAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/ButtonAdv.xaml")]
#endif

#if WPF
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/ButtonAdv.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/TransparentStyle.xaml")]
#endif

    public class ButtonAdv : ButtonBase, ICommandSource, IButtonAdv
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonAdv"/> class.
        /// </summary>
        public ButtonAdv()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ButtonAdv));
            }
#endif
            DefaultStyleKey = typeof(ButtonAdv);
            Initialize();
        }

#if WPF

        /// <summary>
        /// Checking the License.
        /// </summary>
        static ButtonAdv()
        {
         //   EnvironmentTest.ValidateLicense(typeof(ButtonAdv));
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="ButtonAdv"/> class.
        /// </summary>
        static ButtonAdv()
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

        private bool _ispressed = false;

        private Border _normal;

        private Border _large;

#if WPF
        private AccessText _textNormal;
#endif
        
#if SILVERLIGHT

        /// <summary>
        /// 
        /// </summary>
        public bool _istempsize = false;
        private TextBlock _textNormal;
#endif
        internal ItemsControl _multilineTextLarge;
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
            DependencyProperty.Register("Label", typeof(string), typeof(ButtonAdv), new PropertyMetadata(string.Empty));


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
            DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(ButtonAdv), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [Category("Common Properties")]
        [Description("Represents the value, whether the element can be checkable or not")]
        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsToggle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof(bool), typeof(ButtonAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckableChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        [Category("Common Properties")]
        [Description("Represents the value, whether the element is checked or not")]
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

       /// <summary>
       /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ButtonAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedChanged)));


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
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(ButtonAdv), new PropertyMetadata(null));

        /// <summary>
        /// Sets the IconWidth.
        /// </summary>
        /// <value>The Icon Width.</value>
        [Category("Common Properties")]
        [Description("Represents to set the Image width")]
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

       /// <summary>
       /// Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(ButtonAdv), new PropertyMetadata(16.0));

        /// <summary>
        /// Sets the IconHeight.
        /// </summary>
        /// <value>The Icon Height.</value>
        [Category("Common Properties")]
        [Description("Represents to set the Image height")]
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(ButtonAdv), new PropertyMetadata(16.0));        
        
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
            DependencyProperty.Register("SizeMode", typeof(SizeMode), typeof(ButtonAdv), new PropertyMetadata(SizeMode.Normal, new PropertyChangedCallback(OnSizeFormChanged)));

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
            DependencyProperty.Register("IsMultiLine", typeof(bool), typeof(ButtonAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnIsMultiLineChanged)));

        #endregion

        /// <summary>
        /// Occurs when [checked].
        /// </summary>
        public event RoutedEventHandler Checked;

        #region Implementation
        internal string VisualState
        {
            get
            {
                if (this.IsEnabled)
                {
                    if (this._ismouseover)
                    {
                        if (this._ispressed)
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

        internal string State
        {
            get
            {
                if (this.IsChecked)
                {
                    return "Checked";
                }

                return "UnChecked";
            }
        }


        internal void UpdateVisualState()
        {
            VisualStateManager.GoToState(this, this.VisualState, true);
        }

        internal void UpdateCheckState()
        {
            if (IsChecked)
            {
                VisualStateManager.GoToState(this, "Checked", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "UnChecked", true);
            }
        }

        private void Initialize()
        {
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
        }

        private void InitSizeForm()
        {
            _large = GetTemplateChild("ItemBorder1") as Border;
            _normal = GetTemplateChild("ItemBorder") as Border;
#if WPF
            _textNormal = GetTemplateChild("PART_Text") as AccessText;
#endif
#if SILVERLIGHT
            _textNormal = GetTemplateChild("PART_Text") as TextBlock;
#endif
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
            if (this.SmallIcon == null)
            {
                this.IconHeight = 0;
                this.IconWidth = 0;
            }

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
        }

        void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                _ismouseover = false;
                _ispressed = false;
                //this.IsChecked = false;
            }
            UpdateVisualState();
        }

        private static void OnIsCheckableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as ButtonAdv;
            sender.OnIsCheckableChanged();
        }

        private void OnIsCheckableChanged()
        {
            if (IsChecked)
                IsChecked = false;
            OnIsCheckedChanged();
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as ButtonAdv;
            sender.OnIsCheckedChanged();
        }

        private void OnIsCheckedChanged()
        {
            if (IsCheckable)
            {
                if (Checked != null)
                {
                    Checked(this, new RoutedEventArgs());
                }

                UpdateCheckState();
            }
        }

        private static void OnSizeFormChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as ButtonAdv;
            sender.OnSizeFormChanged();
        }

        private void OnSizeFormChanged()
        {
            UpdateSizeForm(SizeMode);
        }

        private static void OnIsMultiLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as ButtonAdv;
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
#if WPF
          protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _ispressed = true;
           
            if (IsCheckable)
            {
                if (IsChecked)
                {
                    IsChecked = false;
                }
                else
                {
                    IsChecked = true;
                }
            }
            else
            {
                UpdateVisualState();
            }
            base.OnMouseLeftButtonDown(e);
        }
 protected override void OnMouseLeave(MouseEventArgs e)
        {
            _ismouseover = false;
            if (!IsCheckable)
            {
                UpdateVisualState();
            }
            else
            {
                UpdateCheckState();
            }
            base.OnMouseLeave(e);
        }
         protected override void OnMouseEnter(MouseEventArgs e)
        {
            _ismouseover = true;
            UpdateVisualState();
            base.OnMouseEnter(e);


        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeave"/> routed event that occurs when the mouse leaves an element.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.MouseLeave"/> event.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            _ismouseover = false;

            if (FindAncestorVisualStyle(this) == "Blend")
            {
                TextBlock txt;
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

                }
            }
            }
            if (!IsCheckable)
            {
                UpdateVisualState();
            }
            else
            {
                UpdateCheckState();
            }
            base.OnMouseLeave(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _ispressed = true;
            if (FindAncestorVisualStyle(this) == "Blend")
            {
                TextBlock txt;
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

                    }
                }
            }
            if (IsCheckable)
            {
                if (IsChecked)
                {
                    IsChecked = false;
                }
                else
                {
                    IsChecked = true;
                }
            }
            else
            {
                UpdateVisualState();
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            TextBlock txt;
            _ismouseover = true;
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
                    }
                }
            }
            UpdateVisualState();
            base.OnMouseEnter(e);


        }
#endif
        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseMove"/> event that occurs when the mouse pointer moves while over this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _ismouseover = true;
            UpdateVisualState();
            base.OnMouseMove(e);
        }

        ///// <summary>
        ///// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event that occurs when the left mouse button is pressed while the mouse pointer is over this control.
        ///// </summary>
        ///// <param name="e">The event data.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        ///// 	<paramref name="e"/> is null.</exception>
      


        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> event.
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event that occurs when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="e"/> is null.</exception>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _ispressed = false;
            if (!IsCheckable)
            {
                UpdateVisualState();
            }
            else
            {
                if (IsChecked)
                {
                    VisualStateManager.GoToState(this, "Checked", true);
                }
                else
                {
                    UpdateVisualState();
                }
                
            }
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            UpdateVisualState();
            if (IsCheckable)
            {
                UpdateCheckState();
            }
            InitSizeForm();
            InitMultiLine();
            base.OnApplyTemplate();
        }
        #endregion
    }

    /// <summary>
    /// Represents the Size Mode Enumeration.
    /// </summary>
    public enum SizeMode
    {
        /// <summary>
        /// Represents the Normal value.
        /// </summary>
        Normal,

        /// <summary>
        /// Represents the Normal value.
        /// </summary>
        Small,

        /// <summary>
        /// Represents the Normal value.
        /// </summary>
        Large
    }
}
