// <copyright file="FontListComboBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents combo box that contains the list of fonts.
    /// </summary>
    /// <para>FontListComboBox class is handy for working with fonts.</para>
    /// <para>Fonts can be organized in three groups (theme fonts, recently used fonts and all fonts groups).</para>
    /// <para>FontListComboBox supports 6 Windows themes (Default, Silver, Metallic, Zune, Royale and Aero) and 5 skins
    /// (Office2003, Office2007Blue, Office2007Black, Office2007Silver and Blend).<para/>
    /// <para>Also you can define your own user skin by setting properties of the FontListComboBox class.<para/>
    /// <example>
    /// <para>This example shows how to create a FontListComboBox in XAML.</para>
    /// <code>
    /// <Window x:Class="FontListSample.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="FontListComboBox" Height="300" Width="300">
    /// <StackPanel Name="panel1" HorizontalAlignment="Center" Width="208">
    ///        <local:FontListComboBox Name="fontListComboBox1"/>
    /// </StackPanel>
    /// </Window>
    /// </code>
    /// <para>This example shows how to create a FontListComboBox in C#.</para>
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// namespace FontListSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         public Window1()
    ///         {
    ///             InitializeComponent();
    ///             FontListComboBox fontListComboBox1 = new FontListComboBox();
    ///             this.panel1.Children.Add( fontListComboBox1 );
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </para>
    /// </para>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
  Type = typeof(FontListComboBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/FontComboBox/Themes/TransparentStyle.xaml")]
    public class FontListComboBox : FontListBase
    {
        #region Constants

        /// <summary>
        /// Default fonts source.
        /// </summary>
        private const string SystemFontsSource = "System";

        #endregion Constants

        #region Private fields

        /// <summary>
        /// Collection of theme fonts.
        /// </summary>
        private ThemeFontFamilyCollection m_collectionThemeFontsDefault = new ThemeFontFamilyCollection();

        /// <summary>
        /// Collection of recently used fonts.
        /// </summary>
        private FontCollection m_collectionRecentlyUsedFontsDefault = new FontCollection();

        /// <summary>
        /// Command that opens popup with FintListBox control.
        /// </summary>
        public static RoutedCommand M_openPopup;

        /*
        ///// <summary>
        ///// Default foreground.
        ///// </summary>
        //private Brush m_defaultForeground = SystemColors.ControlTextBrush;
        ///// <summary>
        ///// Default background.
        ///// </summary>
        //private Brush m_defaultBackground = Brushes.Transparent;
        ///// <summary>
        ///// Default border brush.
        ///// </summary>
        //private Brush m_defaultBorderBrush = Brushes.Transparent;
         */

        #endregion Private fields

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="FontListComboBox"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static FontListComboBox()
        {
            //EnvironmentTest.ValidateLicense(typeof(FontListComboBox));
            M_openPopup = new RoutedCommand();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontListComboBox), new FrameworkPropertyMetadata(typeof(FontListComboBox)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontListComboBox"/> class.
        /// </summary>
        public FontListComboBox()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(FontListComboBox));
            }
            CommandBinding openPopupValueBinding = new CommandBinding(M_openPopup);
            openPopupValueBinding.Executed += new ExecutedRoutedEventHandler(ChangeOpenPopupValue);
            CoerceValue(AllFontsProperty);
            CoerceValue(ThemeFontsProperty);
            CoerceValue(RecentlyUsedFontsProperty);
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether dropDown is shown. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }

            set
            {
                SetValue(IsDropDownOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected font family. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamily"/>
        /// </value>
        /// <remarks>
        /// Selected font family is displayed in text box of <see cref="Syncfusion.Windows.Tools.Controls.FontListComboBox"/> control.
        /// </remarks>
        /// <seealso cref="FontFamily"/>
        public FontFamily SelectedFontFamily
        {
            get
            {
                return (FontFamily)GetValue(SelectedFontFamilyProperty);
            }

            set
            {
                FontFamily fontFamily = (FontFamily)value;

                if (AllFonts.ContainsName(fontFamily))
                {
                    SetValue(SelectedFontFamilyProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the focused font family. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamily"/>
        /// </value>
        /// <remarks>
        /// Focused font family is displayed in text box of <see cref="Syncfusion.Windows.Tools.Controls.FontListComboBox"/> control.
        /// </remarks>
        /// <seealso cref="FontFamily"/>
        public FontFamily FocusedFontFamily
        {
            get
            {
                return (FontFamily)GetValue(FocusedFontFamilyProperty);
            }

            set
            {
                SetValue(FocusedFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of recently used fonts. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontCollection"/>
        /// </value>
        /// <seealso cref="FontCollection"/>
        /// <example>
        /// FontListComboBox fontListComboBox1 = new FontListComboBox();
        /// this.panel1.Children.Add(fontListComboBox1);
        /// FontFamily font = new FontFamily( "Century" );
        /// fontListComboBox1.RecentlyUsedFonts.Add(font);
        /// </example>
        public FontCollection RecentlyUsedFonts
        {
            get
            {
                return (FontCollection)GetValue(RecentlyUsedFontsProperty);
            }

            set
            {
                SetValue(RecentlyUsedFontsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of theme fonts. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ThemeFontFamilyCollection"/>
        /// </value>
        /// <seealso cref="ThemeFontFamilyCollection"/>
        /// <example>
        /// FontListComboBox fontListComboBox1 = new FontListComboBox();
        /// this.panel1.Children.Add(fontListComboBox1);
        /// ThemeFontFamily font = new ThemeFontFamily("Century", "body");
        /// fontListComboBox1.ThemeFonts.Add(font);
        /// </example>
        public ThemeFontFamilyCollection ThemeFonts
        {
            get
            {
                return (ThemeFontFamilyCollection)GetValue(ThemeFontsProperty);
            }

            set
            {
                SetValue(ThemeFontsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source for the font collection. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Default value is "System".
        /// </value>
        /// <seealso cref="object"/>
        /// <example>
        /// FontListComboBox fontListComboBox1 = new FontListComboBox();
        /// this.panel1.Children.Add(fontListComboBox1);
        /// FontCollection collection = new FontCollection();
        /// collection.Add(new FontFamily("Tahoma"));
        /// collection.Add(new FontFamily("Century"));
        /// collection.Add(new FontFamily("Arial"));
        /// fontListComboBox1.FontsSource = collection;
        /// </example>
        public object FontsSource
        {
            get
            {
                return (object)GetValue(FontsSourceProperty);
            }

            set
            {
                SetValue(FontsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fonts should be displayed in system font. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool DisplayFontNamesInSystemFont
        {
            get
            {
                return (bool)GetValue(DisplayFontNamesInSystemFontProperty);
            }

            set
            {
                SetValue(DisplayFontNamesInSystemFontProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the group header. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        /// <seealso cref="Style"/>
        public Style GroupHeaderStyle
        {
            get
            {
                return (Style)GetValue(GroupHeaderStyleProperty);
            }

            set
            {
                SetValue(GroupHeaderStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template of the item. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// </value>
        /// <seealso cref="DataTemplate"/>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }

            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the item container. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        /// <seealso cref="Style"/>
        public Style ItemContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }

            set
            {
                SetValue(ItemContainerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets pop-up's items foreground. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush ItemsForeground
        {
            get
            {
                return (Brush)GetValue(ItemsForegroundProperty);
            }

            set
            {
                SetValue(ItemsForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets pop-up's height. Default value is Double.NaN. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <remarks>
        /// When the NaN value is assigned, the auto-sizing behavior is enabled.
        /// When the negative value is assigned, it is treated as NaN and, therefore,
        /// the auto-sizing behavior is enabled.
        /// </remarks>
        public double PopupDropDownHeight
        {
            get
            {
                return (double)GetValue(PopupDropDownHeightProperty);
            }

            set
            {
                SetValue(PopupDropDownHeightProperty, value);
            }
        }

        /*
        ///// <summary>
        ///// Gets default foreground. This is a dependency property.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="Brush"/>
        ///// </value>
        //internal Brush DefaultForeground
        //{
        //    get
        //    {
        //        return ( Brush )GetValue( DefaultForegroundProperty );
        //    }
        //    set
        //    {
        //        SetValue( DefaultForegroundProperty, value );
        //    }
        //}
        ///// <summary>
        ///// Gets default background. This is a dependency property.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="Brush"/>
        ///// </value>
        //internal Brush DefaultBackground
        //{
        //    get
        //    {
        //        return (Brush)GetValue(DefaultBackgroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultBackgroundProperty, value);
        //    }
        //}
        ///// <summary>
        ///// Gets default BorderBrush. This is a dependency property.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="Brush"/>
        ///// </value>
        //internal Brush DefaultBorderBrush
        //{
        //    get
        //    {
        //        return (Brush)GetValue(DefaultBorderBrushProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultBorderBrushProperty, value);
        //    }
        //}
        */

        #endregion Properties

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="IsDropDownOpen"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;

        /// <summary>
        /// Event that is raised when <see cref="FontsSource"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FontsSourceChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedFontFamily"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedFontFamilyChanged;

        /// <summary>
        /// Event that is raised when <see cref="RecentlyUsedFonts"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RecentlyUsedFontsChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedFontFamily"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedFontFamilyChanged;

        /// <summary>
        /// Event that is raised when <see cref="ThemeFonts"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ThemeFontsChanged;

        /// <summary>
        /// Event that is raised when <see cref="DisplayFontNamesInSystemFont"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DisplayFontNamesInSystemFontChanged;

        /// <summary>
        /// Event that is raised when <see cref="GroupHeaderStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupHeaderStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemTemplate"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemContainerStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemContainerStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="PopupDropDownHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PopupDropDownHeightChanged;

        #endregion Events

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
              DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(FontListComboBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedFontFamilyProperty =
              DependencyProperty.Register("SelectedFontFamily", typeof(FontFamily), typeof(FontListComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedFontFamilyChanged)));

        /// <summary>
        /// Identifies the <see cref="FocusedFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedFontFamilyProperty =
              DependencyProperty.Register("FocusedFontFamily", typeof(FontFamily), typeof(FontListComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFocusedFontFamilyChanged)));

        /// <summary>
        /// Identifies the <see cref="FontsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontsSourceProperty =
              DependencyProperty.Register("FontsSource", typeof(object), typeof(FontListComboBox), new FrameworkPropertyMetadata("System", new PropertyChangedCallback(OnFontsSourceChanged)));

        /// <summary>
        /// Identifies the <see cref="RecentlyUsedFonts"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecentlyUsedFontsProperty =
              DependencyProperty.Register("RecentlyUsedFonts", typeof(FontCollection), typeof(FontListComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRecentlyUsedFontsChanged), CoerceRecentlyUsedFontsProperty));

        /// <summary>
        /// Identifies the <see cref="ThemeFonts"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeFontsProperty =
              DependencyProperty.Register("ThemeFonts", typeof(ThemeFontFamilyCollection), typeof(FontListComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnThemeFontsChanged), CoerceThemeFontsProperty));

        /// <summary>
        /// Identifies the <see cref="DisplayFontNamesInSystemFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayFontNamesInSystemFontProperty =
              DependencyProperty.Register("DisplayFontNamesInSystemFont", typeof(bool), typeof(FontListComboBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDisplayFontNamesInSystemFontChanged)));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleProperty =
              DependencyProperty.Register("GroupHeaderStyle", typeof(Style), typeof(FontListComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupHeaderStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
              DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(FontListComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
              DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(FontListComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContainerStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="ItemsForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsForegroundProperty =
              DependencyProperty.Register("ItemsForeground", typeof(Brush), typeof(FontListComboBox), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="PopupDropDownHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupDropDownHeightProperty =
          DependencyProperty.Register("PopupDropDownHeight", typeof(double), typeof(FontListComboBox), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnPopupDropDownHeightChanged)));

        /*
        ///// <summary>
        ///// Identifies the <see cref="DefaultForeground"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty DefaultForegroundProperty =
        //    DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(FontListComboBox), new FrameworkPropertyMetadata(Brushes.Black) );
        ///// <summary>
        ///// Identifies the <see cref="DefaultBackground"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty DefaultBackgroundProperty =
        //    DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(FontListComboBox), new FrameworkPropertyMetadata(Brushes.Transparent));
        ///// <summary>
        ///// Identifies the <see cref="DefaultBorderBrush"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty DefaultBorderBrushProperty =
        //    DependencyProperty.Register("DefaultBorderBrush", typeof(Brush), typeof(FontListComboBox), new FrameworkPropertyMetadata(Brushes.Transparent));
        */

        #endregion Dependency Properties

        #region Implementation

        /// <summary>
        /// Is raised by m_openPopup command when user clicks on drop down button.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void ChangeOpenPopupValue(object sender, ExecutedRoutedEventArgs e)
        {
            DependencyPropertyChangedEventArgs ev = new DependencyPropertyChangedEventArgs();
            OnIsDropDownOpenChanged(ev);
        }

        /// <summary>
        /// Invoked when an <see cref="E:System.Windows.Input.Mouse.LostMouseCapture"/> attached event reaches an element.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (Mouse.Captured != this)
            {
                if (e.OriginalSource == this || e.OriginalSource is ToggleButton)
                {
                    if (Mouse.Captured == null)
                    {
                        IsDropDownOpen = false;
                    }
                }
                else
                {
                    if (VisualUtils.IsDescendant((DependencyObject)e.Source, (DependencyObject)e.OriginalSource))
                    {
                        Mouse.Capture(this, CaptureMode.SubTree);
                    }
                }
            }

            base.OnLostMouseCapture(e);
        }

        /// <summary>
        /// Invoked when an <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                IsDropDownOpen = false;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Fulfils the logic before setting the <see cref="ThemeFonts"/> value.
        ///   </summary>
        /// <param name="d">FontListComboBox instance to which this property belongs.</param>
        /// <param name="baseValue">BaseValue that should be set.</param>
        /// <returns>Value that should be set.</returns>
        public static object CoerceThemeFontsProperty(DependencyObject d, object baseValue)
        {
            FontListComboBox fontListComboBox = (FontListComboBox)d;

            return (baseValue != null) ? baseValue : fontListComboBox.m_collectionThemeFontsDefault;
        }

        /// <summary>
        /// Fulfils the logic before setting the <see cref="RecentlyUsedFonts"/> value.
        /// </summary>
        /// <param name="d">FontListComboBox instance to which this property belongs.</param>
        /// <param name="baseValue">BaseValue that should be set.</param>
        /// <returns>Value that should be set.</returns>
        public static object CoerceRecentlyUsedFontsProperty(DependencyObject d, object baseValue)
        {
            FontListComboBox fontListComboBox = (FontListComboBox)d;

            return (baseValue != null) ? baseValue : fontListComboBox.m_collectionRecentlyUsedFontsDefault;
        }

        /// <summary>
        /// Fulfils the logic before setting the <see cref="PopupDropDownHeight"/> value.
        /// </summary>
        /// <param name="d">FontListComboBox instance to which this property belongs.</param>
        /// <param name="baseValue">Value that should be set.</param>
        /// <returns>Coerced Value</returns>
        public static object CoercePopupDropDownHeight(DependencyObject d, object baseValue)
        {
            FontListComboBox fontListComboBox = (FontListComboBox)d;

            double value = (double)baseValue;

            if (value >= 0)
            {
                return baseValue;
            }
            else
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Calls OnIsDropDownOpenChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnIsDropDownOpenChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsDropDownOpenChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDropDownOpen)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }
            else
            {
                Mouse.Capture(null);
            }

            if (IsDropDownOpenChanged != null)
            {
                IsDropDownOpenChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFocusedFontFamilyChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnFocusedFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnFocusedFontFamilyChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FocusedFontFamilyChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnFocusedFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedFontFamilyChanged != null)
            {
                FocusedFontFamilyChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFontsSourceChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnFontsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;

            object newValue = e.NewValue;

            if (newValue is string)
            {
                string path = (String)newValue;

                if (path == SystemFontsSource)
                {
                    instance.SetAllFonts(Fonts.SystemFontFamilies);
                }
                else
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        Uri uri = new Uri(path);
                        instance.SetAllFonts(Fonts.GetFontFamilies(uri));
                    }
                    else
                    {
                        throw new ArgumentException("Path is not valid", "FonstSource");
                    }
                }
            }
            else if (newValue is ICollection<FontFamily>)
            {
                instance.SetAllFonts((ICollection<FontFamily>)newValue);
            }
            else
            {
                throw new Exception("Error   occurred     when try set FontsSourceProperty");
            }

            if (!instance.AllFonts.ContainsName(instance.SelectedFontFamily))
            {
                instance.ClearValue(FontListComboBox.SelectedFontFamilyProperty);
                instance.ClearValue(FontListComboBox.FocusedFontFamilyProperty);
            }

            instance.OnFontsSourceChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FontsSourceChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnFontsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FontsSourceChanged != null)
            {
                FontsSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRecentlyUsedFontsChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnRecentlyUsedFontsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnRecentlyUsedFontsChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RecentlyUsedFontsChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnRecentlyUsedFontsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RecentlyUsedFontsChanged != null)
            {
                RecentlyUsedFontsChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedFontFamilyChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnSelectedFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;

            FontFamily font = instance.SelectedFontFamily;

            if (!(font is ThemeFontFamily))
            {
                if (instance.RecentlyUsedFonts.ContainsName(font))
                {
                    int oldIndex = instance.RecentlyUsedFonts.IndexOf(font);
                    if (oldIndex != -1)
                    {
                        instance.RecentlyUsedFonts.Move(oldIndex, 0);
                    }
                }
                else
                {
                    instance.RecentlyUsedFonts.Insert(0, font);
                }
            }

            instance.OnSelectedFontFamilyChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectedFontFamilyChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnSelectedFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedFontFamilyChanged != null)
            {
                SelectedFontFamilyChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDisplayFontNamesInSystemFontChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnDisplayFontNamesInSystemFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnDisplayFontNamesInSystemFontChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DisplayFontNamesInSystemFontChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDisplayFontNamesInSystemFontChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DisplayFontNamesInSystemFontChanged != null)
            {
                DisplayFontNamesInSystemFontChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnThemeFontsChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnThemeFontsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnThemeFontsChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ThemeFontsChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnThemeFontsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ThemeFontsChanged != null)
            {
                ThemeFontsChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGroupHeaderStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnGroupHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnGroupHeaderStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="GroupHeaderStyleChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnGroupHeaderStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupHeaderStyleChanged != null)
            {
                GroupHeaderStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemTemplateChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnItemTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ItemTemplateChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemTemplateChanged != null)
            {
                ItemTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemContainerStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnItemContainerStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ItemContainerStyleChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemContainerStyleChanged != null)
            {
                ItemContainerStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPopupDropDownHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnPopupDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListComboBox instance = (FontListComboBox)d;
            instance.OnPopupDropDownHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PopupDropDownHeightChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnPopupDropDownHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PopupDropDownHeightChanged != null)
            {
                PopupDropDownHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Sets <see cref="AllFonts"/> property with <see cref="Syncfusion.Windows.Tools.Controls.FontCollection"/> value.
        /// </summary>
        /// <param name="collection">Collection of <see cref="System.Windows.Media.FontFamily"/> objects for
        /// setting the <see cref="AllFonts"/> property.</param>
        private void SetAllFonts(ICollection<FontFamily> collection)
        {
            AllFonts.Clear();
            ThemeFonts.Clear();
            RecentlyUsedFonts.Clear();

            FontCollection temp = new FontCollection();

            foreach (FontFamily family in collection)
            {
                temp.Add(family);
            }

            AllFonts = temp;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //if (SkinStorage.GetVisualStyle(this) == "Default")
            //{
            //    Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            //    if (list1 != null)
            //    {
            //        Shared.DictionaryList list2 = list1["Default"] as Shared.DictionaryList;
            //        if (list2 != null)
            //        {
            //            if (list2.ContainsKey("ForegroundBrush"))
            //            {
            //                Foreground = list2["ForegroundBrush"] as Brush;
            //            }

            //            if (list2.ContainsKey("BackgroundBrush"))
            //            {
            //                Background = list2["BackgroundBrush"] as Brush;
            //            }
            //        }
            //    }
            //}
        }

        /*
        ///// <summary>
        ///// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        ///// </summary>
        ///// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);
        //    if (e.Property == SkinStorage.VisualStyleProperty &&
        //        (string)e.NewValue == "Default")
        //    {
        //        Foreground = m_defaultForeground;
        //        Background = m_defaultBackground;
        //        BorderBrush = m_defaultBorderBrush;
        //    }
        //    else if (e.Property == ForegroundProperty)
        //    {
        //        m_defaultForeground = Foreground;
        //    }
        //    else if (e.Property == BackgroundProperty)
        //    {
        //        m_defaultBackground = Background;
        //    }
        //    else if (e.Property == BorderBrushProperty)
        //    {
        //        m_defaultBorderBrush = BorderBrush;
        //    }
        //}*/

        #endregion Implementation
    }
}