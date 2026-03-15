// <copyright file="ChromelessWindow.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// ChromelessWindow class provides us a new window
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
   Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
  Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
 Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(ChromelessWindow), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/TransparentStyle.xaml")]
    public class ChromelessWindow : Window
    {
        #region Fields

        /// <summary>
        /// Define the Enabled value
        /// </summary>
        private bool m_dwmEnabled;

        /// <summary>
        /// Define the Sizing
        /// </summary>
        private bool m_isSizing;

        /// <summary>
        /// Define the Initialization
        /// </summary>
        private bool m_isInitialized;

        /// <summary>
        /// Define restore button
        /// </summary>
        private TitleButton restButton;

        internal bool updateNew = false;

        /// <summary>
        /// Define maximize button
        /// </summary>
        private TitleButton maxButton;

        private Border contentArea;

        ///<summary>
        /// Define minimize button
        /// </summary>
        private TitleButton minButton = null;


        /// <summary>
        /// Define title bar point
        /// </summary>
        private Point m_lastTitlebarPoint;

        /// <summary>
        /// Define icon click
        /// </summary>
        private DateTime m_lastIconClick;

        /// <summary>
        /// Define Icon point
        /// </summary>
        private Point m_lastIconPoint;

        /// <summary>
        /// Define icon
        /// </summary>
        private Image m_icon;

        /// <summary>
        /// HWND reference variable.
        /// </summary>
        private IntPtr window_hwnd;

        private System.Windows.Controls.Label ResizeGrip;

        /// <summary>
        /// Previous state for menu.
        /// </summary>
        private WindowState lastMenuState;

        private bool iscustommaxhghtused = false, iscustommaxwidthused = false;

        #endregion Fields

        #region Commands

        /// <summary>
        /// Command that is executed when click close button in the chromelesswindow
        /// </summary>
        public static RoutedUICommand CloseWindow = new RoutedUICommand("Close", "CloseWindow", typeof(ChromelessWindow));

        /// <summary>
        /// Command that is executed when click minimize button in the chromelesswindow
        /// </summary>
        public static RoutedUICommand ToggleMaximizedState = new RoutedUICommand("Maximize", "ToggleMaximizedState", typeof(ChromelessWindow));

        /// <summary>
        /// Command that is executed when click maximize button in the chromelesswindow
        /// </summary>
        public static RoutedUICommand ToggleMinimizedState = new RoutedUICommand("Minimize", "ToggleMinimizedState", typeof(ChromelessWindow));

        /// <summary>
        /// Maximizes the window executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void MaximizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            SystemButtonsUpdate();
        }

        /// <summary>
        /// Minimizes the window executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void MinimizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Minimized) ? WindowState.Normal : WindowState.Minimized;
        }

        /// <summary>
        /// Closes the window executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CloseWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        #endregion Commands

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is glass active.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is glass active; otherwise, <c>false</c>.
        /// </value>
        internal bool IsGlassActive
        {
            get
            {
                return (bool)GetValue(IsGlassActiveProperty);
            }

            set
            {
                SetValue(IsGlassActiveProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can enable glass.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can enable glass; otherwise, <c>false</c>.
        /// </value>
        private bool CanEnableGlass
        {
            get
            {
                return this.m_dwmEnabled && this.IsGlassActive;
            }
        }

        /// <summary>
        /// Gets the DPI offset.
        /// </summary>
        /// <value>The DPI offset.</value>
        private double DPIOffset
        {
            get
            {
                double num = 1.0;
                Point point = ChromelessWindowInterop.GetTransformedPoint(this);
                if (point.Y != 96.0)
                {
                    num = (double)(num + ((point.Y - 96.0) / 96.0));
                }

                return num;
            }
        }

        /// <summary>
        /// Gets or sets value that represents border brush of the resizable window.
        /// </summary>
        /// <value>The resize border brush.</value>
        public Brush ResizeBorderBrush
        {
            get
            {
                return (Brush)GetValue(ResizeBorderBrushProperty);
            }

            set
            {
                SetValue(ResizeBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents background brush of TitleBar.
        /// </summary>
        /// <value>The title bar background.</value>
        public Brush TitleBarBackground
        {
            get
            {
                return (Brush)GetValue(TitleBarBackgroundProperty);
            }

            set
            {
                SetValue(TitleBarBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents background brush of TitleBar.
        /// </summary>
        /// <value>The title bar background.</value>
        public Brush TitleBarForeground
        {
            get
            {
                return (Brush)GetValue(TitleBarForegroundProperty);
            }

            set
            {
                SetValue(TitleBarForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents background brush of <see cref="NavigationBar"/>.
        /// </summary>
        /// <value>The navigation bar background.</value>
        public LinearGradientBrush NavigationBarBackground
        {
            get
            {
                return (LinearGradientBrush)GetValue(NavigationBarBackgroundProperty);
            }

            set
            {
                SetValue(NavigationBarBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents background of window content area.
        /// </summary>
        /// <value>The window content area border brush.</value>
        public Brush WindowContentAreaBorderBrush
        {
            get
            {
                return (Brush)GetValue(WindowContentAreaBorderBrushProperty);
            }

            set
            {
                SetValue(WindowContentAreaBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents border brush thickness of the resizable window.
        /// </summary>
        /// <value>The resize border thickness.</value>
        public Thickness ResizeBorderThickness
        {
            get
            {
                return (Thickness)GetValue(ResizeBorderThicknessProperty);
            }

            set
            {
                SetValue(ResizeBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents the template for the TitleBar.
        /// </summary>
        /// <value>The title bar template.</value>
        public ControlTemplate TitleBarTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(TitleBarTemplateProperty);
            }

            set
            {
                SetValue(TitleBarTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents the template for the Maximize Button of the TitleBar.
        /// </summary>
        /// <value>The maximize button template.</value>
        public ControlTemplate MaximizeButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(MaximizeButtonTemplateProperty);
            }

            set
            {
                SetValue(MaximizeButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents the template for the Minimize Button of the TitleBar.
        /// </summary>
        /// <value>The minimize button template.</value>
        public ControlTemplate MinimizeButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(MinimizeButtonTemplateProperty);
            }

            set
            {
                SetValue(MinimizeButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents the template for the Restore Button of the TitleBar.
        /// </summary>
        public ControlTemplate RestoreButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(RestoreButtonTemplateProperty);
            }

            set
            {
                SetValue(RestoreButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents the template for the Close Button of the TitleBar.
        /// </summary>
        /// <value>The close button template.</value>
        public ControlTemplate CloseButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(CloseButtonTemplateProperty);
            }

            set
            {
                SetValue(CloseButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the title bar.
        /// </summary>
        /// <value>The title bar.</value>
        public TitleBar TitleBar
        {
            get
            {
                return (TitleBar)base.GetTemplateChild("PART_TitleBar");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public System.Windows.HorizontalAlignment TitleTextAlignment
        {
            get { return (System.Windows.HorizontalAlignment)GetValue(TitleTextAlignmentProperty); }
            set { SetValue(TitleTextAlignmentProperty, value); }
        }

        /// <summary>
        /// Defines alignment of the Title Text.
        /// </summary>
        public static readonly DependencyProperty TitleTextAlignmentProperty =
            DependencyProperty.Register("TitleTextAlignment", typeof(System.Windows.HorizontalAlignment), typeof(ChromelessWindow), new FrameworkPropertyMetadata(System.Windows.HorizontalAlignment.Left));

        /// <summary>
        /// Gets or sets value that represents the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        /// <remarks>
        /// Remark: The CornerRadius value should be set to a minimum of 4 for effective resizing.
        /// </remarks>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        internal CornerRadius cornerRadius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show icon].
        /// </summary>
        /// <value><c>true</c> if [show icon]; otherwise, <c>false</c>.</value>
        public bool ShowIcon
        {
            get
            {
                return (bool)GetValue(ShowIconProperty);
            }
            set
            {
                SetValue(ShowIconProperty, value);
            }
        }

        #endregion Properties

        #region DP's

        /// <summary>
        /// Gets or sets Stlye for the ResizeGrip.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Style ResizeGripStyle
        {
            get { return (Style)GetValue(ResizeGripStyleProperty); }
            set { SetValue(ResizeGripStyleProperty, value); }
        }

        /// <summary>
        /// Defines Style for the ResizeGrip. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeGripStyleProperty =
            DependencyProperty.Register("ResizeGripStyle", typeof(Style), typeof(ChromelessWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets NativeChrome property set for the window.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool UseNativeChrome
        {
            get { return (bool)GetValue(UseNativeChromeProperty); }
            set { SetValue(UseNativeChromeProperty, value); }
        }

        /// <summary>
        /// Defines NativeChrome for the window. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty UseNativeChromeProperty =
            DependencyProperty.Register("UseNativeChrome", typeof(bool), typeof(ChromelessWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUseNativeChromeChanged)));

        /// <summary>
        /// Defines whether Glass effects are active. This is a dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsGlassActiveProperty =
            DependencyProperty.Register("IsGlassActive", typeof(bool), typeof(ChromelessWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsGlassActiveChanged)));

        /// <summary>
        /// Identifies <see cref="TitleBarTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleBarTemplateProperty =
          DependencyProperty.Register("TitleBarTemplate", typeof(ControlTemplate), typeof(ChromelessWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="MaximizeButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonTemplateProperty =
           DependencyProperty.Register("MaximizeButtonTemplate", typeof(ControlTemplate), typeof(ChromelessWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="MinimizeButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonTemplateProperty =
                   DependencyProperty.Register("MinimizeButtonTemplate", typeof(ControlTemplate), typeof(ChromelessWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="RestoreButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RestoreButtonTemplateProperty =
                  DependencyProperty.Register("RestoreButtonTemplate", typeof(ControlTemplate), typeof(ChromelessWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="CloseButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonTemplateProperty =
                  DependencyProperty.Register("CloseButtonTemplate", typeof(ControlTemplate), typeof(ChromelessWindow), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="ResizeBorderBrushProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeBorderBrushProperty = DependencyProperty.Register("ResizeBorderBrush", typeof(Brush), typeof(ChromelessWindow));

        /// <summary>
        /// Identifies <see cref="TitleBarBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register("TitleBarBackground", typeof(Brush), typeof(ChromelessWindow));

        /// <summary>
        /// Identifies <see cref="TitleBarForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleBarForegroundProperty = DependencyProperty.Register("TitleBarForeground", typeof(Brush), typeof(ChromelessWindow));

        /// <summary>
        /// Identifies <see cref="NavigationBarBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationBarBackgroundProperty = DependencyProperty.Register("NavigationBarBackground", typeof(Brush), typeof(ChromelessWindow));

        /// <summary>
        /// Identifies <see cref="WindowContentAreaBorderBrushProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowContentAreaBorderBrushProperty = DependencyProperty.Register("WindowContentAreaBorderBrush", typeof(Brush), typeof(ChromelessWindow));

        /// <summary>
        /// Identifies <see cref="ResizeBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty;

        /// <summary>
        /// Identifies <see cref="WindowCornerRadiusProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowCornerRadiusProperty;

        /// <summary>
        /// Identifies <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty;

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register
            ("ShowIcon", typeof(bool), typeof(ChromelessWindow), new PropertyMetadata(true));

        /// <summary>
        /// Used to hide the TaskBar when WindowStyle set to None.
        /// </summary>
        public bool HideTaskBar
        {
            get { return (bool)GetValue(HideTaskBarProperty); }
            set { SetValue(HideTaskBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HideTaskBar.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty HideTaskBarProperty =
            DependencyProperty.Register("HideTaskBar", typeof(bool), typeof(ChromelessWindow), new UIPropertyMetadata(false));

        private static readonly DependencyProperty WindowSizePropertyDescriptor =
      DependencyProperty.RegisterAttached("WindowSizeDescriptor",
                                           typeof(DependencyPropertyDescriptor),
                                         typeof(ChromelessWindow));

        /// <summary>
        /// Calls OnIsGlassActiveChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsGlassActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromelessWindow instance = (ChromelessWindow)d;
            instance.OnIsGlassActiveChanged(e);
        }

        private void OnWindowSizeDpChanged()
        {
            if (!BindingOperations.IsDataBound(this, Window.ResizeModeProperty))
            {
                var dpd = (DependencyPropertyDescriptor)GetValue(WindowSizePropertyDescriptor);
                if (dpd != null)
                {
                    dpd.RemoveValueChanged(this, ChromelessWindow_StateChanged);
                    SetValue(WindowSizePropertyDescriptor, null);
                }
            }
        }

        /// <summary>
        /// Raises IsGlassActiveChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsGlassActiveChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsGlassActiveChanged != null)
            {
                IsGlassActiveChanged(this, e);
            }
        }

        /// <summary>
        /// Raises UseNativeChromeChanged event.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnUseNativeChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromelessWindow instance = (ChromelessWindow)d;
            if (instance.UseNativeChrome)
            {
#if !SyncfusionFramework3_5
                WindowChrome chrome = new WindowChrome();
                chrome.UseAeroCaptionButtons = false;
                chrome.GlassFrameThickness = new Thickness(0);
                BindingOperations.SetBinding(chrome, ChromelessWindow.CornerRadiusProperty, new System.Windows.Data.Binding { Path = new PropertyPath("CornerRadius"), Source = instance });
                BindingOperations.SetBinding(chrome, ChromelessWindow.ResizeBorderThicknessProperty, new System.Windows.Data.Binding { Path = new PropertyPath("ResizeBorderThickness"), Source = instance });
                WindowChrome.SetWindowChrome(instance, chrome);
#endif
            }
        }

        /// <summary>
        /// Occurs when [is glass active changed].
        /// </summary>
        public event PropertyChangedCallback IsGlassActiveChanged;

        /// <summary>
        /// Called when [resize property thickness changed].
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnResizePropertyThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChromelessWindow instance = (ChromelessWindow)d;
        }

        #endregion DP's

        #region Initialize

        /// <summary>
        /// Initializes static members of the <see cref="ChromelessWindow"/> class.
        /// </summary>
        static ChromelessWindow()
        {
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(ChromelessWindow), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(ChromelessWindow), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ChromelessWindow), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(ChromelessWindow), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromelessWindow), new FrameworkPropertyMetadata(typeof(ChromelessWindow)));

            Thickness defaultThickness = new Thickness(SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight, SystemParameters.ResizeFrameVerticalBorderWidth, SystemParameters.ResizeFrameHorizontalBorderHeight);
            ResizeBorderThicknessProperty = DependencyProperty.Register(
                "ResizeBorderThickness",
                typeof(Thickness),
                typeof(ChromelessWindow),
                new FrameworkPropertyMetadata(defaultThickness, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender, OnResizePropertyThicknessChanged));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ChromelessWindow), new FrameworkPropertyMetadata(new CornerRadius(0d)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromelessWindow"/> class.
        /// </summary>
        public ChromelessWindow()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ChromelessWindow));
            }
            CommandBindings.Add(new CommandBinding(CloseWindow, CloseWindowExecuted));
            CommandBindings.Add(new CommandBinding(ToggleMinimizedState, MinimizeWindowExecuted));
            CommandBindings.Add(new CommandBinding(ToggleMaximizedState, MaximizeWindowExecuted));
            this.StateChanged += new EventHandler(ChromelessWindow_StateChanged);
            this.Loaded += new RoutedEventHandler(ChromelessWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(ChromelessWindow_Unloaded);
        }

        private void ChromelessWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(ChromelessWindow_Unloaded);
            OnWindowSizeDpChanged();
        }

        /// <summary>
        /// Handles the Loaded event of the ChromelessWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(Window.ResizeModeProperty, GetType());
            dpd.AddValueChanged(this, ChromelessWindow_StateChanged);
            SetValue(WindowSizePropertyDescriptor, dpd);
            if (!UseNativeChrome)
            {
                this.IsGlassActive = false;
                window_hwnd = new WindowInteropHelper(this).Handle;
                this.m_dwmEnabled = ChromelessWindowInterop.CanEnableDwm();
                if (!m_dwmEnabled)
                {
                    IsGlassActive = false;
                }

                if (this.MaxHeight != double.PositiveInfinity)
                    iscustommaxhghtused = true;
                if (this.MaxWidth != double.PositiveInfinity)
                    iscustommaxwidthused = true;
            }
            CheckVisualStyle();
        }

        /// <summary>
        /// Handles the StateChanged event of the ChromelessWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ChromelessWindow_StateChanged(object sender, EventArgs e)
        {
            SystemButtonsUpdate();
            if (!UseNativeChrome)
                UpdateWindowRegion(true);
        }

        /// <summary>
        /// Updates system buttons visibility.
        /// </summary>
        protected internal void SystemButtonsUpdate()
        {
            if (this.WindowState == WindowState.Normal)
            {
                if (restButton != null)
                {
                    restButton.Visibility = Visibility.Collapsed;
                }

                if (maxButton != null)
                {
                    maxButton.Visibility = Visibility.Visible;
                }
            }

            if (this.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
            }
            
            if (this.ResizeMode == ResizeMode.NoResize)
            {
                ResizeBorderThickness = new Thickness(1);
                if (maxButton != null)
                {
                    maxButton.Visibility = Visibility.Collapsed;
                }

                if (restButton != null)
                {
                    restButton.Visibility = Visibility.Collapsed;
                }

                if (minButton != null)
                {
                    minButton.Visibility = Visibility.Collapsed;
                }
            }

            if ((this.ResizeMode == ResizeMode.NoResize) && (this.WindowState == WindowState.Maximized))
            {
                if (maxButton != null)
                {
                    maxButton.Visibility = Visibility.Collapsed;
                }

                if (restButton != null)
                {
                    restButton.Visibility = Visibility.Collapsed;
                }

                if (minButton != null)
                {
                    minButton.Visibility = Visibility.Collapsed;
                }
            }

            if (this.ResizeMode == ResizeMode.CanMinimize)
            {
                if (maxButton != null)
                {
                    maxButton.IsEnabled = false;
                }

                if (restButton != null)
                {
                    restButton.IsEnabled = false;
                }
            }

            if ((this.WindowState == WindowState.Maximized) && (this.ResizeMode != ResizeMode.NoResize))
            {
                if (maxButton != null)
                {
                    maxButton.Visibility = Visibility.Collapsed;
                }

                if (restButton != null)
                {
                    restButton.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion Initialize

        #region Implementation

        /// <summary>
        /// Called when [icon mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnIconMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if ((!e.Handled && (e.ChangedButton == MouseButton.Left)) && this.IsMouseOver)
            {
                Point position = e.GetPosition(this);
                if (((DateTime.Now.Subtract(m_lastIconClick).TotalMilliseconds < 500) && (Math.Abs((double)(m_lastIconPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastIconPoint.Y - position.Y)) <= 2))
                {
                    this.Close();
                }
                else
                {
                    m_lastIconPoint = e.GetPosition(this);
                }

                m_lastIconClick = DateTime.Now;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the imgicon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Imgicon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((!e.Handled && (e.ChangedButton == MouseButton.Left)) && this.IsMouseOver)
            {
                e.Handled = true;
                Point point = base.PointToScreen(e.GetPosition(this));
                point.Y = point.Y + 20;
                _UpdateSystemMenu(this.WindowState);
                ChromelessWindowInterop.ShowSystemMenu(window_hwnd, point);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.TitleBar != null && !UseNativeChrome)
            {
                if ((!e.Handled && (e.ChangedButton == MouseButton.Right)) && this.TitleBar.IsMouseOver)
                {
                    e.Handled = true;
                    IntPtr handle = new WindowInteropHelper(this).Handle;
                    Point point = base.PointToScreen(e.GetPosition(this));
                    _UpdateSystemMenu(this.WindowState);
                    ChromelessWindowInterop.ShowSystemMenu(handle, point);
                }
            }
        }

        /// <summary>
        /// To update system menu based on current state of the menu items.
        /// </summary>
        /// <param name="assumeState">State of the assume.</param>
        private void _UpdateSystemMenu(WindowState? assumeState)
        {
            const SystemMenuItemBehavior MfEnabled = SystemMenuItemBehavior.ENABLED | SystemMenuItemBehavior.BYCOMMAND;
            const SystemMenuItemBehavior MfDisabled = SystemMenuItemBehavior.GRAYED | SystemMenuItemBehavior.DISABLED | SystemMenuItemBehavior.BYCOMMAND;

            WindowState state = assumeState ?? _GetHwndState();

            if (null != assumeState || lastMenuState != state)
            {
                lastMenuState = state;

                bool modified = _ModifyStyle(WindowStyleValues.VISIBLE, 0);
                IntPtr hmenu = NativeMethods.GetSystemMenu(window_hwnd, false);
                if (IntPtr.Zero != hmenu)
                {
                    var dwStyle = (WindowStyleValues)NativeMethods.GetWindowLongPtr(window_hwnd, GWL.STYLE).ToInt32();

                    bool canMinimize = IsFlagSet((int)dwStyle, (int)WindowStyleValues.MINIMIZEBOX);
                    bool canMaximize = IsFlagSet((int)dwStyle, (int)WindowStyleValues.MAXIMIZEBOX);
                    bool canSize = IsFlagSet((int)dwStyle, (int)WindowStyleValues.THICKFRAME);

                    switch (state)
                    {
                        case WindowState.Maximized:
                            if (ResizeMode == ResizeMode.NoResize)
                            {
                                NativeMethods.EnableMenuItem(hmenu, SystemCommands.RESTORE, MfDisabled);
                                NativeMethods.EnableMenuItem(hmenu, SystemCommands.MOVE, MfEnabled);
                            }
                            else
                            {
                                NativeMethods.EnableMenuItem(hmenu, SystemCommands.RESTORE, MfEnabled);
                                NativeMethods.EnableMenuItem(hmenu, SystemCommands.MOVE, MfDisabled);
                            }
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.SIZE, MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MINIMIZE, canMinimize ? MfEnabled : MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MAXIMIZE, MfDisabled);
                            break;

                        case WindowState.Minimized:
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.RESTORE, MfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MOVE, MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.SIZE, MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MINIMIZE, MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MAXIMIZE, canMaximize ? MfEnabled : MfDisabled);
                            break;

                        default:
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.RESTORE, MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MOVE, MfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.SIZE, canSize ? MfEnabled : MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MINIMIZE, canMinimize ? MfEnabled : MfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, SystemCommands.MAXIMIZE, canMaximize ? MfEnabled : MfDisabled);
                            break;
                    }
                }

                if (modified)
                {
                    _ModifyStyle(0, WindowStyleValues.VISIBLE);
                }
            }
        }

        /// <summary>
        /// Determines whether [is flag set] [the specified value].
        /// </summary>
        /// <param name="value">The actual value.</param>
        /// <param name="mask">The mask value.</param>
        /// <returns>
        /// <c>true</c> if [is flag set] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsFlagSet(int value, int mask)
        {
            return 0 != (value & mask);
        }

        /// <summary>
        /// Get the WindowState as the native HWND knows it to be.  This isn't necessarily the same as what Window thinks.
        /// </summary>
        /// <returns>State of the window.</returns>
        private WindowState _GetHwndState()
        {
            var wpl = NativeMethods.GetWindowPlacement(new WindowInteropHelper(this).Handle);
            switch (wpl.showCmd)
            {
                case ShowWindowOptions.SHOWMINIMIZED: return WindowState.Minimized;
                case ShowWindowOptions.SHOWMAXIMIZED: return WindowState.Maximized;
            }

            return WindowState.Normal;
        }

        /// <summary>Add and remove a native WindowStyle from the HWND.</summary>
        /// <param name="removeStyle">The styles to be removed.  These can be bitwise combined.</param>
        /// <param name="addStyle">The styles to be added.  These can be bitwise combined.</param>
        /// <returns>Whether the styles of the HWND were modified as a result of this call.</returns>
        private bool _ModifyStyle(WindowStyleValues removeStyle, WindowStyleValues addStyle)
        {
            var dwStyle = (WindowStyleValues)NativeMethods.GetWindowLongPtr(window_hwnd, GWL.STYLE).ToInt32();
            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
            if (dwStyle == dwNewStyle)
            {
                return false;
            }

            NativeMethods.SetWindowLongPtr(window_hwnd, GWL.STYLE, new IntPtr((int)dwNewStyle));
            return true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.TitleBar != null && !UseNativeChrome)
            {
                if ((!e.Handled && (e.ChangedButton == MouseButton.Left)) && this.TitleBar.IsMouseOver)
                {
                    Point position = e.GetPosition(this);
                    if (((e.ClickCount == 2) && (Math.Abs((double)(m_lastTitlebarPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastTitlebarPoint.Y - position.Y)) <= 2))
                    {
                        if ((this.ResizeMode != ResizeMode.NoResize) && (this.ResizeMode != ResizeMode.CanMinimize))
                        {
                            if (this.WindowState != WindowState.Maximized)
                            {
                                this.WindowState = WindowState.Maximized;
                            }
                            else
                            {
                                this.WindowState = WindowState.Normal;
                                Width -= 1;
                                Width += 1;
                            }
                        }
                        else
                        {
                            if (this.WindowState == WindowState.Maximized)
                            {
                                if (this.WindowState != WindowState.Normal)
                                {
                                    this.WindowState = WindowState.Normal;
                                }
                            }
                        }
                    }
                    else
                    {
                        m_lastTitlebarPoint = e.GetPosition(this);
                        this.DragMove();
                    }
                }
            }
        }

        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            maxButton = (TitleButton)this.Template.FindName("PART_MaximizeButton", this);
            restButton = (TitleButton)this.Template.FindName("PART_RestoreButton", this);
            ResizeGrip = GetTemplateChild("PART_Resizegrip") as System.Windows.Controls.Label;
            m_icon = (Image)this.Template.FindName("PART_Icon", this);
            contentArea = GetTemplateChild("ContentAreaBorder") as Border;
            if (contentArea != null)
            {
                if (contentArea.BorderThickness == new Thickness(0))
                {
                    contentArea.BorderThickness = new Thickness(2, 0, 2, 2);
                }
            }

            if (m_icon != null)
            {
                m_icon.MouseLeftButtonUp += new MouseButtonEventHandler(Imgicon_MouseLeftButtonUp);
                m_icon.MouseDown += new MouseButtonEventHandler(OnIconMouseDown);
            }
            if (UseNativeChrome)
                HandleSizeToContent();
        }

        

        /// <summary>
        /// Checks if the entered VisualStyle exists, else sets Default style.
        /// </summary>
        private void CheckVisualStyle()
        {
            if (SkinStorage.GetVisualStyle(this) != "Default" &&
                SkinStorage.GetVisualStyle(this) != "Office2007Blue" &&
                SkinStorage.GetVisualStyle(this) != "Office2007Black" &&
                SkinStorage.GetVisualStyle(this) != "Office2007Silver" &&
                SkinStorage.GetVisualStyle(this) != "Office2003" &&
                SkinStorage.GetVisualStyle(this) != "Blend" &&
                SkinStorage.GetVisualStyle(this) != "Luna.NormalColor" &&
                SkinStorage.GetVisualStyle(this) != "Luna.Homestead" &&
                SkinStorage.GetVisualStyle(this) != "Luna.Metallic" &&
                SkinStorage.GetVisualStyle(this) != "Aero.NormalColor" &&
                SkinStorage.GetVisualStyle(this) != "Royale.NormalColor" &&
                SkinStorage.GetVisualStyle(this) != "Zune.NormalColor" &&
                SkinStorage.GetVisualStyle(this) != "WMPClassic" &&
                SkinStorage.GetVisualStyle(this) != "ForestGreen" &&
                SkinStorage.GetVisualStyle(this) != "CoolBlue" &&
                SkinStorage.GetVisualStyle(this) != "LawnGreen" &&
                SkinStorage.GetVisualStyle(this) != "OrangeRed" &&
                SkinStorage.GetVisualStyle(this) != "SyncOrange" &&
                SkinStorage.GetVisualStyle(this) != "ChocolateYellow" &&
                SkinStorage.GetVisualStyle(this) != "SpringGreen" &&
                SkinStorage.GetVisualStyle(this) != "BrightGray" &&
                SkinStorage.GetVisualStyle(this) != "BlueWave" &&
                SkinStorage.GetVisualStyle(this) != "ShinyRed" &&
                SkinStorage.GetVisualStyle(this) != "ShinyBlue" &&
                SkinStorage.GetVisualStyle(this) != "VS2010" &&
                SkinStorage.GetVisualStyle(this) != "Office2010Blue" &&
                SkinStorage.GetVisualStyle(this) != "Office2010Black" &&
                SkinStorage.GetVisualStyle(this) != "Office2010Silver" &&
                SkinStorage.GetVisualStyle(this) != "Metro" &&
                SkinStorage.GetVisualStyle(this) != "Transparent" &&
                SkinStorage.GetVisualStyle(this) != "MixedGreen")
            {
                SkinStorage.SetVisualStyle(this, "Default");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            window_hwnd = new WindowInteropHelper(this).Handle;
            if (window_hwnd != IntPtr.Zero)
            {
                this.m_isInitialized = true;
                HwndSource.FromHwnd(window_hwnd).AddHook(new HwndSourceHook(this.HookMethod));
                this.UpdateGlassChange();
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.WindowStyle != WindowStyle.None && this.AllowsTransparency == true && !UseNativeChrome)
            {
                try
                {
                    this.WindowStyle = WindowStyle.None;
                }
                catch (Exception)
                {
                    throw new Exception("Window Style.None is the only valid value when Allowstransparency is set to true");
                }
            }
            base.OnPropertyChanged(e);

            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                ResourceDictionary rd = new ResourceDictionary();

                if (SkinStorage.GetVisualStyle(this) == "Blend")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["BlendChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2007BlueChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2007BlackChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2007SilverChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2003")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2003ChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["ShinyRedChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["ShinyBlueChromelessWindowStyle"] as Style;
                }

                else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["SyncOrangeChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "VS2010")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["VS2010ChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2010BlueChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2010BlackChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2010SilverChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Metro")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["MetroChromelessWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Transparent")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["TransparentChromelessWindowStyle"] as Style;
                }
                else
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/Controls/ChromelessWindow/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["DefaultChromelessWindowStyle"] as Style;
                }
            }
        }

        /// <summary>
        /// Override this method to arrange and size a window and its child elements.
        /// </summary>
        /// <param name="arrangeBounds">A <see cref="T:System.Windows.Size"/> that reflects the final size that the window should use to arrange itself and its children.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> that reflects the actual size that was used.
        /// </returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeBounds)
        {
            if (!UseNativeChrome)
            {
                if (CanEnableGlass)
                {
                    double dpi = this.DPIOffset;
                    int height = 0;
                    if ((dpi >= 1.0) && (dpi < 1.3))
                    {
                        height = 2;
                    }
                    else if (dpi >= 1.0)
                    {
                        height = 4;
                    }

                    if (dpi >= 1.0)
                    {
                        dpi = 1.0 - Math.Abs((double)(dpi - 1.0));
                    }

                    arrangeBounds.Height = (double)(arrangeBounds.Height + (Math.Ceiling((double)(28 * dpi)) + height));
                }
                if ((WindowState == WindowState.Maximized && updateNew) || (this.SizeToContent != SizeToContent.Manual && WindowState == WindowState.Maximized))
                {
                    arrangeBounds = new Size(MaxWidth, MaxHeight);
                }
            }
            else
            {
                if (this.SizeToContent != SizeToContent.Manual && WindowState == WindowState.Maximized)
                {
                    arrangeBounds = new Size(MaxWidth, MaxHeight);
                }
            }
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// Override this method to measure the size of a window.
        /// </summary>
        /// <param name="availableSize">A <see cref="T:System.Windows.Size"/> that reflects the available size that this window can give to the child. Infinity can be given as a value to indicate that the window will size to whatever content is available.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> that reflects the size that this window determines it needs during layout, based on its calculations of children's sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if ((WindowState == WindowState.Maximized && updateNew && !UseNativeChrome) || (this.SizeToContent != SizeToContent.Manual && this.WindowState == WindowState.Maximized))
            {
                availableSize = new Size(MaxWidth, MaxHeight);
            }
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source is Window)
            {
                SendSizingMessage(e.GetPosition(this), e);
            }
            else if ((e.Source as FrameworkElement) != null && (e.Source as FrameworkElement).GetType().BaseType == typeof(ChromelessWindow))
            {
                SendSizingMessage(e.GetPosition(this), e);
            }
            else if ((e.Source as FrameworkElement) != null && (e.Source as FrameworkElement).GetType() != typeof(Window))
            {
                if ((e.Source as FrameworkElement).TemplatedParent != null && (e.Source as FrameworkElement).TemplatedParent.GetType().BaseType == typeof(ChromelessWindow))
                {
                    SendSizingMessage(e.GetPosition(this), e);
                }
            }
            
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Sends the sizing message.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void SendSizingMessage(Point point, MouseButtonEventArgs e)
        {
            ChromelessWindowInterop.SizingDirection direction = GetSizingDirection(point);

            if (direction != ChromelessWindowInterop.SizingDirection.None)
            {
                SendSizingMessage(direction);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            System.Windows.Input.Cursor cursor = null;
            
            if ((e.LeftButton != MouseButtonState.Pressed) && (e.RightButton == MouseButtonState.Released))
            {
                Point position = e.GetPosition(this);
                ChromelessWindowInterop.SizingDirection command = this.GetSizingDirection(position);
                if (command == ChromelessWindowInterop.SizingDirection.None)
                {
                    if (base.Cursor != null)
                    {
                        base.Cursor = null;
                    }
                }
                else
                {
                    switch (command)
                    {
                        case ChromelessWindowInterop.SizingDirection.West:
                        case ChromelessWindowInterop.SizingDirection.East:
                            cursor = System.Windows.Input.Cursors.SizeWE;
                            break;

                        case ChromelessWindowInterop.SizingDirection.North:
                        case ChromelessWindowInterop.SizingDirection.South:
                            cursor = System.Windows.Input.Cursors.SizeNS;
                            break;

                        case ChromelessWindowInterop.SizingDirection.NorthWest:
                            cursor = System.Windows.Input.Cursors.SizeNWSE;
                            break;

                        case ChromelessWindowInterop.SizingDirection.NorthEast:
                            cursor = System.Windows.Input.Cursors.SizeNESW;
                            break;

                        case ChromelessWindowInterop.SizingDirection.SouthWest:
                            cursor = System.Windows.Input.Cursors.SizeNESW;
                            break;

                        case ChromelessWindowInterop.SizingDirection.SouthEast:
                            cursor = System.Windows.Input.Cursors.SizeNWSE;
                            break;
                    }

                    if (cursor != null)
                    {
                        base.Cursor = cursor;
                    }
                }
            }

            if (this.m_isSizing)
            {
                e.Handled = true;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                base.Cursor = null;
            }
            
            return;
        }

        #endregion Implementation

        #region P/Invoke and helper method

        /// <summary>
        /// Hooks the method.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the value</returns>
        private IntPtr HookMethod(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WindowsMessages)msg)
            {
                case WindowsMessages.WM_SIZE:
                    {
                        return this.HandleWM_SIZE(hWnd, msg, wParam, lParam, ref handled);
                    }

                case WindowsMessages.WM_EXITSIZEMOVE:
                    {
                        return this.HandleWM_EXITSIZEMOVE(hWnd, msg, wParam, lParam, ref handled);
                    }

                case WindowsMessages.WM_ENTERSIZEMOVE:
                    {
                        return this.HandleWM_ENTERSIZEMOVE(hWnd, msg, wParam, lParam, ref handled);
                    }

                case WindowsMessages.WM_NCHITTEST:
                    {
                        return this.HandleWM_NCHITTEST(hWnd, msg, wParam, lParam, ref handled);
                    }

                case WindowsMessages.WM_NCCALCSIZE:
                    {
                        return this.HandleWM_NCCALCSIZE(hWnd, msg, wParam, lParam, ref handled);
                    }

                case WindowsMessages.WM_GETMINMAXINFO:
                    {
                        return this.HandleWM_GETMINMAXINFO(hWnd, msg, wParam, lParam, ref handled);
                    }

                case WindowsMessages.WM_DWMCOMPOSITIONCHANGED:
                    {
                        return this.HandleWM_DWMCOMPOSITIONCHANGED(hWnd, msg, wParam, lParam, ref handled);
                    }

                default:
                    {
                        return IntPtr.Zero;
                    }
            }
        }

        /// <summary>
        /// Handles the W m_ SIZE.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the size</returns>
        private IntPtr HandleWM_SIZE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int value = wParam.ToInt32();

            // #define SIZE_RESTORED       0
            // #define SIZE_MAXIMIZED      2
            if (value == 0 || value == 2)
            {
                UpdateWindowRegion(true);
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ EXITSIZEMOVE.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the size</returns>
        private IntPtr HandleWM_EXITSIZEMOVE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            m_isSizing = false;
            UpdateWindowRegion(true);
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ ENTERSIZEMOVE.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the size</returns>
        private IntPtr HandleWM_ENTERSIZEMOVE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            m_isSizing = true;
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ GETMINMAXINFO.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the size</returns>
        private IntPtr HandleWM_GETMINMAXINFO(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (!this.CanEnableGlass && !this.HideTaskBar)
            {
                IntPtr hMonitor = ChromelessWindowInterop.MonitorFromWindow(hWnd, ChromelessWindowInterop.MONITOR_DEFAULTTONEAREST);
                ChromelessWindowInterop.MINMAXINFO minmaxinfo = (ChromelessWindowInterop.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(ChromelessWindowInterop.MINMAXINFO));
                bool flag = false;
                if (hMonitor != IntPtr.Zero)
                {
                    ChromelessWindowInterop.MONITORINFO monitorInfo = new ChromelessWindowInterop.MONITORINFO();
                    ChromelessWindowInterop.GetMonitorInfo(hMonitor, monitorInfo);
                    ChromelessWindowInterop.RECT rcWork = monitorInfo.rcWork;
                    ChromelessWindowInterop.RECT rcMonitor = monitorInfo.rcMonitor;
                    if (Environment.OSVersion.Platform.ToString() == "Win32NT" && Environment.OSVersion.Version.Major.ToString() == "6" && Environment.OSVersion.Version.Minor.ToString() == "1")
                    {
                        minmaxinfo.ptMaxPosition.x = Math.Abs((int)(rcWork.left - rcMonitor.left));
                        if (UseNativeChrome && minmaxinfo.ptMaxPosition.x > 0)
                            minmaxinfo.ptMaxPosition.x = minmaxinfo.ptMaxPosition.x - (int)(rcWork.left);
                    }
                    else
                    {
                        minmaxinfo.ptMaxPosition.x = Math.Abs((int)(rcWork.left - rcMonitor.left));
                    }
                    minmaxinfo.ptMaxPosition.y = (int)(Math.Abs((int)(rcWork.top - rcMonitor.top)) - 3);
                    minmaxinfo.ptMaxSize.x = Math.Abs((int)(rcWork.right - rcWork.left));
                    minmaxinfo.ptMaxSize.y = (int)(Math.Abs((int)(rcWork.bottom - rcWork.top)) + ((rcMonitor.Height == rcWork.Height) ? 2 : 5));

                    if (rcMonitor.Location.X < 0 || rcMonitor.Location.Y < 0)
                    {
                        flag = true;
                    }
                }

                if ((this.MaxHeight == double.PositiveInfinity || this.MaxWidth == double.PositiveInfinity || flag
                    || this.MaxHeight != minmaxinfo.ptMaxSize.y || this.MaxWidth != minmaxinfo.ptMaxSize.x)
                    )
                {
                    if ((this.MaxHeight == double.PositiveInfinity || this.MaxHeight != minmaxinfo.ptMaxSize.y) && !iscustommaxhghtused)
                        this.MaxHeight = minmaxinfo.ptMaxSize.y;
                    if ((this.MaxWidth == double.PositiveInfinity || this.MaxWidth != minmaxinfo.ptMaxSize.x) && !iscustommaxwidthused)
                        this.MaxWidth = minmaxinfo.ptMaxSize.x;
                    flag = false;
                }

                minmaxinfo.ptMinTrackSize = new ChromelessWindowInterop.POINT((this.MinWidth > 0.0) ? ((int)this.MinWidth) : ((int)160.0), (this.MinHeight > 0.0) ? ((int)this.MinHeight) : ((int)38.0));
                minmaxinfo.ptMaxTrackSize = new ChromelessWindowInterop.POINT((int)this.MaxWidth, (int)this.MaxHeight);
                Marshal.StructureToPtr(minmaxinfo, lParam, true);
                handled = true;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ DWMCOMPOSITIONCHANGED.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the position</returns>
        private IntPtr HandleWM_DWMCOMPOSITIONCHANGED(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            this.m_dwmEnabled = ChromelessWindowInterop.CanEnableDwm();

            this.UpdateGlassChange();

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ NCCALCSIZE.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the Size</returns>
        private IntPtr HandleWM_NCCALCSIZE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr zero = IntPtr.Zero;

            if (this.CanEnableGlass)
            {
                if (wParam == IntPtr.Zero)
                {
                    ChromelessWindowInterop.RECT rect = (ChromelessWindowInterop.RECT)Marshal.PtrToStructure(lParam, typeof(ChromelessWindowInterop.RECT));
                    Marshal.StructureToPtr(ChromelessWindowInterop.RECT.FromRectangle(this.CalculateClientRectangle(rect.ToRectangle())), lParam, false);
                    zero = IntPtr.Zero;
                }
                else
                {
                    ChromelessWindowInterop.NCCALCSIZE_PARAMS nccalcsize_params = (ChromelessWindowInterop.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(ChromelessWindowInterop.NCCALCSIZE_PARAMS));
                    ChromelessWindowInterop.WINDOWPOS windowpos = (ChromelessWindowInterop.WINDOWPOS)Marshal.PtrToStructure(nccalcsize_params.lppos, typeof(ChromelessWindowInterop.WINDOWPOS));
                    ChromelessWindowInterop.RECT rect3 = ChromelessWindowInterop.RECT.FromRectangle(this.CalculateClientRectangle(new Rect((double)windowpos.x, (double)windowpos.y, (double)windowpos.cx, (double)windowpos.cy)));
                    nccalcsize_params.rgrc0 = rect3;
                    nccalcsize_params.rgrc1 = rect3;
                    Marshal.StructureToPtr(nccalcsize_params, lParam, false);
                    zero = (IntPtr)new IntPtr(0x400);
                }
            }

            handled = true;
            return zero;
        }

        /// <summary>
        /// Handles the W m_ NCHITTEST.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG value.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Return the Point</returns>
        private IntPtr HandleWM_NCHITTEST(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (!this.CanEnableGlass)
            {
                return IntPtr.Zero;
            }

            int plResult = 0;
            IntPtr zero = IntPtr.Zero;
            int x = ChromelessWindowInterop.GetX(lParam);
            int y = ChromelessWindowInterop.GetY(lParam);

            base.PointFromScreen(new Point((double)x, (double)y));

            ChromelessWindowInterop.DwmDefWindowProc(hWnd, (int)WindowsMessages.WM_NCHITTEST, wParam, lParam, out zero);

            plResult = zero.ToInt32();
            handled = true;

            if (plResult == 20 || plResult == 8 || plResult == 9 || plResult == 21)
            {
                return zero;
            }
            else
            {
                if (this.ResizeMode != ResizeMode.NoResize)
                {
                    handled = false;
                }

                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Updates the window region.
        /// </summary>
        /// <param name="bRedraw">if set to <c>true</c> [b redraw].</param>
        private void UpdateWindowRegion(bool bRedraw)
        {
            if (!this.CanEnableGlass)
            {
                IntPtr hWnd = new WindowInteropHelper(this).Handle;

                int width = (int)(((int)this.Width) + 1);
                int height = (int)(((int)this.Height) + 1);

                if (hWnd != IntPtr.Zero)
                {
                    ChromelessWindowInterop.RECT r = new ChromelessWindowInterop.RECT();
                    if (WindowState == WindowState.Maximized)
                    {
                        IntPtr hMonitor = ChromelessWindowInterop.MonitorFromWindow(hWnd, ChromelessWindowInterop.MONITOR_DEFAULTTONEAREST);
                        ChromelessWindowInterop.MONITORINFO monitorInfo = new ChromelessWindowInterop.MONITORINFO();
                        ChromelessWindowInterop.GetMonitorInfo(hMonitor, monitorInfo);
                        r = monitorInfo.rcWork;
                    }
                    else
                        ChromelessWindowInterop.GetWindowRect(hWnd, ref r);
                    width = (int)(r.Width + 1);
                    height = (int)(r.Height + 1);

                    if (this.SizeToContent == SizeToContent.WidthAndHeight && !UseNativeChrome)
                    {
                        if (this.WindowState == WindowState.Maximized)
                        {
                            InvalidateMeasure();
                        }

                        if (this.WindowState == WindowState.Normal)
                        {
                            InvalidateMeasure();
                        }
                    }
                }

                this.DefineWindowRegion(bRedraw, width, height);
            }
        }

        /// <summary>
        /// Offsets the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Return the offset</returns>
        private static int Offset(int value)
        {
            return (value << 0x10) >> 0x10;
        }

        /// <summary>
        /// Defines the window region.
        /// </summary>
        /// <param name="bRedraw">if set to <c>true</c> [b redraw].</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void DefineWindowRegion(bool bRedraw, int width, int height)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                IntPtr hWnd = new WindowInteropHelper(this).Handle;
                IntPtr zero = IntPtr.Zero;

                if (this.WindowState != WindowState.Maximized)
                {
                    zero = ChromelessWindowInterop.CreateRoundRectRgn(0, 0, (int)width, (int)height, 0, 0);
                }

                ChromelessWindowInterop.SetWindowRgn(hWnd, zero, bRedraw);
            }
        }

        /// <summary>
        /// Updates the glass change.
        /// </summary>
        private void UpdateGlassChange()
        {
            if (this.m_isInitialized)
            {
                int windowLong = 0;
                if (!this.CanEnableGlass)
                {
                    if (this.WindowStyle != WindowStyle.None)
                    {
                        this.WindowStyle = WindowStyle.None;
                    }
                    if (!UseNativeChrome)
                    {
                        IntPtr hWnd = new WindowInteropHelper(this).Handle;
                        HwndSource.FromHwnd(hWnd).CompositionTarget.BackgroundColor = Colors.Transparent;
                        windowLong = ChromelessWindowInterop.GetWindowLong(hWnd, -16);
                        windowLong = (int)(windowLong & ~(windowLong & 0x40000));
                        ChromelessWindowInterop.SetWindowLong(hWnd, -16, windowLong);
                        ChromelessWindowInterop.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, 0x27);
                    }
#if SyncfusionFramework3_5
                    if(UseNativeChrome)
                    {
                        IntPtr hWnd = new WindowInteropHelper(this).Handle;
                        windowLong = ChromelessWindowInterop.GetWindowLong(hWnd, -16);
                        windowLong = (int)(windowLong & ~(windowLong & 0x40000));
                        ChromelessWindowInterop.SetWindowLong(hWnd, -16, windowLong);
                    }
#endif
                    UpdateWindowRegion(false);
                    this.IsGlassActive = false;
                }
                else
                {
                    IntPtr ptr = new WindowInteropHelper(this).Handle;
                    ChromelessWindowInterop.SetWindowRgn(ptr, IntPtr.Zero, false);
                    int dwNewLong = ChromelessWindowInterop.GetWindowLong(ptr, -16);
                    if ((dwNewLong & 0x40000) == 0)
                    {
                        dwNewLong = (int)(dwNewLong | 0x40000);
                        ChromelessWindowInterop.SetWindowLong(ptr, -16, dwNewLong);
                        ChromelessWindowInterop.SetWindowPos(ptr, IntPtr.Zero, 0, 0, 0, 0, 0x27);
                    }

                    if (this.WindowStyle == WindowStyle.None)
                    {
                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                    }

                    this.ExtendWindow();
                    this.IsGlassActive = true;
                }
            }
        }

        
        /// <summary>
        /// Extends the window.
        /// </summary>
        private void ExtendWindow()
        {
            if (!this.CanEnableGlass || !this.m_isInitialized)
            {
                return;
            }

            double dpi = this.DPIOffset;
            int num = (int)(((int)Math.Ceiling((double)(29 * dpi))) - 1);

            IntPtr ptr = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(ptr).CompositionTarget.BackgroundColor = Colors.Transparent;
            ChromelessWindowInterop.ExtendWindow(ptr, num);
        }

        /// <summary>
        /// Calculates the client rectangle.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <returns>Return the Rectangle</returns>
        private Rect CalculateClientRectangle(Rect rect)
        {
            int width = SystemInformation.FrameBorderSize.Width * SystemInformation.BorderMultiplierFactor;
            int height = SystemInformation.FrameBorderSize.Height * SystemInformation.BorderMultiplierFactor;
            double dpi = this.DPIOffset;

            if (dpi >= 1.5)
            {
                dpi = 1.0 - Math.Abs(dpi - 1.0);
                width = (int)dpi * width;
                height = (int)dpi * height;
            }

            rect.X = rect.X + width;
            rect.Width = rect.Width - (width * 2);
            rect.Height = rect.Height - height;

            return rect;
        }

        /// <summary>
        /// Gets the sizing direction.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Return the sizing direction</returns>
        private ChromelessWindowInterop.SizingDirection GetSizingDirection(System.Windows.Point point)
        {
            if (this.ResizeMode == ResizeMode.NoResize || WindowState != WindowState.Normal)
            {
                return ChromelessWindowInterop.SizingDirection.None;
            }

            Size size = new Size(this.ActualWidth, this.ActualHeight);
            if (ChromelessWindowInterop.RECT.GetExtendedRect(new Rect(0.0, 0.0, size.Width, size.Height), ResizeBorderThickness, Padding, Margin).Contains(point))
            {
                if (ResizeGrip != null && ResizeMode == ResizeMode.CanResizeWithGrip && ResizeGrip.IsMouseOver)
                {
                    return ChromelessWindowInterop.SizingDirection.SouthEast;
                }
                else
                {
                    return ChromelessWindowInterop.SizingDirection.None;
                }
            }

            if (CornerRadius.BottomLeft == 0 && CornerRadius.BottomRight == 0 && CornerRadius.TopLeft == 0 && CornerRadius.TopRight == 0)
            {
                cornerRadius = new CornerRadius(5, 5, 5, 5);
            }
            else
            {
                cornerRadius = CornerRadius;
            }

            if ((point.Y > (ResizeBorderThickness.Top + Padding.Top + Margin.Top)) || ((point.Y < 0.0) || (point.X > (CornerRadius.TopLeft + Padding.Top + Padding.Left + Margin.Left))) || (point.X < 0.0))
            {
                if ((point.Y <= (cornerRadius.TopRight + Padding.Top + Padding.Right + Margin.Top)) && (point.X >= (size.Width - (cornerRadius.TopRight + Padding.Top + Padding.Right + Margin.Left + Margin.Right))))
                {
                    return ChromelessWindowInterop.SizingDirection.NorthEast;
                }

                if (((point.Y >= (size.Height - (cornerRadius.BottomLeft + Padding.Bottom + Padding.Left + Margin.Top + Margin.Bottom))) && (point.Y <= size.Height)) && ((point.X <= (cornerRadius.BottomLeft + Padding.Bottom + Padding.Left + Margin.Left)) && (point.X >= 0.0)))
                {
                    return ChromelessWindowInterop.SizingDirection.SouthWest;
                }

                if ((((point.Y >= (size.Height - (cornerRadius.BottomRight + Padding.Bottom + Padding.Right + Margin.Top + Margin.Bottom))) && (point.Y <= size.Height)) && (point.X >= (size.Width - (cornerRadius.BottomRight + Padding.Bottom + Padding.Right + Margin.Left + Margin.Right)))) && (point.X <= size.Width))
                {
                    return ChromelessWindowInterop.SizingDirection.SouthEast;
                }

                if ((point.Y <= (ResizeBorderThickness.Top + Padding.Top + Margin.Top)) && (point.Y >= 0.0))
                {
                    return ChromelessWindowInterop.SizingDirection.North;
                }

                if ((point.X <= (ResizeBorderThickness.Left + Padding.Left) + Margin.Left) && (point.X >= 0.0))
                {
                    return ChromelessWindowInterop.SizingDirection.West;
                }

                if ((point.X >= (size.Width - (ResizeBorderThickness.Right + Padding.Right + Margin.Right + Margin.Left))) && (point.X <= size.Width))
                {
                    return ChromelessWindowInterop.SizingDirection.East;
                }

                if ((point.Y < (size.Height - (ResizeBorderThickness.Bottom + Padding.Bottom + Margin.Top + Margin.Bottom))) || (point.Y > size.Height))
                {
                    return ChromelessWindowInterop.SizingDirection.None;
                }

                return ChromelessWindowInterop.SizingDirection.South;
            }

            return ChromelessWindowInterop.SizingDirection.NorthWest;
        }

        /// <summary>
        /// Sends the sizing message.
        /// </summary>
        /// <param name="sizing">The sizing.</param>
        private void SendSizingMessage(ChromelessWindowInterop.SizingDirection sizing)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                IntPtr hWnd = new WindowInteropHelper(this).Handle;
                ChromelessWindowInterop.SendMessage(hWnd, ChromelessWindowInterop.WM_SYSCOMMAND, (int)(0xf000 + sizing), 0);
                ChromelessWindowInterop.SendMessage(hWnd, ChromelessWindowInterop.WM_LBUTTONUP, 0, 0);
            }
        }

        #endregion P/Invoke and helper method

        private void HandleSizeToContent()
        {
            if (this.SizeToContent == SizeToContent.Manual)
                return;

            var previousSizeToContent = SizeToContent;
            SizeToContent = SizeToContent.Manual;

            Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            (Action)(() =>
            {
                this.SizeToContent = previousSizeToContent;
                this.Visibility = Visibility.Visible;
            }));
        }
    }

    /// <summary>
    /// CornerRadiusConverter class that convert the corner radius
    /// </summary>
    public class CornerRadiusConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CornerRadius c = (CornerRadius)value;
            CornerRadius temp = new CornerRadius();
            if ((string)parameter == "Top")
            {
                temp.TopLeft = c.TopLeft;
                temp.TopRight = c.TopRight;
                temp.BottomLeft = 0;
                temp.BottomRight = 0;
                return temp;
            }
            else
            {
                temp.TopLeft = 0;
                temp.TopRight = 0;
                temp.BottomLeft = c.BottomLeft;
                temp.BottomRight = c.BottomRight;
                return temp;
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }

        #endregion IValueConverter Members
    }

    /// <summary>
    /// True/False values using class.
    /// </summary>
    internal static class BooleanBoxes
    {
        /// <summary>
        /// FalseBox variable.
        /// </summary>
        internal static object FalseBox = false;

        /// <summary>
        /// True box variable.
        /// </summary>
        internal static object TrueBox = true;

        /// <summary>
        /// Boxes the specified value.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>Boolean value.</returns>
        internal static object Box(bool value)
        {
            if (value)
            {
                return TrueBox;
            }

            return FalseBox;
        }
    }
}