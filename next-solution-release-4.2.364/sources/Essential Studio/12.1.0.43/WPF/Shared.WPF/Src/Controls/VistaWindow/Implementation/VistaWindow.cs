// <copyright file="VistaWindow.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a VistaWindow control.
    /// </summary>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <para/>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class VistaWindow : Window</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<shared:VistaWindow x:Class="Sample.Window1" x:Name="VistaWindow"/>]]></code></example>
    /// </list>
    /// <para/>
    /// </example>
    /// </list>
    /// <para/>
    /// <remarks>
    /// VistaWindow class represents main UI element - Window control.
    /// </remarks>
    /// <para/>
    /// <example>
    /// <para/>This example shows how to create a VistaWindow in XAML.
    /// <code><![CDATA[<sample:VistaWindow x:Class="Sample.Window1" x:Name="VistaWindow"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:shared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF"
    /// xmlns:sample="clr-namespace:Sample"
    /// Title="Test" Width="800" Height="600"/>]]></code>
    /// <para/>
    /// <para/>This example shows how to create a VistaWindow in C#.
    /// <code>    
    /// using System;
    /// using System.Windows;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace CSharp
    /// {
    ///     public partial class CodeOnlyWindow : VistaWindow
    ///     {
    ///         public CodeOnlyWindow()
    ///         {
    ///             this.Title = "Main Window in Code Only";
    ///             this.Width = 300;
    ///             this.Height = 300;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VistaWindow : Window
    {
        #region Constants
        /// <summary>
        /// Define sub key name
        /// </summary>
        private const string REG_SUB_KEY_NAME = "VistaWindowInfo";

        /// <summary>
        /// Define param name
        /// </summary>
        private const string REG_PARAM_NAME = "SaveWindowState";

        /// <summary>
        /// Define buffer size
        /// </summary>
        private const int REG_BUFFER_SIZE = 500;

        /// <summary>
        /// Define window border brush
        /// </summary>
        private static LinearGradientBrush windowBorderBrush;

        /// <summary>
        /// Define Title bar background
        /// </summary>
        private static LinearGradientBrush titlebarbackground;

        /// <summary>
        /// Define navigation bar background
        /// </summary>
        private static LinearGradientBrush navigationbarbackground;

        /// <summary>
        /// Define window content area border
        /// </summary>
        private static LinearGradientBrush windowContentAreaBorder;
        #endregion

        #region Fields
        /// <summary>
        /// Define title bar click
        /// </summary>
        private DateTime m_lastTitlebarClick;

        /// <summary>
        /// Define Title bar point
        /// </summary>
        private Point m_lastTitlebarPoint;

        /// <summary>
        /// Define theme color
        /// </summary>
        private SolidColorBrush m_themeColor = new SolidColorBrush();
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [persist state].
        /// </summary>
        /// <value><c>true</c> if [persist state]; otherwise, <c>false</c>.</value>
        public bool PersistState
        {
            get
            {
                return (bool)GetValue(PersistStateProperty);
            }

            set
            {
                SetValue(PersistStateProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is navigation bar enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is navigation bar enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNavigationBarEnabled
        {
            get
            {
                return (bool)GetValue(IsNavigationBarEnabledProperty);
            }

            set
            {
                SetValue(IsNavigationBarEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets UI element that represents container for navigation buttons.
        /// </summary>
        /// <value>The navigation bar.</value>
        public NavigationBar NavigationBar
        {
            get
            {
                return (NavigationBar)base.GetTemplateChild("PART_NavigationToolbar");
            }

            set
            {
                SetValue(NavigationBarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents window border brush.
        /// </summary>
        /// <value>The window border brush.</value>
        public LinearGradientBrush WindowBorderBrush
        {
            get
            {
                return (LinearGradientBrush)GetValue(WindowBorderBrushProperty);
            }

            set
            {
                SetValue(WindowBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value that represents background brush of <see cref="TitleBar"/>.
        /// </summary>
        /// <value>The title bar background.</value>
        public LinearGradientBrush TitleBarBackground
        {
            get
            {
                return (LinearGradientBrush)GetValue(TitleBarBackgroundProperty);
            }

            set
            {
                SetValue(TitleBarBackgroundProperty, value);
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
        /// <value>The window content area border.</value>
        public LinearGradientBrush WindowContentAreaBorder
        {
            get
            {
                return (LinearGradientBrush)GetValue(WindowContentAreaBorderProperty);
            }

            set
            {
                SetValue(WindowContentAreaBorderProperty, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="VistaTitleBar"/> control of the <see cref="VistaWindow"/>.
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
        /// <example><code>public VistaTitleBar TitleBar{ get; }</code></example>
        /// </list>
        /// <para/>
        /// <list type="table">
        /// <listheader>
        /// <description>XAML Object Element Usage</description>
        /// </listheader>
        /// <example><code><![CDATA[<shared:VistaTitleBar x:Name="title"/>]]></code></example>
        /// </list>       
        /// </example>
        /// </list>        
        /// <remarks>
        /// Type: <see cref="VistaTitleBar"/>
        /// Instance of VistaTitleBar control used in window.       
        /// </remarks>
        /// <example> 
        /// <para/>This example shows how to use a VistaTitleBar in C#.
        /// <code>    
        /// using System;
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace CSharp
        /// {
        ///     public partial class CodeOnlyWindow : VistaWindow
        ///     {
        ///         public CodeOnlyWindow()
        ///         {
        ///             VistaTitleBar title = this.TitleBar;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="VistaTitleBar"/>
        /// <seealso cref="VistaWindow"/>
        public VistaTitleBar TitleBar
        {
            get
            {
                return (VistaTitleBar)base.GetTemplateChild("PART_TitleBar");
            }
        }

        /// <summary>
        /// Gets or sets value that represents main color of theme.
        /// </summary>
        /// <value>The color of the theme.</value>
        internal Color ThemeColor
        {
            get
            {
                return (Color)GetValue(ThemeColorValueProperty);
            }

            set
            {
                SetValue(ThemeColorValueProperty, value);
            }
        }
        #endregion

        #region dependency Properties
        /// <summary>
        /// Identifies <see cref="NavigationBar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationBarProperty = DependencyProperty.Register("NavigationBar", typeof(UIElement), typeof(VistaWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="WindowBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowBorderBrushProperty = DependencyProperty.Register("WindowBorderBrush", typeof(LinearGradientBrush), typeof(VistaWindow), new UIPropertyMetadata(windowBorderBrush));

        /// <summary>
        /// Identifies <see cref="TitleBarBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register("TitleBarBackground", typeof(LinearGradientBrush), typeof(VistaWindow), new UIPropertyMetadata(titlebarbackground));

        /// <summary>
        /// Identifies <see cref="NavigationBarBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationBarBackgroundProperty = DependencyProperty.Register("NavigationBarBackground", typeof(LinearGradientBrush), typeof(VistaWindow), new UIPropertyMetadata(navigationbarbackground));

        /// <summary>
        /// Identifies <see cref="WindowContentAreaBorder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WindowContentAreaBorderProperty = DependencyProperty.Register("WindowContentAreaBorder", typeof(LinearGradientBrush), typeof(VistaWindow), new UIPropertyMetadata(windowContentAreaBorder));

        /// <summary>
        /// Identifies <see cref="IsNavigationBarEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNavigationBarEnabledProperty = DependencyProperty.Register("IsNavigationBarEnabled", typeof(bool), typeof(VistaWindow), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies <see cref="PersistState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PersistStateProperty = DependencyProperty.Register("PersistState", typeof(bool), typeof(VistaWindow), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPersistStateChanged)));

        /// <summary>
        /// Identifies <see cref="ThemeColorValueProperty"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ThemeColorValueProperty = DependencyProperty.Register("ThemeColorValue", typeof(Color), typeof(VistaWindow), new UIPropertyMetadata((Color)ColorConverter.ConvertFromString("#FF576077")));
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="VistaWindow"/> class.
        /// </summary>
        static VistaWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaWindow), new FrameworkPropertyMetadata(typeof(VistaWindow)));
            EnvironmentTest.ValidateLicense(typeof(VistaWindow));

            windowBorderBrush = new LinearGradientBrush();
            titlebarbackground = new LinearGradientBrush();
            navigationbarbackground = new LinearGradientBrush();
            windowContentAreaBorder = new LinearGradientBrush();

            windowBorderBrush.EndPoint = new Point(1, 0.5);
            windowBorderBrush.StartPoint = new Point(0, 0.5);
            windowBorderBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF576077"), 0));
            windowBorderBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5F6982"), 1));

            titlebarbackground.EndPoint = new Point(0.489, 0.896);
            titlebarbackground.StartPoint = new Point(0.489, -0.104);
            titlebarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF454A5B"), 0.644));
            titlebarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF020203"), 0.322));
            titlebarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5F6982"), 0.981));
            titlebarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF18181B"), 0.462));
            titlebarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF464C5B"), 0.625));
            titlebarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF22232A"), 0.529));

            navigationbarbackground.EndPoint = new Point(0.5, 1);
            navigationbarbackground.StartPoint = new Point(0.5, 0);
            navigationbarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5E6882"), 0));
            navigationbarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF252F36"), 0.5));
            navigationbarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF111111"), 0.5));
            navigationbarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF1A1B1C"), 0.827));
            navigationbarbackground.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF547C96"), 1));

            windowContentAreaBorder.EndPoint = new Point(0.474, 0.027);
            windowContentAreaBorder.StartPoint = new Point(0.473, 0.15);
            windowContentAreaBorder.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF000000"), 0));
            windowContentAreaBorder.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5F6982"), 1));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VistaWindow"/> class.
        /// </summary>
        public VistaWindow()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(VistaWindow));
            }
            base.SourceInitialized += new EventHandler(this.InitSource);
            if (VistaWindowInterop.CanExtend())
            {
                AllowsTransparency = false;
                BorderThickness = new Thickness(0);
            }

            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="PersistState"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PersistStateChanged;
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnPersistStateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnPersistStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VistaWindow instance = (VistaWindow)d;
            instance.OnPersistStateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// <see cref="PersistStateChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnPersistStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PersistStateChanged != null)
            {
                PersistStateChanged(this, e);
            }
        }

        /// <summary>
        /// Saves to registry.
        /// </summary>
        /// <param name="themeColorValue">The theme color value.</param>
        internal void SaveToRegistry(Color themeColorValue)
        {
            ThemeColor = themeColorValue;
            BinaryFormatter serializer = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream(REG_BUFFER_SIZE);
            byte[] byteArray = Encoding.UTF8.GetBytes(string.Format("ColorValue" + "=" + ThemeColor + ";"));
            serializer.Serialize(memStream, byteArray);
            memStream.Position = 0;
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(REG_SUB_KEY_NAME);
            Registry.SetValue(regKey.ToString(), REG_PARAM_NAME, memStream.ToArray(), RegistryValueKind.Binary);
        }

        /// <summary>
        /// Loads control from registry.
        /// </summary>
        internal void LoadFromRegistry()
        {
            BinaryFormatter serializer = new BinaryFormatter();
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REG_SUB_KEY_NAME);

            if (regKey != null)
            {
                MemoryStream memStream = new MemoryStream(REG_BUFFER_SIZE);

                byte[] byteArr = (byte[])regKey.GetValue(REG_PARAM_NAME);

                if (byteArr != null)
                {
                    memStream.Write(byteArr, 0, byteArr.Length);
                    memStream.Position = 0;

                    try
                    {
                        serializer.Deserialize(memStream);
                    }
                    catch (SerializationException)
                    {
                        throw;
                    }

                    char[] ch = Encoding.UTF8.GetChars(byteArr);
                    string endStr = new string(ch);

                    string[] kvPairs = endStr.Split(';');
                    string fromStr = kvPairs[0];
                    int n = fromStr.IndexOf("ColorValue");
                    string val = string.Empty;

                    if (n >= 0)
                    {
                        int i = fromStr.IndexOf('=', n);
                        val = fromStr.Substring(++i, 9);
                        ThemeColor = (Color)ColorConverter.ConvertFromString(val);
                    }
                }
            }
        }

        /// <summary>
        /// Inits the source.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void InitSource(object sender, EventArgs e)
        {
            HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(new HwndSourceHook(this.HookMethod));
            this.ExtendWindow();
        }

        /// <summary>
        /// Extends the window.
        /// </summary>
        private void ExtendWindow()
        {
            if (VistaWindowInterop.CanExtend())
            {
                VistaWindowInterop.ExtendWindowIntoClientArea(this, new Thickness(0, 28, 0, 0));
            }
        }

        /// <summary>
        /// Hooks the method.
        /// </summary>
        /// <param name="hwnd">The HWND integer points</param>
        /// <param name="msg">The MSG to be displayed.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="handled">if set to <c>true</c> [handled].</param>
        /// <returns>Integer pointer</returns>
        private IntPtr HookMethod(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                //// public const int WM_GETMINMAXINFO = 0x24;
                case 0x24:
                    VistaWindowInterop.HandleMinMax(this, hwnd, lParam);
                    handled = true;
                    break;

                //// public const int WM_NCHITTEST = 0x84;
                case 0x84:
                    if (!handled)
                    {
                        int point = lParam.ToInt32();
                        int screenX = VistaWindowInterop.GetX(point);
                        int screenY = VistaWindowInterop.GetY(point);

                        double x = screenX - base.Left;
                        double y = screenY - base.Top;

                        if (this.WindowState != WindowState.Maximized)
                        {
                            if ((x >= 0) && (x < 4))
                            {
                                handled = true;
                                if ((y >= 0) && (y < 29))
                                {
                                    //// #define HTTOPLEFT           13
                                    return new IntPtr(13);
                                }

                                if ((y < (base.ActualHeight - 4)) || (y >= base.ActualHeight))
                                {
                                    //// #define HTLEFT              10
                                    return new IntPtr(10);
                                }
                                //// #define HTBOTTOMLEFT        16
                                return new IntPtr(0x10);
                            }

                            if ((x >= (base.ActualWidth - 4)) && (x < base.ActualWidth))
                            {
                                handled = true;
                                if ((y >= 0) && (y < 29))
                                {
                                    //// #define HTTOPRIGHT          14
                                    return new IntPtr(14);
                                }

                                if ((y < (base.ActualHeight - 4)) || (y >= base.ActualHeight))
                                {
                                    //// #define HTRIGHT             11
                                    return new IntPtr(11);
                                }

                                return new IntPtr(0x11);
                            }

                            if ((y < 0) || (y >= 4))
                            {
                                if ((y >= (base.ActualHeight - 4)) && (y < base.ActualHeight))
                                {
                                    //// #define HTBOTTOM            15
                                    handled = true;
                                    return new IntPtr(15);
                                }
                            }
                            else
                            {
                                // #define HTTOP               12
                                handled = true;
                                return new IntPtr(12);
                            }
                        }

                        goto Label_02B7;
                    }

                    break;
            }

        Label_02B7:
            return IntPtr.Zero;
        }

        #endregion

        #region Color Scheme Codes
        /// <summary>
        /// Changes VistaWindow color scheme to custom according to specified blending color.
        /// </summary>
        /// <param name="blendColor">Blending color.</param>
        /// <example>
        /// <code>
        /// VistaWindow window;
        /// // ....
        /// window.ChangeColorScheme(Colors.Red);
        /// </code>
        /// </example>
        /// <seealso cref="Color"/>
        /// <seealso cref="VistaWindow"/>
        public void ChangeColorScheme(Color blendColor)
        {
            ThemeColor = blendColor;

            WindowBorderBrush = ApplyGradientCustomColor(windowBorderBrush);

            TitleBarBackground = ApplyGradientCustomColor(titlebarbackground);

            NavigationBarBackground = ApplyGradientCustomColor(navigationbarbackground);

            WindowContentAreaBorder = ApplyGradientCustomColor(windowContentAreaBorder);
        }

        /// <summary>
        /// Applies the color of the gradient custom.
        /// </summary>
        /// <param name="brush">Given <see cref="LinearGradientBrush"/> object.</param>
        /// <returns>Return the linear gradient brush</returns>
        private LinearGradientBrush ApplyGradientCustomColor(LinearGradientBrush brush)
        {
            LinearGradientBrush newBrush = new LinearGradientBrush();

            newBrush.StartPoint = brush.StartPoint;
            newBrush.EndPoint = brush.EndPoint;

            foreach (GradientStop stop in brush.GradientStops)
            {
                GradientStop newStop = new GradientStop(GetColor(stop.Color), stop.Offset);
                newBrush.GradientStops.Add(newStop);
            }

            return newBrush;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <param name="baseColor">Color of the base.</param>
        /// <returns>Return the color</returns>
        internal Color GetColor(Color baseColor)
        {
            if (baseColor.A == 0)
            {
                return baseColor;
            }

            return Color.FromArgb(baseColor.A, MergeChannels(baseColor.R, ThemeColor.R), MergeChannels(baseColor.G, ThemeColor.G), MergeChannels(baseColor.B, ThemeColor.B));
        }

        /// <summary>
        /// Merges the channels.
        /// </summary>
        /// <param name="baseChannel">The base channel.</param>
        /// <param name="blendChannel">The blend channel.</param>
        /// <returns>Return the merge channel</returns>
        internal byte MergeChannels(int baseChannel, int blendChannel)
        {
            int mediana, dif, rest, max = 255;

            mediana = baseChannel * blendChannel / max;
            dif = (max - baseChannel) * (max - blendChannel) / max;
            rest = baseChannel * (max - dif - mediana);

            return (byte)(mediana + (rest / 255));
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Invoked whenever application code or internal processes call ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ChangeColorScheme(ThemeColor);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (PersistState == true)
            {
                LoadFromRegistry();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
           base.OnClosed(e);
            if (PersistState == true)
            {
                SaveToRegistry(ThemeColor);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
           base.OnMouseUp(e);

            if ((!e.Handled && (e.ChangedButton == MouseButton.Right)) && this.TitleBar.IsMouseOver)
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                Point point = base.PointToScreen(e.GetPosition(this));
                VistaWindowInterop.ShowSystemMenu(handle, point);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
           base.OnMouseDown(e);
            if ((!e.Handled && (e.ChangedButton == MouseButton.Left)) && this.TitleBar.IsMouseOver)
            {
                Point position = e.GetPosition(this);
                if (((DateTime.Now.Subtract(m_lastTitlebarClick).TotalMilliseconds < 500) && (Math.Abs((double)(m_lastTitlebarPoint.X - position.X)) <= 2)) && (Math.Abs((double)(m_lastTitlebarPoint.Y - position.Y)) <= 2))
                {
                    if (this.ResizeMode != ResizeMode.NoResize)
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
                }
                else
                {
                    m_lastTitlebarPoint = e.GetPosition(this);
                    this.DragMove();
                }

                m_lastTitlebarClick = DateTime.Now;
            }
        }

        #endregion
    }
}
