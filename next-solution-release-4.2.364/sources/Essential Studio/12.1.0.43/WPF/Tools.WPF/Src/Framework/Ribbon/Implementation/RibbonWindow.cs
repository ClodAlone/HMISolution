// <copyright file="RibbonWindow.cs" company="Syncfusion">
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
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a RibbonWindow control.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class RibbonWindow : Window</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonWindow x:Class="RibbonSample.Window1" x:Name="RibbonWindow"/>]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// RibbonWindow class represents main Ribbon UI element - Window control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonWindow in XAML.
    /// <code><![CDATA[<ribbon:RibbonWindow x:Class="RibbonSample.Window1" x:Name="RibbonWindow"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:ribbon="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// xmlns:shared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF"
    /// xmlns:sample="clr-namespace:RibbonSample"
    /// Title="Ribbon" Width="800" Height="600"/>]]></code>
    /// <para/>This example shows how to create a RibbonWindow in C#.
    /// <code>
    /// using System;
    /// using System.Windows;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace CSharp
    /// {
    /// public partial class CodeOnlyWindow : RibbonWindow
    /// {
    /// public CodeOnlyWindow()
    /// {
    /// this.Title = "Main Window in Code Only";
    /// this.Width = 300;
    /// this.Height = 300;
    /// }
    /// }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
       Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange ,
   Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2013Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Windows8,
Type = typeof(RibbonWindow), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Windows8Style.xaml")]
   
    public class RibbonWindow : Window
    {
        #region Fields
        /// <summary>
        /// Represents the down enabled
        /// </summary>
        private bool m_dwmEnabled;

        /// <summary>
        /// Enable Sizing
        /// </summary>

        /// <summary>
        /// Enable Initialized
        /// </summary>
        private bool m_isInitialized;

        internal int borderWidth;

        internal int borderHeight;

        Point currentWindowPoint;
        internal bool updateNew = false;

        internal string iconUri = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Resources/SysIcon.png";
        
        #endregion

        #region Properties

        WindowChrome windowChrome;

        /// <summary>
        /// Used to store Last applied VisualStyle for Custom Color implementation.
        /// </summary>
        public  string WindowTitleVisualStyle = "default";



        /// <summary>
        /// Gets the titlebar control of the <see cref="RibbonWindow"/>.
        /// </summary>        
        /// <list type="table">
        /// <listheader>
        /// <term>Help Page</term>
        /// <description>Syntax</description>
        /// </listheader>
        /// <example>
        /// <list type="table">
        /// <listheader>
        /// <description>C#</description>
        /// </listheader>
        /// <example><code>public TitleBar TitleBar{ get; }</code></example>
        /// </list>
        /// <para/>
        /// <list type="table">
        /// <listheader>
        /// <description>XAML Object Element Usage</description>
        /// </listheader>
        /// <example><code><![CDATA[<ribbon:TitleBar x:Name="title"/>]]></code></example>
        /// </list>
        /// </example>
        /// </list>
        /// <remarks>
        /// Type: <see cref="TitleBar"/>
        /// Instance of TitleBar control used in window.       
        /// </remarks>
        /// <example> 
        /// <para/>This example shows how to use a TitleBar in C#.
        /// <code>    
        /// using System;
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace CSharp
        /// {
        /// public partial class CodeOnlyWindow : RibbonWindow
        /// {
        /// public CodeOnlyWindow()
        /// {
        /// TitleBar title = this.TitleBar;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TitleBar"/>
        /// <seealso cref="RibbonWindow"/>
        public TitleBar TitleBar
        {
            get
            {
                return (TitleBar)base.GetTemplateChild("PART_TitleBar");
            }
        }

        /// <summary>
        /// Gets or sets the RibbonStatusBar control of the <see cref="RibbonWindow"/>.
        /// </summary>
        /// <list type="table">
        /// <listheader>
        /// <term>Help Page</term>
        /// <description>Syntax</description>
        /// </listheader>
        /// <example>
        /// <list type="table">
        /// <listheader>
        /// <description>C#</description>
        /// </listheader>
        /// <example><code>public RibbonStatusBar RibbonStatusBar{ get; set; }</code></example>
        /// </list>
        /// <para/>
        /// <list type="table">
        /// <listheader>
        /// <description>XAML Object Element Usage</description>
        /// </listheader>
        /// <example><code><![CDATA[<ribbon:RibbonStatusBar x:Name="status"/>]]></code></example>
        /// </list>
        /// </example>
        /// </list>
        /// <remarks>
        /// Type: <see cref="RibbonStatusBar"/>
        /// Instance of RibbonStatusBar control used in window.       
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to create a RibbonStatusBar in XAML.
        /// <code><![CDATA[<ribbon:RibbonWindow x:Class="RibbonSample.Window1" x:Name="RibbonWindow"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:ribbon="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// xmlns:shared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF"
        /// xmlns:sample="clr-namespace:RibbonSample"
        /// Title="Ribbon" Width="800" Height="600">
        /// <ribbon:RibbonWindow.StatusBar>
        /// <ribbon:RibbonStatusBar x:Name="status"/>
        /// </ribbon:RibbonWindow.StatusBar>]]></code>
        /// <para/>This example shows how to create a TitleBar in C#.
        /// <code>    
        /// using System;
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace CSharp
        /// {
        /// public partial class CodeOnlyWindow : RibbonWindow
        /// {
        /// public CodeOnlyWindow()
        /// {
        /// RibbonStatusBar status = new RibbonStatusBar();
        /// this.StatusBar = status;             
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="RibbonStatusBar"/>
        /// <seealso cref="RibbonWindow"/>
        public RibbonStatusBar StatusBar
        {
            get
            {
                return (RibbonStatusBar)GetValue(StatusBarProperty);
            }

            set
            {
                SetValue(StatusBarProperty, value);
            }
        }

        /// <summary>
        /// Gets the resize grip.
        /// </summary>
        /// <value>The resize grip.</value>
        private System.Windows.Controls.Label ResizeGrip
        {
            get
            {
                return (System.Windows.Controls.Label)base.GetTemplateChild("PART_ResizeGrip");
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets a value indicating whether minimal size of window is reached.
        /// </summary>
        internal bool IsMinimalSizeReached
        {
            get
            {
                return (bool)GetValue(IsMinimalSizeReachedProperty);
            }

            set
            {
                SetValue(IsMinimalSizeReachedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is glass active.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is glass active; otherwise, <c>false</c>.
        /// </value>
        public bool IsGlassActive
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
        /// Gets or sets a window's icon.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Windows.Media.ImageSource"/> object that represents the icon.</returns>
        public ImageSource  Office2010Icon
        {
            get { return (ImageSource)GetValue(Office2010IconProperty); }
            set { SetValue(Office2010IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Gets or Sets the Office 2010 window Icon. It is a dependency property.
        /// </summary>
        public static readonly DependencyProperty Office2010IconProperty = 
            DependencyProperty.Register("Office2010Icon", typeof(ImageSource), typeof(RibbonWindow), new PropertyMetadata(null));

        
        /// <summary>
        /// Gets or sets the maximized mode.
        /// </summary>
        /// <value>The maximized mode.</value>
        public MaximizedMode MaximizedMode
        {
            get
            {
                return (MaximizedMode)GetValue(MaximizedModeProperty);
            }

            set
            {
                SetValue(MaximizedModeProperty, value);
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
                Point point = WindowInterop.GetTransformedPoint(this);
                if (point.Y != 96.0)
                {
                    num = (double)(num + ((point.Y - 96.0) / 96.0));
                }

                return num;
            }
        }

        /// <summary>
        /// Gets or sets the ribbon status bar style.
        /// </summary>
        /// <value>The ribbon status bar style.</value>
        public Style RibbonStatusBarStyle
        {
            get { return (Style)GetValue(RibbonStatusBarStyleProperty); }
            set { SetValue(RibbonStatusBarStyleProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [save original state].
        /// </summary>
        /// <value><c>true</c> if [save original state]; otherwise, <c>false</c>.</value>
        [Description("Indicates whether to save state persisted on loading.")]
        public bool AutoPersist
        {
            get { return (bool)GetValue(AutoPersistProperty); }
            set { SetValue(AutoPersistProperty, value); }
        }

        /// <summary>
        /// Gets or sets the back stage.
        /// </summary>
        /// <value>The back stage.</value>
        public Backstage BackStage
        {
            get { return (Backstage)GetValue(BackStageProperty); }
            set { SetValue(BackStageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageProperty =
            DependencyProperty.Register("BackStage", typeof(Backstage), typeof(RibbonWindow), new FrameworkPropertyMetadata(null));

        public bool ShowHelpButton
        {
            get { return (bool)GetValue(ShowHelpButtonProperty); }
            set { SetValue(ShowHelpButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHelpButtonProperty =
            DependencyProperty.Register("ShowHelpButton", typeof(bool), typeof(RibbonWindow), new FrameworkPropertyMetadata(false));     

        #endregion

        #region Dependency Properties


        /// <summary>
        /// Identifies the <see cref="AutoPersist"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoPersistProperty =
            DependencyProperty.Register("AutoPersist", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(false));


        // Using a DependencyProperty as the backing store for RibbonStatusBarStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RibbonStatusBarStyleProperty =
            DependencyProperty.Register("RibbonStatusBarStyle", typeof(Style), typeof(RibbonWindow), new PropertyMetadata(null, OnRibbonStatusBarStyleChanged));

        private static void OnRibbonStatusBarStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var rbnwdw = sender as RibbonWindow;
            if (rbnwdw == null) return;
            rbnwdw.SetRibbonStatusBarStyle();
        }

        /// <summary>
        /// Sets the ribbon status bar style.
        /// </summary>
        private void SetRibbonStatusBarStyle()
        {
            if (this.StatusBar != null && this.RibbonStatusBarStyle != null)
            {
                this.StatusBar.Style = this.RibbonStatusBarStyle;
            }
        }

        /// <summary>
        /// Defines whether minimal size of window is reached.
        /// </summary>
        internal static readonly DependencyProperty IsMinimalSizeReachedProperty =
            DependencyProperty.Register("IsMinimalSizeReached", typeof(bool), typeof(RibbonWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsMinimalSizeReachedChanged)));

        /// <summary>
        /// Gets or sets the StatusBar control of the window. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusBarProperty =
            DependencyProperty.Register("StatusBar", typeof(RibbonStatusBar), typeof(RibbonWindow), new UIPropertyMetadata(null, OnRibbonStatusBarStyleChanged));

        /// <summary>
        /// Defines whether Glass effects are active. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGlassActiveProperty =
            DependencyProperty.Register("IsGlassActive", typeof(bool), typeof(RibbonWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsGlassActiveChanged)));

        /// <summary>
        /// Gets or Sets appearance style of maximized RibbonWindow.
        /// This property have to be used only with OS Version minor then 6(Vista OS). This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizedModeProperty =
            DependencyProperty.Register("MaximizedMode", typeof(MaximizedMode), typeof(RibbonWindow), new FrameworkPropertyMetadata(MaximizedMode.Default, new PropertyChangedCallback(OnMaximizedModeChanged)));

        /// <summary>
        /// Gets or Sets the alignment of the Title Text
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
            DependencyProperty.Register("TitleTextAlignment", typeof(System.Windows.HorizontalAlignment), typeof(RibbonWindow), new FrameworkPropertyMetadata(System.Windows.HorizontalAlignment.Center));
        
        public double MinimumResizeValue
        {
            get { return (double)GetValue(MinimumResizeValueProperty); }
            set { SetValue(MinimumResizeValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumResizeValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumResizeValueProperty =
            DependencyProperty.Register("MinimumResizeValue", typeof(double), typeof(RibbonWindow), new FrameworkPropertyMetadata(300.00,FrameworkPropertyMetadataOptions.AffectsMeasure));



        public Visibility BackStageCornerImageVisibility
        {
            get { return (Visibility)GetValue(BackStageCornerImageVisibilityProperty); }
            set { SetValue(BackStageCornerImageVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageCornerImageVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageCornerImageVisibilityProperty =
            DependencyProperty.Register("BackStageCornerImageVisibility", typeof(Visibility), typeof(RibbonWindow), new UIPropertyMetadata(Visibility.Visible));
              

        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(RibbonWindow), new UIPropertyMetadata(Brushes.Blue));

        public ImageSource TitleBarPatternImage
        {
            get { return (ImageSource)GetValue(TitleBarPatternImageProperty); }
            set { SetValue(TitleBarPatternImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleBarPatternImageProperty =
            DependencyProperty.Register("TitleBarPatternImage", typeof(ImageSource), typeof(RibbonWindow), new UIPropertyMetadata(null));

        
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes static members of the <see cref="RibbonWindow"/> class.
        /// </summary>
        static RibbonWindow()
        {
            //EnvironmentTest.ValidateLicense(typeof(RibbonWindow));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonWindow"/> class.
        /// </summary>
        public RibbonWindow()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(RibbonWindow));
            }
            this.m_dwmEnabled = WindowInterop.CanEnableDwm();
            string skin = SkinStorage.GetVisualStyle(this);
            if (skin == "Default")
            {
                ResourceDictionary rd = new ResourceDictionary();
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                this.Style = rd["Office2007BlueRibbonWindowStyle"] as Style;
            }

            if (!m_dwmEnabled)
            {
                IsGlassActive = false;
            }
        }
        #endregion      

        #region Implementation

        internal void ShowBackStage()
        {
            if (this.BackStageContent != null && this.BackStage != null)
            {
                this.BackStageContent.Visibility = Visibility.Visible;
                if (SkinStorage.GetVisualStyle(this) == "Office2013")
                {
                    Storyboard ScrollVertical = new Storyboard();
                    DoubleAnimationUsingKeyFrames HeightAnimation = new DoubleAnimationUsingKeyFrames();
                    ScrollVertical.Children.Add(HeightAnimation);
                    Storyboard.SetTarget(HeightAnimation, this.BackStageContent);
                    Storyboard.SetTargetProperty(HeightAnimation, new System.Windows.PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                    SplineDoubleKeyFrame DoubleKeyFrame1 = new SplineDoubleKeyFrame();
                    DoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    DoubleKeyFrame1.Value = 0;
                    HeightAnimation.KeyFrames.Add(DoubleKeyFrame1);
                    SplineDoubleKeyFrame DoubleKeyFrame2 = new SplineDoubleKeyFrame();
                    DoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2));
                    DoubleKeyFrame2.Value = 1;
                    HeightAnimation.KeyFrames.Add(DoubleKeyFrame2);
                    ScrollVertical.Begin();
                }
                this.BackStageContent.Focus();
            }
        }

        internal void HideBackStage()
        {
            if (this.BackStageContent != null && this.BackStage != null)
                this.BackStageContent.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Calls OnIsGlassActiveChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsGlassActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow instance = (RibbonWindow)d;
            instance.OnIsGlassActiveChanged(e);
        }

        /// <summary>
        /// Raises IsGlassActiveChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsGlassActiveChanged(DependencyPropertyChangedEventArgs e)
        {
            //UpdateGlassChange();

            if (IsGlassActiveChanged != null)
            {
                IsGlassActiveChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMaximizedModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMaximizedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow instance = (RibbonWindow)d;
            instance.OnMaximizedModeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MaximizedModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMaximizedModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!CanEnableGlass && WindowStyle == WindowStyle.None)
            {
                UpdateWindowBounds();
            }
        }

        /// <summary>
        /// Updates the window bounds.
        /// </summary>
        private void UpdateWindowBounds()
        {
            if (WindowState == WindowState.Maximized)
            {
                HwndSource src = (HwndSource)PresentationSource.FromVisual(this);

                if (src != null)
                {
                    IntPtr hwnd = src.Handle;
                    WindowInterop.RECT r = GetMaxWindowBounds(hwnd);

                    if (currentWindowPoint.X < SystemInformation.PrimaryMonitorSize.Width)
                        NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_NOTOPMOST, r.left, r.top, r.Width, r.Height, NativeConstants.SWP_NOACTIVATE | NativeConstants.SWP_FRAMECHANGED);
                    else if (currentWindowPoint.X >= SystemInformation.PrimaryMonitorSize.Width)
                        NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_NOTOPMOST, r.left + borderWidth, r.top, r.Width, r.Height, NativeConstants.SWP_NOACTIVATE | NativeConstants.SWP_FRAMECHANGED);
                    else
                        NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_NOTOPMOST, r.left, r.top, r.Width, r.Height, NativeConstants.SWP_NOACTIVATE | NativeConstants.SWP_FRAMECHANGED);
                }
            }
        }

        /// <summary>
        /// Gets the max window bounds.
        /// </summary>
        /// <param name="ptr">The PTR Param value.</param>
        /// <returns>Maximized window bounds</returns>
        private WindowInterop.RECT GetMaxWindowBounds(IntPtr ptr)
        {
            int x = 0, y = 0, cx = 0, cy = 0;

            IntPtr hMonitor = WindowInterop.MonitorFromWindow(ptr, WindowInterop.MONITOR_DEFAULTTONEAREST);
            if (hMonitor != IntPtr.Zero)
            {
                WindowInterop.MONITORINFO monitorInfo = new WindowInterop.MONITORINFO();
                WindowInterop.GetMonitorInfo(hMonitor, monitorInfo);
                WindowInterop.RECT rcWork = monitorInfo.rcWork;
                WindowInterop.RECT rcMonitor = monitorInfo.rcMonitor;

                bool isKioskMode = MaximizedMode == MaximizedMode.KioskStyle && WindowStyle == WindowStyle.None;
                borderWidth = SystemInformation.FrameBorderSize.Width * SystemInformation.BorderMultiplierFactor;
                borderHeight = SystemInformation.FrameBorderSize.Height * SystemInformation.BorderMultiplierFactor;

                if (CanEnableGlass && WindowStyle == WindowStyle.None)
                {
                    if (WindowStyle == WindowStyle.None)
                    {
                        borderWidth = 0;
                        borderHeight = 0;                       
                    }

                    x = rcMonitor.left - borderWidth;
                    y = rcMonitor.top - borderHeight;

                    cx = rcMonitor.Width + borderWidth * 2;
                    cy = rcMonitor.Height + borderHeight * 2;
                }
                else if (!CanEnableGlass && isKioskMode)
                {
                    x = rcMonitor.left;
                    y = rcMonitor.top;

                    cx = rcMonitor.Width;
                    cy = rcMonitor.Height;
                }
                else
                {
                    //x = Math.Abs((int)(rcWork.left - rcMonitor.left));
                    if (rcMonitor.left == rcWork.left)
                        x = 0;
                    else
                        x = rcMonitor.left;
                    //y = (int)(Math.Abs((int)(rcWork.top - rcMonitor.top)) - 3);
                    y = rcMonitor.top;

                    cx = Math.Abs((int)(rcWork.right - rcWork.left));
                    cy = (int)(Math.Abs((int)(rcWork.bottom - rcWork.top)) + ((rcMonitor.Height == rcWork.Height) ? -1 : 5));

                    if (CanEnableGlass &&
                        WindowStyle == WindowStyle.ThreeDBorderWindow)
                    {
                        cx += SystemInformation.Border3DSize.Width;
                        cy += SystemInformation.Border3DSize.Height;
                    }
                }
            }

            if (!double.IsInfinity(MaxWidth) && !double.IsNaN(MaxWidth) && MaxWidth != 0 &&
                cx > (int)MaxWidth)
            {
                cx = (int)MaxWidth;
            }

            if (!double.IsInfinity(MaxHeight) && !double.IsNaN(MaxHeight) &&
                cy > (int)MaxHeight)
            {
                cy = (int)MaxHeight;
            }
            if (x + cx == 0)
                return new WindowInterop.RECT(0, y, cx, y + cy);          
            return new WindowInterop.RECT(x, y, x + cx, y + cy);
        }

        /// <summary>
        /// Calls OnIsMinimalSizeRichedChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsMinimalSizeReachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow instance = (RibbonWindow)d;
            instance.OnIsMinimalSizeReachedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// IsMinimalSizeReachedChanged  event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsMinimalSizeReachedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMinimalSizeReachedChanged != null)
            {
                IsMinimalSizeReachedChanged(this, e);
            }
        }
        #endregion

        #region Class Overrides


        internal ContentControl BackStageContent;

        private Grid rootGrid;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.BackStageContent = this.GetTemplateChild("BackStageContent") as ContentControl;
            this.rootGrid = this.GetTemplateChild("rootGrid") as Grid;
            this.SetRibbonStatusBarStyle();
            if (marginSetByChrome != null)
                UpdateRootElementMargin(marginSetByChrome);
            if(ResizeGrip!=null)
                ResizeGrip.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ResizeGrip_PreviewMouseLeftButtonDown);
        }

        void ResizeGrip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            WindowInterop.SizingDirection direction = GetSizingDirection(position);
            
            if (direction == WindowInterop.SizingDirection.SouthEast)
            {
                SendSizingMessage(direction);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            //if ((!e.Handled && (e.ChangedButton == MouseButton.Right)) && TitleBar != null && TitleBar.IsMouseOver)
            //{
            //    e.Handled = true;
            //    IntPtr handle = new WindowInteropHelper(this).Handle;
            //    Point point = base.PointToScreen(e.GetPosition(this));

            //    IntPtr hMenu = WindowInterop.GetSystemMenu(handle, false);
            //    if (WindowState == System.Windows.WindowState.Maximized)
            //    {
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x000);                    
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 1), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x000);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
            //    }
            //    else if (WindowState == System.Windows.WindowState.Minimized)
            //    {
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x000);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 1), 0x001);                    
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x000);
            //    }
            //    else
            //    {
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 1), 0x000);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x000);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x000);                    
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x000);
            //    }
            //    if (ResizeMode == ResizeMode.NoResize)
            //    {
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
            //    }
            //    if (ResizeMode == ResizeMode.CanMinimize)
            //    {
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);                    
            //        WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
            //    }

            //    WindowInterop.ShowSystemMenu(handle, point);
            //}
			if(this.TitleBar != null)
            this.TitleBar.IsMousePressed = false;
        }


        internal void OnClickSysIcon(MouseButtonEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            Point point = base.PointToScreen(e.GetPosition(this));
            point.Y += 10;
            WindowInterop.ShowSystemMenu(handle, point);
        }

         #if !SyncfusionFramework3_5
        internal void OnClickTouchSysIcon(TouchEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            Point point = base.PointToScreen(e.GetTouchPoint(this).Position);
            point.Y += 10;
            WindowInterop.ShowSystemMenu(handle, point);
        }
#endif

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for a panel.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to child
        /// elements. Infinity can be
        /// specified as a value to indicate
        /// that the element will size to
        /// whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if ((availableSize.Width <= MinimumResizeValue || availableSize.Height <= MinimumResizeValue - 50))
            {
                IsMinimalSizeReached = true;
            }
            else if ((availableSize.Width > MinimumResizeValue || availableSize.Height > MinimumResizeValue - 50) && IsMinimalSizeReached)
            {
                IsMinimalSizeReached = false;
            }
            //if (WindowState == WindowState.Maximized && updateNew)
            //{
            //    availableSize = new Size(MaxWidth, MaxHeight);
            //}

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.m_isInitialized = true;

            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource.FromHwnd(helper.Handle).AddHook(new HwndSourceHook(this.HookMethod));
            windowChrome = new WindowChrome() { ResizeBorderThickness = SystemParameters2.Current.WindowResizeBorderThickness };
            windowChrome.MarginChanged += new MarginChangedHandler(windowChrome_MarginChanged);
            this.UpdateGlassChange();
        }

        Thickness marginSetByChrome;

        void windowChrome_MarginChanged(object o, MarginChangedEventArgs e)
        {
            marginSetByChrome = e.Margin;
            Dispatcher.BeginInvoke(new Action(() => UpdateRootElementMargin(marginSetByChrome)), System.Windows.Threading.DispatcherPriority.Loaded);            
        }

        private void UpdateRootElementMargin(Thickness thickness)
        {
            try
            {
                FrameworkElement rootElement = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);
                if (rootElement != null && IsGlassActive)
                {
                    Thickness newMargin = new Thickness(thickness.Left + SystemParameters2.Current.WindowResizeBorderThickness.Left,
                                             thickness.Top,
                                             thickness.Right + SystemParameters2.Current.WindowResizeBorderThickness.Right,
                                            thickness.Bottom + SystemParameters2.Current.WindowResizeBorderThickness.Bottom);
                    if (SkinStorage.GetVisualStyle(this) != "Office2013")
                        rootElement.Margin = newMargin;
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Override this method to arrange and size a window and its child elements.
        /// </summary>
        /// <param name="arrangeBounds">A <see cref="T:System.Windows.Size"/> that reflects the final size that the window should use to arrange itself and its children.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> that reflects the actual size that was used.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            //string version = Environment.Version.ToString();
            //if (CanEnableGlass && !version.StartsWith("4.0"))
            //{
            //    if (this.TitleBar != null)
            //    {
            //        if (this.TitleBar.ActualHeight != 0)
            //        {
            //            arrangeBounds.Height = (double)(arrangeBounds.Height) + this.TitleBar.ActualHeight;
            //            if (this.actualHeight == 0)
            //            {
            //                this.actualHeight = this.TitleBar.ActualHeight;
            //            }
            //            else
            //            {
            //                arrangeBounds.Height = (double)(arrangeBounds.Height) - this.TitleBar.ActualHeight + this.actualHeight;
            //            }
            //        }
            //        else
            //            arrangeBounds.Height = (double)(arrangeBounds.Height) + this.actualHeight;

            //    }
            //}
            //if (WindowState == WindowState.Maximized && updateNew)
            //{
            //    arrangeBounds = new Size(MaxWidth, MaxHeight);
            //}

            return base.ArrangeOverride(arrangeBounds);
        }

        double captionHeight = 0d;

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary rd = new ResourceDictionary();
            if (e.Property == RibbonWindow.BackStageProperty)
            {
                if(BackStage!=null)
                BackStage.FlowDirection = this.FlowDirection;
            }
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                 this.WindowTitleVisualStyle = SkinStorage.GetVisualStyle(this);

                if (SkinStorage.GetVisualStyle(this) == "Blend")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["BlendRibbonWindowStyle"] as Style;
                    captionHeight = SystemParameters2.Current.WindowCaptionHeight;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2007BlueRibbonWindowStyle"] as Style;
                    captionHeight = SystemParameters2.Current.WindowCaptionHeight;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2007BlackRibbonWindowStyle"] as Style;
                    captionHeight = SystemParameters2.Current.WindowCaptionHeight;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2007SilverRibbonWindowStyle"] as Style;
                    captionHeight = SystemParameters2.Current.WindowCaptionHeight;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2003")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2003RibbonWindowStyle"] as Style;
                    captionHeight = SystemParameters2.Current.WindowCaptionHeight;
                }
                else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["ShinyRedRibbonWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["ShinyBlueRibbonWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2010BlackRibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2010BlueRibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2010SilverRibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                else if (SkinStorage.GetVisualStyle(this) == "VS2010")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["VS2010RibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["SyncOrangeRibbonWindowStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Metro")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["MetroRibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Transparent")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["TransparentRibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2013")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2013Style.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2013RibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Windows8")
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Windows8Style.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Windows8RibbonWindowStyle"] as Style;
                    captionHeight = 45d;
                }
                    
                else
                {
                    rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    this.Style = rd["Office2007BlueRibbonWindowStyle"] as Style;
                    captionHeight = SystemParameters2.Current.WindowCaptionHeight;
                }
                if (windowChrome != null)
                    windowChrome.CaptionHeight = captionHeight;
               // ExtendWindow();
                UpdateGlassChange();
            }

            else if (e.Property == SkinManager.ActiveColorSchemeProperty)
            {
                Brush brush;

                if (e.NewValue != null)
                {
                    brush = e.NewValue as Brush;
                    ResourceDictionary newwindow = new ResourceDictionary();
                    if (brush is SolidColorBrush)
                    {
                        Color color = (brush as SolidColorBrush).Color;

                        if (this.WindowTitleVisualStyle.Contains("Office2010"))
                        {
                            var mergd = new ResourceDictionary();
                            mergd.Add("RibbonWindowStyle", rd["RibbonWindowStyle"]);
                            newwindow.MergedDictionaries.Add(mergd);
                            //newwindow.Source = rd.Source;
                            newwindow.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                            newwindow = SkinColorScheme.ApplyCustomColorScheme(newwindow, color);
                            this.Style = newwindow["Office2010BlueRibbonWindowStyle"] as Style;
                        }

                     
                        else
                        {
                            var mergd = new ResourceDictionary();
                            mergd.Add("Office2007BlueRibbonWindowStyle", rd["Office2007BlueRibbonWindowStyle"]);
                            newwindow.MergedDictionaries.Add(mergd);
                            //newwindow.Source = rd.Source;
                            newwindow.Source = new Uri(@"/Syncfusion.Tools.WPF;component//Framework/Ribbon/Themes/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                            newwindow = SkinColorScheme.ApplyCustomColorScheme(newwindow, color);
                            this.Style = newwindow["Office2007BlueRibbonWindowStyle"] as Style;
                        }
                    }
                }              
            }

            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //Point position = e.GetPosition(this);
            //WindowInterop.SizingDirection direction = GetSizingDirection(position);
            //if ((!(e.Source is Ribbon) && !(VisualUtils.FindLogicalAncestor((e.Source as FrameworkElement), typeof(Ribbon)) is Ribbon)))
            //{
            //    if (e.Source as Visual != null)
            //        currentWindowPoint = (e.Source as Visual).PointToScreen(new Point(0, 0));
            //}
            //if (direction != WindowInterop.SizingDirection.None)
            //{
            //    //SendSizingMessage(direction);
            //    //e.Handled = true;
            //    if ((e.Source as FrameworkElement) != null && ((e.Source as FrameworkElement).GetType().BaseType == typeof(RibbonWindow) || (e.Source as FrameworkElement).GetType().BaseType == typeof(System.Windows.Controls.Control) || (e.Source as FrameworkElement).GetType().BaseType == typeof(FrameworkElement)))
            //    {
            //        SendSizingMessage(direction);
            //        e.Handled = true;
            //    }
            //    else if ((e.Source as FrameworkElement) != null && (e.Source as FrameworkElement).GetType() != typeof(Window))
            //    {
            //        if ((e.Source as FrameworkElement).TemplatedParent != null && (e.Source as FrameworkElement).TemplatedParent.GetType().BaseType == typeof(RibbonWindow))
            //        {
            //            SendSizingMessage(direction);
            //            e.Handled = true;
            //        }
            //    }
            //    if ((e.Source as FrameworkElement) != null && (e.Source as FrameworkElement).GetType() == typeof(Ribbon))
            //    {
            //        //if ((e.Source as FrameworkElement).TemplatedParent != null && (e.Source as FrameworkElement).TemplatedParent.GetType().BaseType == typeof(RibbonWindow))
            //        //{
            //            SendSizingMessage(direction);
            //            e.Handled = true;
            //        //}
            //    }
            //}
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            //System.Windows.Input.Cursor cursor = null;
            //if ((e.LeftButton == MouseButtonState.Released) && (e.RightButton == MouseButtonState.Released))
            //{
            //    Point position = e.GetPosition(this);
            //    WindowInterop.SizingDirection command = this.GetSizingDirection(position);
            //    if (command == WindowInterop.SizingDirection.None)
            //    {
            //        if (base.Cursor != null)
            //        {
            //            base.Cursor = null;
            //        }
            //    }
            //    else
            //    {
            //        switch (command)
            //        {
            //            case WindowInterop.SizingDirection.West:
            //            case WindowInterop.SizingDirection.East:
            //                cursor = System.Windows.Input.Cursors.SizeWE;
            //                break;
            //            case WindowInterop.SizingDirection.North:
            //            case WindowInterop.SizingDirection.South:
            //                cursor = System.Windows.Input.Cursors.SizeNS;
            //                break;
            //            case WindowInterop.SizingDirection.NorthWest:
            //                cursor = System.Windows.Input.Cursors.SizeNWSE;
            //                break;
            //            case WindowInterop.SizingDirection.NorthEast:
            //                cursor = System.Windows.Input.Cursors.SizeNESW;
            //                break;
            //            case WindowInterop.SizingDirection.SouthWest:
            //                cursor = System.Windows.Input.Cursors.SizeNESW;
            //                break;
            //            case WindowInterop.SizingDirection.SouthEast:
            //                cursor = System.Windows.Input.Cursors.SizeNWSE;
            //                break;
            //        }

            //        if (cursor != null)
            //        {
            //            base.Cursor = cursor;
            //        }
            //    }
            //}

            //if (this.m_isSizing)
            //{
            //    e.Handled = true;
            //}

            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.StateChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            //UpdateWindowRegion(true);

            //if (WindowState != WindowState.Minimized)
            //{
            //    ExtendWindow();
            //}
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when IsMinimalSizeRiched property is
        /// changed.
        /// </summary>
        internal event PropertyChangedCallback IsMinimalSizeReachedChanged;

        /// <summary>
        /// Event that is raised when IsGlassActive property is changed.
        /// </summary>
        public event PropertyChangedCallback IsGlassActiveChanged;
        #endregion

        #region P/Invoke and helper method

        /// <summary>
        /// Hooks the method.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value.</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>hook value</returns>
        private IntPtr HookMethod(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WindowsMessages)msg)
            {
                //case WindowsMessages.WM_SIZE:
                //    {
                //        return this.HandleWM_SIZE(hWnd, msg, wParam, lParam, ref handled);
                //    }

                //case WindowsMessages.WM_EXITSIZEMOVE:
                //    {
                //        return this.HandleWM_EXITSIZEMOVE(hWnd, msg, wParam, lParam, ref handled);
                //    }

                //case WindowsMessages.WM_ENTERSIZEMOVE:
                //    {
                //        return this.HandleWM_ENTERSIZEMOVE(hWnd, msg, wParam, lParam, ref handled);
                //    }

                //case WindowsMessages.WM_NCHITTEST:
                //    {
                //        return this.HandleWM_NCHITTEST(hWnd, msg, wParam, lParam, ref handled);
                //    }

                //case WindowsMessages.WM_NCCALCSIZE:
                //    {
                //        return this.HandleWM_NCCALCSIZE(hWnd, msg, wParam, lParam, ref handled);
                //    }

                //case WindowsMessages.WM_GETMINMAXINFO:
                //    {
                //        return this.HandleWM_GETMINMAXINFO(hWnd, msg, wParam, lParam, ref handled);
                //    }

                case WindowsMessages.WM_DWMCOMPOSITIONCHANGED:
                    {
                        return this.HandleWM_DWMCOMPOSITIONCHANGED(hWnd, msg, wParam, lParam, ref handled);
                    }

                //case WindowsMessages.WM_STYLECHANGING:
                //    {
                //        return this.HandleWM_STYLECHANGING(hWnd, msg, wParam, lParam, ref handled);
                //    }

                case WindowsMessages.WM_STYLECHANGED:
                    {
                        return this.HandleWM_STYLECHANGED(hWnd, msg, wParam, lParam, ref handled);
                    }
                //case WindowsMessages.WM_SYSCOMMAND:
                //    {
                //        return this.HandleWM_SYSCOMMAND(hWnd, msg, wParam, lParam, ref handled);
                //    }
                default:
                    return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Handles the W m_ SIZE.
        /// </summary>
        /// <param name="hWnd">The h WND params value.</param>
        /// <param name="msg">The MSG params value..</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>window size</returns>
        private IntPtr HandleWM_SIZE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int value = wParam.ToInt32();

            if (value == 0 || value == 2)
            {
                UpdateWindowRegion(true);
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ EXITSIZEMOVE.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value..</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>updated window region</returns>
        private IntPtr HandleWM_EXITSIZEMOVE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            UpdateWindowRegion(true);

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ ENTERSIZEMOVE.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value.</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>window sizing enter</returns>
        private IntPtr HandleWM_ENTERSIZEMOVE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ GETMINMAXINFO.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value.</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>get min max information</returns>
        private IntPtr HandleWM_GETMINMAXINFO(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Ribbon Dual Monitor 
            if (!CanEnableGlass)
            {
                IntPtr hMonitor = WindowInterop.MonitorFromWindow(hWnd, WindowInterop.MONITOR_DEFAULTTONEAREST);
                WindowInterop.MINMAXINFO minmaxinfo = (WindowInterop.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(WindowInterop.MINMAXINFO));

                if (hMonitor != IntPtr.Zero)
                {
                    //WindowInterop.MONITORINFO monitorInfo = new WindowInterop.MONITORINFO();
                    //WindowInterop.GetMonitorInfo(hMonitor, monitorInfo);
                    //WindowInterop.RECT rcWork = monitorInfo.rcWork;
                    //WindowInterop.RECT rcMonitor = monitorInfo.rcMonitor;

                    WindowInterop.RECT r = GetMaxWindowBounds(hWnd);
                    if(r.left <= 0 && r.top <= 0)
                    {
                    minmaxinfo.ptMaxPosition.x = r.left;
                    minmaxinfo.ptMaxPosition.y = r.top;

                    minmaxinfo.ptMaxSize.x = r.Width;
                    minmaxinfo.ptMaxSize.y = r.Height;
                    }
                }

                minmaxinfo.ptMinTrackSize = new WindowInterop.POINT((base.MinWidth > 0.0) ? ((int)base.MinWidth) : ((int)160.0), (base.MinHeight > 0.0) ? ((int)base.MinHeight) : ((int)38.0));
                minmaxinfo.ptMaxTrackSize = new WindowInterop.POINT((int)minmaxinfo.ptMaxSize.x, (int)minmaxinfo.ptMaxSize.y);
                Marshal.StructureToPtr(minmaxinfo, lParam, true);
                handled = true;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ STYLECHANGING.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value.</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>style changing information</returns>
        private IntPtr HandleWM_STYLECHANGING(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((CanEnableGlass || MaximizedMode == MaximizedMode.KioskStyle) && WindowStyle == WindowStyle.None) 
            {
                UpdateWindowBounds();
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ STYLECHANGED.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value.</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>window style changed</returns>
        private IntPtr HandleWM_STYLECHANGED(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //The following code is added for Ribbon window black border issue SD15764(I102104).
            //This issue reproduces when you set (WindowState=Maximized,WindowStyle=None,ResizeMode=NoResize)
            //once you changed the Screen Resolution 

            if (WindowState == WindowState.Maximized && ResizeMode == ResizeMode.NoResize && WindowStyle == WindowStyle.None)
                WindowChrome.SetWindowChrome(this, null);
            else
                WindowChrome.SetWindowChrome(this, windowChrome);
            //
            
            if ((CanEnableGlass || MaximizedMode == MaximizedMode.KioskStyle) && WindowStyle == WindowStyle.None) 
            {
                UpdateWindowBounds();
            }

            return IntPtr.Zero;
        }

        private IntPtr HandleWM_SYSCOMMAND(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((uint)wParam == (uint)61587)
            {
                IntPtr hMenu = WindowInterop.GetSystemMenu(hWnd, false);
                IntPtr handle = new WindowInteropHelper(this).Handle;
                WindowInterop.RECT rect = new WindowInterop.RECT();
                WindowInterop.GetWindowRect(hWnd, ref rect);
                Point pnt = new Point(rect.left + 8, rect.top + 24);

                if (ResizeMode == ResizeMode.NoResize)
                {
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 3), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
                    WindowInterop.ShowSystemMenu(handle, pnt);
                }
                if (ResizeMode == ResizeMode.CanMinimize)
                {
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 0), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 2), 0x001);
                    WindowInterop.EnableMenuItem(hMenu, (uint)WindowInterop.GetMenuItemID(hMenu, 4), 0x001);
                    WindowInterop.ShowSystemMenu(handle, pnt);
                }
                //handled = true;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ DWMCOMPOSITIONCHANGED.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value..</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Update Glass Change</returns>
        private IntPtr HandleWM_DWMCOMPOSITIONCHANGED(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            this.m_dwmEnabled = WindowInterop.CanEnableDwm();
            if (this.CanEnableGlass)
            {
                this.UpdateGlassChange();
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the W m_ NCCALCSIZE.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value.</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>window value</returns>
        private IntPtr HandleWM_NCCALCSIZE(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr zero = IntPtr.Zero;

            if (this.CanEnableGlass)
            {
                if (wParam == IntPtr.Zero)
                {
                    WindowInterop.RECT rect = (WindowInterop.RECT)Marshal.PtrToStructure(lParam, typeof(WindowInterop.RECT));
                    Marshal.StructureToPtr(WindowInterop.RECT.FromRectangle(this.CalculateClientRectangle(rect.ToRectangle())), lParam, false);
                    zero = IntPtr.Zero;
                }
                else
                {
                    WindowInterop.NCCALCSIZE_PARAMS nccalcsize_params = (WindowInterop.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(WindowInterop.NCCALCSIZE_PARAMS));
                    WindowInterop.WINDOWPOS windowpos = (WindowInterop.WINDOWPOS)Marshal.PtrToStructure(nccalcsize_params.lppos, typeof(WindowInterop.WINDOWPOS));
                    WindowInterop.RECT rect3 = WindowInterop.RECT.FromRectangle(this.CalculateClientRectangle(new Rect((double)windowpos.x, (double)windowpos.y, (double)windowpos.cx, (double)windowpos.cy)));
                    nccalcsize_params.rgrc0 = rect3;
                    nccalcsize_params.rgrc1 = rect3;
                    Marshal.StructureToPtr(nccalcsize_params, lParam, false);
                    zero = (IntPtr)new IntPtr(0x400);
                }
                handled = true;
            }

            
            return zero;
        }

        /// <summary>
        /// Handles the W m_ NCHITTEST.
        /// </summary>
        /// <param name="hWnd">The h WND param value.</param>
        /// <param name="msg">The MSG param value.</param>
        /// <param name="wParam">The w param value.</param>
        /// <param name="lParam">The l param value.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>window value</returns>
        private IntPtr HandleWM_NCHITTEST(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (!this.CanEnableGlass)
            {
                return IntPtr.Zero;
            }

            int plResult = 0;
            IntPtr zero = IntPtr.Zero;
            int x = WindowInterop.GetX(lParam);
            int y = WindowInterop.GetY(lParam);
            WindowInterop.RECT rect = new WindowInterop.RECT();
            WindowInterop.GetWindowRect(hWnd, ref rect);
            Point point = new Point((double)x, (double)y);
            base.PointFromScreen(new Point((double)x, (double)y));

            WindowInterop.DwmDefWindowProc(hWnd, (int)WindowsMessages.WM_NCHITTEST, wParam, lParam, out zero);

            plResult = zero.ToInt32();
            handled = true;
            if (plResult == 20 || plResult == 8 || plResult == 9 || plResult == 21)
            {
                return zero;
            }
            else
            {
                if (!((base.ResizeMode == ResizeMode.NoResize || base.ResizeMode == ResizeMode.CanMinimize) && !HitOverClient(point, rect)))
                {
                    handled = false;
                }
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Checks whether mouse point is within the client area of the window
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private bool HitOverClient(Point p, WindowInterop.RECT rect)
        {
            double borderSize = SystemInformation.FrameBorderSize.Width;
            double offsetAtTop = 1;

            if (WindowStyle == WindowStyle.ThreeDBorderWindow)
                borderSize += SystemInformation.Border3DSize.Width;

            Rect cRect = new Rect(rect.Location.X + borderSize, rect.Location.Y + offsetAtTop, rect.Width - (borderSize * 2), rect.Height - borderSize - offsetAtTop);
            return cRect.Contains(p);
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
                int width = (int)(((int)base.Width) + 1);
                int height = (int)(((int)base.Height) + 1);

                if (hWnd != IntPtr.Zero)
                {
                    WindowInterop.RECT r = new WindowInterop.RECT();
                    WindowInterop.RECT rcWork = new WindowInterop.RECT();
                    if (WindowState == WindowState.Maximized)
                    {
                        IntPtr hMonitor = WindowInterop.MonitorFromWindow(hWnd, WindowInterop.MONITOR_DEFAULTTONEAREST);
                        WindowInterop.MONITORINFO monitorInfo = new WindowInterop.MONITORINFO();
                        WindowInterop.GetMonitorInfo(hMonitor, monitorInfo);
                        r = monitorInfo.rcMonitor;
                        rcWork = monitorInfo.rcWork;
                    }
                    else
                        WindowInterop.GetWindowRect(hWnd, ref r);

                    width = (int)(r.Width + 1);
                    height = (int)(r.Height + 1);
                    double tempWidth = Math.Abs(System.Windows.SystemParameters.VirtualScreenWidth - System.Windows.SystemParameters.PrimaryScreenWidth);

                    if (r.Location.X >= System.Windows.SystemParameters.PrimaryScreenWidth || r.Location.X < 0)
                    {
                        if (WindowState == WindowState.Maximized)
                        {
                            MaxWidth = rcWork.Width;
                            MaxHeight = rcWork.Height;
                        }
                        else
                        {
                            MaxWidth = tempWidth;
                            MaxHeight = System.Windows.SystemParameters.VirtualScreenHeight;
                        }
                    }
                    else
                    {
                        MaxWidth = SystemInformation.WorkingArea.Width;
                        MaxHeight = SystemInformation.WorkingArea.Height;
                    }

                    if (WindowState == WindowState.Maximized)
                        updateNew = true;
                    else
                        updateNew = false;
                }

                this.DefineWindowRegion(bRedraw, width, height);
            }
        }

        /// <summary>
        /// Defines the window region.
        /// </summary>
        /// <param name="bRedraw">if set to <c>true</c> [b redraw].</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void DefineWindowRegion(bool bRedraw, int width, int height)
        {
            if (base.WindowState != WindowState.Minimized)
            {
                IntPtr hWnd = new WindowInteropHelper(this).Handle;
                IntPtr zero = IntPtr.Zero;

                if (base.WindowState != WindowState.Maximized)
                {
                    zero = WindowInterop.CreateRoundRectRgn(0, 0, width, height, 7, 7);
                }

                WindowInterop.SetWindowRgn(hWnd, zero, bRedraw);
            }
        }

        /// <summary>
        /// Updates the glass change.
        /// </summary>
        private void UpdateGlassChange()
        {
            if (this.m_isInitialized)
            {
               // int windowLong = 0;

                if (!this.CanEnableGlass)
                {
                    //if (base.WindowStyle != WindowStyle.None)
                    //{
                    //    base.WindowStyle = WindowStyle.None;
                    //}

                    //IntPtr hWnd = new WindowInteropHelper(this).Handle;
                    //HwndSource.FromHwnd(hWnd).CompositionTarget.BackgroundColor = Colors.Black;
                    //windowLong = WindowInterop.GetWindowLong(hWnd, -16);
                    //windowLong = (int)(windowLong & ~(windowLong & 0x40000));
                    //WindowInterop.SetWindowLong(hWnd, -16, windowLong);
                    //WindowInterop.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, 0x27);

                    //UpdateWindowRegion(false);
                    if (windowChrome != null)
                    {
                        windowChrome.GlassFrameThickness = SkinStorage.GetVisualStyle(this) != "Office2013" ? new Thickness(0) : new Thickness(1);
                        if (captionHeight > 0d)
                            windowChrome.CaptionHeight = captionHeight;
                        WindowChrome.SetWindowChrome(this,windowChrome);
                    }
                    this.IsGlassActive = false;                
                }
                else
                {
                    //IntPtr ptr = new WindowInteropHelper(this).Handle;
                    //WindowInterop.SetWindowRgn(ptr, IntPtr.Zero, false);
                    //int dwNewLong = WindowInterop.GetWindowLong(ptr, -16);
                    //if ((dwNewLong & 0x40000) == 0)
                    //{
                    //    dwNewLong = (int)(dwNewLong | 0x40000);
                    //    WindowInterop.SetWindowLong(ptr, -16, dwNewLong);
                    //    WindowInterop.SetWindowPos(ptr, IntPtr.Zero, 0, 0, 0, 0, 0x27);
                    //}

                    //this.ExtendWindow();
                    if (windowChrome != null)
                    {
                       
                        if (captionHeight > 0d)
                            windowChrome.CaptionHeight = captionHeight;

                        if (this.ResizeMode != ResizeMode.NoResize)
                        {
                            if (SkinStorage.GetVisualStyle(this) != "Office2013")
                            {
                                Thickness framethickness = windowChrome.GlassFrameThickness;
                                if (DPIOffset != 1.0)
                                    framethickness.Top += DPIOffset;

 #if SyncfusionFramework4_5

                            if (this.rootGrid != null)
                                this.rootGrid.Margin = new Thickness(2, 0, 2, 2);

                            windowChrome.GlassFrameThickness = new Thickness(10, 55, 10, 10);
#else

                                windowChrome.GlassFrameThickness =
                                    new Thickness(framethickness.Left + SystemParameters.ThickVerticalBorderWidth,
                                                  framethickness.Top + SystemParameters.WindowCaptionHeight +
                                                  SystemParameters.ThickHorizontalBorderHeight +
                                                  SystemParameters.ThinHorizontalBorderHeight,
                                                  framethickness.Right + SystemParameters.ThickVerticalBorderWidth,
                                                  framethickness.Bottom +
                                                  SystemParameters2.Current.WindowResizeBorderThickness.Bottom);

#endif
                                windowChrome.NonClientFrameEdges = NonClientFrameEdges.Bottom | NonClientFrameEdges.Left |
                                                                   NonClientFrameEdges.Right;
                            }
                            else
                            {
                                windowChrome.GlassFrameThickness = new Thickness(1);
                                windowChrome.NonClientFrameEdges = NonClientFrameEdges.None;
                            }
                        }

                        WindowChrome.SetWindowChrome(this, windowChrome);
                    }
                   
                   
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

            if (this.WindowTitleVisualStyle != null && (this.WindowTitleVisualStyle.Contains("Office2010") || this.WindowTitleVisualStyle.Contains("Transparent") || this.WindowTitleVisualStyle.Contains("Metro")))
            {
                num = (int)(((int)Math.Ceiling((double)(53 * dpi))) - 1);

                if (WindowState == WindowState.Maximized)
                    num += SystemInformation.FrameBorderSize.Width / 2;
            }

         
            else if (WindowState == WindowState.Maximized)
            {
                num += SystemInformation.FrameBorderSize.Width;
            }

            IntPtr ptr = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(ptr).CompositionTarget.BackgroundColor = Colors.Transparent;

            WindowInterop.ExtendWindow(ptr, num);
        }

        /// <summary>
        /// Calculates the client rectangle.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <returns>client rectangle window</returns>
        private Rect CalculateClientRectangle(Rect rect)
        {
            int width = SystemInformation.FrameBorderSize.Width * SystemInformation.BorderMultiplierFactor;
            int height = SystemInformation.FrameBorderSize.Height * SystemInformation.BorderMultiplierFactor;
            double dpi = this.DPIOffset;

            if (WindowStyle == WindowStyle.ThreeDBorderWindow)
            {
                width += SystemInformation.Border3DSize.Width;
                height += SystemInformation.Border3DSize.Height;
            }

            if (dpi >= 1.5)
            {
                dpi = 1.0 - Math.Abs(dpi - 1.0);
                width = (int)dpi * width;
                height = (int)dpi * height;
            }

            if (rect.X == 0)
                rect.X -= width;
            else if (rect.X <= SystemInformation.WorkingArea.Width)
            {
                if (this.WindowState == System.Windows.WindowState.Maximized && this.WindowStyle == System.Windows.WindowStyle.None)
                {
                    if (rect.X < SystemInformation.WorkingArea.Width)
                        rect.Width = SystemInformation.WorkingArea.Width;
                    else
                        rect.X = SystemInformation.WorkingArea.Width - width;
                }
            }
            if (this.DPIOffset >= 1.5)
            {
                if (width == 0)
                    width = SystemInformation.FrameBorderSize.Width;
                if (height == 0)
                    height = SystemInformation.FrameBorderSize.Height;
            }
            rect.X += width;
            rect.Width -= width * 2;
            rect.Height -= height;
            NativeMethods.APPBARDATA appbardata = new NativeMethods.APPBARDATA();
            
            // Method to identify whether TaskBar is autohidden, if temp value is 1, TaskBar is autohidden.

            int temp = (int)NativeMethods.SHAppBarMessage(4, ref appbardata);     
          
            if (SystemInformation.MonitorCount == 1)
            {
                if (temp == 1)
                {
                    if (rect.Height > SystemInformation.WorkingArea.Height)
                        rect.Height = SystemInformation.WorkingArea.Height + 4.5;                
                }
                else if (rect.Height > SystemInformation.PrimaryMonitorSize.Height)
                    rect.Height = SystemInformation.PrimaryMonitorSize.Height + (rect.Height - SystemInformation.WorkingArea.Height);

                if (rect.Width > SystemInformation.WorkingArea.Width || (this.WindowState == System.Windows.WindowState.Maximized && this.WindowStyle == System.Windows.WindowStyle.None))
                    rect.Width = SystemInformation.WorkingArea.Width;
            }
            else
            {
                if (Math.Abs(currentWindowPoint.X) <= SystemInformation.PrimaryMonitorSize.Width)
                {
                    if (temp == 1)
                    {
                        if (rect.Height > SystemInformation.WorkingArea.Height)
                            rect.Height = SystemInformation.WorkingArea.Height + 4.5;
                    }
                    else if (rect.Height > SystemInformation.WorkingArea.Height)
                        rect.Height = SystemInformation.WorkingArea.Height + (rect.Height - SystemInformation.WorkingArea.Height);
                    if (this.WindowState == System.Windows.WindowState.Maximized && this.WindowStyle == System.Windows.WindowStyle.None)
                    {
                        rect.Width = SystemInformation.WorkingArea.Width;
                    }
                }
                else
                {
                    if (temp == 1)
                    {
                        if (rect.Height > SystemInformation.VirtualScreen.Height)
                            rect.Height = SystemInformation.VirtualScreen.Height + 5;
                    }
                    else
                    {
                        if (rect.Height > SystemInformation.VirtualScreen.Height)
                            rect.Height = SystemInformation.VirtualScreen.Height + (rect.Height - SystemInformation.VirtualScreen.Height);
                    }
                    if (this.WindowState == System.Windows.WindowState.Maximized && this.WindowStyle == System.Windows.WindowStyle.None)
                    {
                        rect.Width = SystemInformation.VirtualScreen.Width;
                    }
                }
            }
            return rect;
        }

        /// <summary>
        /// Gets the sizing direction.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>window sizing directions</returns>
        private WindowInterop.SizingDirection GetSizingDirection(System.Windows.Point point)
        {
            Thickness thickness;
            Size size;
            CornerRadius radius;
            if (base.ResizeMode == ResizeMode.NoResize || WindowState != WindowState.Normal)
            {
                return WindowInterop.SizingDirection.None;
            }

            Thickness thick = this.BorderThickness;
            if (IsGlassActive && this.ResizeMode == System.Windows.ResizeMode.CanMinimize)
                thickness = new Thickness(0.0, 0.0, 0.0, 0.0);
            else if (IsGlassActive)
                thickness = new Thickness(0.0,5.0, 0.0, 0.0);            
            else
            {
                if (thick.Bottom <=1 || thick.Left <=1 || thick.Right <=1 || thick.Top <=1)
                    thick = new Thickness(2);
                thickness = thick;
            }
            radius = new CornerRadius(6.0, 6.0, 6.0, 6.0);
            size = new Size(base.ActualWidth, base.ActualHeight);
            if (WindowInterop.RECT.GetExtendedRect(new Rect(0.0, 0.0, size.Width, size.Height), thickness).Contains(point))
            {
                if (ResizeMode == ResizeMode.CanResizeWithGrip && ResizeGrip.IsMouseOver)
                {
                    return WindowInterop.SizingDirection.SouthEast;
                }
                else if ((((point.Y >= (size.Height - radius.BottomRight)) && (point.Y <= size.Height)) && (point.X >= (size.Width - radius.BottomRight))) && (point.X <= size.Width))
                {
                    return WindowInterop.SizingDirection.SouthEast;
                }

                return WindowInterop.SizingDirection.None;
            }

            
            if ((point.Y > thickness.Top) || ((point.Y < 0.0) || (point.X > radius.TopLeft)) || (point.X < 0.0))
            {
                if ((point.Y < radius.TopRight) && (point.X >= (size.Width - radius.TopRight)))
                {
                    return WindowInterop.SizingDirection.NorthEast;
                }

                if (((point.Y >= (size.Height - radius.BottomLeft)) && (point.Y <= size.Height)) && ((point.X <= radius.BottomLeft) && (point.X >= 0.0)))
                {
                    return WindowInterop.SizingDirection.SouthWest;
                }

                if ((((point.Y >= (size.Height - radius.BottomRight)) && (point.Y <= size.Height)) && (point.X >= (size.Width - radius.BottomRight))) && (point.X <= size.Width))
                {
                    return WindowInterop.SizingDirection.SouthEast;
                }

                if ((point.Y <= thickness.Top) && (point.Y >= 0.0))
                {
                    return WindowInterop.SizingDirection.North;
                }

                if ((point.X <= thickness.Left) && (point.X >= 0.0))
                {
                    return WindowInterop.SizingDirection.West;
                }

                if ((point.X >= (size.Width - thickness.Right)) && (point.X <= size.Width))
                {
                    return WindowInterop.SizingDirection.East;
                }

                if ((point.Y < (size.Height - thickness.Bottom)) || (point.Y > size.Height))
                {
                    return WindowInterop.SizingDirection.None;
                }

                return WindowInterop.SizingDirection.South;
            }

            return WindowInterop.SizingDirection.NorthWest;
        }

        /// <summary>
        /// Sends the sizing message.
        /// </summary>
        /// <param name="sizing">The sizing.</param>
        private void SendSizingMessage(WindowInterop.SizingDirection sizing)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                IntPtr hWnd = new WindowInteropHelper(this).Handle;
                WindowInterop.SendMessage(hWnd, WindowInterop.WM_SYSCOMMAND, (int)(0xf000 + sizing), 0);
                WindowInterop.SendMessage(hWnd, WindowInterop.WM_LBUTTONUP, 0, 0);
            }
        }
        #endregion
    }
}

