// <copyright file="FontListBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the list of fonts.
    /// </summary>
    /// <para>FontListBox class is handy for working with fonts.</para>
    /// <para>Fonts can be organized in three groups (theme fonts, recently used fonts and all fonts groups).</para>
    /// <para>FontListBox supports 6 Windows themes (Default, Silver, Metallic, Zune, Royale and Aero) and 5 skins
    /// (Office2003, Office2007Blue, Office2007Black, Office2007Silver and Blend). </para>
    /// <para>Also you can define your own user skin by setting properties of the FontListBox class.<para/>
    /// <example>
    /// <para>This example shows how to create a FontListBox in XAML.</para>
    /// <code>
    /// <Window x:Class="FontListSample.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="FontListBox" Height="300" Width="300">
    /// <StackPanel HorizontalAlignment="Center">
    ///     <local:FontListBox Name="FontListBox1"/>
    /// </StackPanel>
    /// </Window>
    /// </code>
    /// <para>This example shows how to create a FontListBox in C#.</para>
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
    ///             FontListBox fontListBox1 = new FontListBox();
    ///             this.panel1.Children.Add( fontListBox1 );
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </para>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
  Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
  Type = typeof(FontListBox), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/TransparentStyle.xaml")]
    public class FontListBox : FontListBase
    {
        internal bool _fontSourceChanged = false;

        #region Constants

        /// <summary>
        /// Constant which represents system fonts source.
        /// </summary>
        private const string SystemFontsSource = "System";

        #endregion Constants

        #region Private fields

        /// <summary>
        /// Cache value for ThemeFonts property.
        /// </summary>
        private ThemeFontFamilyCollection m_collectionThemeFontsDefault = new ThemeFontFamilyCollection();

        /// <summary>
        /// Cache value for RecentlyUsedFonts property.
        /// </summary>
        private FontCollection m_collectionRecentlyUsedFontsDefault = new FontCollection();

        /*
        ///// <summary>
        ///// Default background.
        ///// </summary>
        //private Brush m_defaultBackground = Brushes.Transparent;
        ///// <summary>
        ///// Default foreground.
        ///// </summary>
        //private Brush m_defaultForeground = Brushes.Black;
        ///// <summary>
        ///// Default border brush.
        ///// </summary>
        //private Brush m_defaultBorderBrush = Brushes.Transparent;
        */

        #endregion Private fields

        #region	Initialization

        /// <summary>
        /// Initializes static members of the <see cref="FontListBox"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static FontListBox()
        {
            //EnvironmentTest.ValidateLicense(typeof(FontListBox));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontListBox), new FrameworkPropertyMetadata(typeof(FontListBox)));
            FocusableProperty.OverrideMetadata(typeof(FontListBox), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontListBox"/> class.
        /// </summary>
        public FontListBox()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(FontListBox));
            }
            CoerceValue(ThemeFontsProperty);
            CoerceValue(RecentlyUsedFontsProperty);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            this.Loaded += new RoutedEventHandler(FontListBox_Loaded);
            this.FocusVisualStyle = null;
        }

        #endregion

        private void FontListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_fontSourceChanged)
                CoerceValue(AllFontsProperty);
        }

        #region Properties
        /*
        ///// <summary>
        ///// Gets default background. This is a dependency property.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="Brush"/>
        ///// </value>
        // internal Brush DefaultBackground
        // {
        //    get
        //    {
        //        return (Brush)GetValue(DefaultBackgroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultBackgroundProperty, value);
        //    }
        // }

        ///// <summary>
        ///// Gets default foreground. This is a dependency property.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="Brush"/>
        ///// </value>
        // internal Brush DefaultForeground
        // {
        //    get
        //    {
        //        return (Brush)GetValue(DefaultForegroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultForegroundProperty, value);
        //    }
        // }

        ///// <summary>
        ///// Gets default BorderBrush. This is a dependency property.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="Brush"/>
        ///// </value>
        // internal Brush DefaultBorderBrush
        // {
        //    get
        //    {
        //        return (Brush)GetValue(DefaultBorderBrushProperty);
        //    }
        //    set
        //    {
        //        SetValue(DefaultBorderBrushProperty, value);
        //    }
        // }*/

        /// <summary>
        /// Gets or sets the source for the font collection. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Default value is "System".
        /// </value>
        /// <seealso cref="object"/>
        /// <example>
        /// FontListBox fontListBox1 = new FontListBox();
        /// FontCollection collection = new FontCollection();
        /// collection.Add(new FontFamily("Tahoma"));
        /// collection.Add(new FontFamily("Century"));
        /// collection.Add(new FontFamily("Arial"));
        /// fontListBox1.FontsSource = collection;
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
        /// FontListBox fontListBox1 = new FontListBox();
        /// FontFamily font = new FontFamily( "Century" );
        /// fontListBox1.RecentlyUsedFonts.Add( font );
        /// </example>
        public FontCollection RecentlyUsedFonts
        {
            get
            {
                return (FontCollection)GetValue(RecentlyUsedFontsProperty);
            }

            set
            {
                FontCollection fcl = new FontCollection();
                foreach (FontFamily ff in value)
                {
                    if (!fcl.ContainsName(ff))
                        fcl.Add(ff);
                }
                SetValue(RecentlyUsedFontsProperty, fcl);
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
                if (AllFonts != null)
                    if (AllFonts.ContainsName(fontFamily))
                    {
                        SetValue(SelectedFontFamilyProperty, value);
                    }
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
        /// FontListBox fontListBox1 = new FontListBox();
        /// ThemeFontFamily font1 = new ThemeFontFamily( "Century", "(body)" );
        /// ThemeFontFamily font2 = new ThemeFontFamily( "Arial", "(heading)" );
        /// fontListBox1.ThemeFonts.Add( font1 );
        /// fontListBox1.ThemeFonts.Add( font2 );
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
        /// Gets or sets a value indicating whether control has focus. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool HasFocus
        {
            get
            {
                return (bool)GetValue(HasFocusProperty);
            }

            set
            {
                SetValue(HasFocusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fonts should be displayed like system font. This is a dependency property.
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

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when ListBoxBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback ListBoxBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="FontsSource"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FontsSourceChanged;

        /// <summary>
        /// Event that is raised when <see cref="FocusedFontFamily"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedFontFamilyChanged;

        /// <summary>
        /// Event that is raised when <see cref="RecentlyUsedFonts"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback RecentlyUsedFontsChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedFontFamily"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback SelectedFontFamilyChanged;

        /// <summary>
        /// Event that is raised when <see cref="ThemeFonts"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ThemeFontsChanged;

        /// <summary>
        /// Event that is raised when <see cref="HasFocus"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback HasFocusChanged;

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

        #endregion

        #region	Dependency Properties

        /// <summary>
        /// Identifies the <see cref="FontsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontsSourceProperty =
            DependencyProperty.Register("FontsSource", typeof(object), typeof(FontListBox), new FrameworkPropertyMetadata("System", new PropertyChangedCallback(OnFontsSourceChanged)));

        /// <summary>
        /// Identifies the <see cref="FocusedFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedFontFamilyProperty =
            DependencyProperty.Register("FocusedFontFamily", typeof(FontFamily), typeof(FontListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFocusedFontFamilyChanged)));

        /// <summary>
        /// Identifies the <see cref="RecentlyUsedFonts"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecentlyUsedFontsProperty =
            DependencyProperty.Register("RecentlyUsedFonts", typeof(FontCollection), typeof(FontListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRecentlyUsedFontsChanged), CoerceRecentlyUsedFontsProperty));

        /// <summary>
        /// Identifies the <see cref="SelectedFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedFontFamilyProperty =
            DependencyProperty.Register("SelectedFontFamily", typeof(FontFamily), typeof(FontListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedFontFamilyChanged)));

        /// <summary>
        /// Identifies the <see cref="ThemeFonts"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeFontsProperty =
            DependencyProperty.Register("ThemeFonts", typeof(ThemeFontFamilyCollection), typeof(FontListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnThemeFontsChanged), CoerceThemeFontsProperty));

        /// <summary>
        /// Identifies the <see cref="HasFocus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.Register("HasFocus", typeof(bool), typeof(FontListBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasFocusChanged)));

        /// <summary>
        /// Identifies the <see cref="DisplayFontNamesInSystemFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayFontNamesInSystemFontProperty =
            DependencyProperty.Register("DisplayFontNamesInSystemFont", typeof(bool), typeof(FontListBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDisplayFontNamesInSystemFontChanged)));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleProperty =
            DependencyProperty.Register("GroupHeaderStyle", typeof(Style), typeof(FontListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupHeaderStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(FontListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(FontListBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemContainerStyleChanged)));

        /*
        ///// <summary>
        ///// Identifies the <see cref="DefaultBackground"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty DefaultBackgroundProperty =
        //    DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(FontListBox), new FrameworkPropertyMetadata(Brushes.Transparent));
        ///// <summary>
        ///// Identifies the <see cref="DefaultForeground"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty DefaultForegroundProperty =
        //    DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(FontListBox), new FrameworkPropertyMetadata(Brushes.Black));
        ///// <summary>
        ///// Identifies the <see cref="DefaultBorderBrush"/> dependency property.
        ///// </summary>
        //internal static readonly DependencyProperty DefaultBorderBrushProperty =
        //    DependencyProperty.Register("DefaultBorderBrush", typeof(Brush), typeof(FontListBox), new FrameworkPropertyMetadata(Brushes.Transparent));
        */
        #endregion

        #region	Implementation

        /// <summary>
        /// Calls OnListBoxBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnListBoxBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBox instance = (FontListBox)d;
            instance.OnListBoxBackgroundChanged(e);
        }

        /// <summary>
        /// Raises OnListBoxBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnListBoxBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ListBoxBackgroundChanged != null)
            {
                ListBoxBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Coerces <see cref="ThemeFonts"/> property.
        /// Fulfils the logic before setting the <see cref="ThemeFonts"/> value.
        /// </summary>
        /// <param name="d">FontListBox instance to which this property belongs.</param>
        /// <param name="baseValue">New value.</param>
        /// <returns>Value that should be set.</returns>
        public static object CoerceThemeFontsProperty(DependencyObject d, object baseValue)
        {
            FontListBox fontListBox = (FontListBox)d;

            return (baseValue != null) ? baseValue : fontListBox.m_collectionThemeFontsDefault;
        }

        /// <summary>
        /// Coerces <see cref="RecentlyUsedFonts"/> property.
        /// Fulfils the logic before setting the <see cref="RecentlyUsedFonts"/> value.
        /// </summary>
        /// <param name="d">FontListBox instance to which this property belongs.</param>
        /// <param name="baseValue">New value.</param>
        /// <returns>Value that should be set.</returns>
        public static object CoerceRecentlyUsedFontsProperty(DependencyObject d, object baseValue)
        {
            FontListBox fontListBox = (FontListBox)d;

            return (baseValue != null) ? baseValue : fontListBox.m_collectionRecentlyUsedFontsDefault;
        }

        /// <summary>
        /// Calls OnFontsSourceChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnFontsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBox instance = (FontListBox)d;

            object newValue = e.NewValue;

            if (newValue is string)
            {
                string path = (String)newValue;

                if (path == SystemFontsSource)
                {
                    _fontSource = "System";
                    instance.SetAllFonts(Fonts.SystemFontFamilies);
                }
                else
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        _fontSource = "Path";
                        Uri uri = new Uri(path);
                        instance.SetAllFonts(Fonts.GetFontFamilies(uri));
                    }
                    else
                    {
                        throw new Exception("Error	path is	not	valid");
                    }
                }
            }
            else if (newValue is ICollection<FontFamily>)
            {
                _fontSource = "Collection";
                instance.SetAllFonts((ICollection<FontFamily>)newValue);
            }
            else
            {
                throw new Exception("Error	occurred	when try set FontsSourceProperty");
            }

            if (!instance.AllFonts.ContainsName(instance.SelectedFontFamily))
            {
                instance.ClearValue(FontListBox.SelectedFontFamilyProperty);
                instance.ClearValue(FontListBox.FocusedFontFamilyProperty);
            }

            instance.OnFontsSourceChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FontsSourceChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnFontsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            _fontSourceChanged = true;
            if (FontsSourceChanged != null)
            {
                FontsSourceChanged(this, e);
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
            FontListBox instance = (FontListBox)d;
            instance.OnFocusedFontFamilyChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FocusedFontFamilyChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFocusedFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedFontFamilyChanged != null)
            {
                FocusedFontFamilyChanged(this, e);
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
            FontListBox instance = (FontListBox)d;
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
            FontListBox instance = (FontListBox)d;
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
        /// Calls OnThemeFontsChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnThemeFontsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBox instance = (FontListBox)d;
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
        /// Calls OnDisplayFontNamesInSystemFontChanged method of the
        /// instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnDisplayFontNamesInSystemFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBox instance = (FontListBox)d;
            instance.OnDisplayFontNamesInSystemFontChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DisplayFontNamesInSystemFontChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnDisplayFontNamesInSystemFontChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DisplayFontNamesInSystemFontChanged != null)
            {
                DisplayFontNamesInSystemFontChanged(this, e);
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
            FontListBox instance = (FontListBox)d;
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
            FontListBox instance = (FontListBox)d;
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
            FontListBox instance = (FontListBox)d;
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
        /// Calls OnHasFocusChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnHasFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBox instance = (FontListBox)d;
            instance.OnHasFocusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="HasFocusChanged"/> event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnHasFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasFocusChanged != null)
            {
                HasFocusChanged(this, e);
            }
        }

        /// <summary>
        /// Sets <see cref="AllFonts"/> property with <see cref="Syncfusion.Windows.Tools.Controls.FontCollection"/> value.
        /// </summary>
        /// <param name="collection">Collection of <see cref="System.Windows.Media.FontFamily"/> objects for
        /// setting the <see cref="AllFonts"/> property.</param>
        private void SetAllFonts(ICollection<FontFamily> collection)
        {
            if (AllFonts != null)
                AllFonts.Clear();
            ThemeFonts.Clear();
            RecentlyUsedFonts.Clear();

            FontCollection temp = new FontCollection();
            XmlLanguage userLanguage = XmlLanguage.GetLanguage("en-US");

            if (_fontSource.Equals("Collection"))
            {
                foreach (FontFamily font in collection)
                {
                    try
                    {
                        LanguageSpecificStringDictionary dictionary = font.FamilyNames;
                        if (dictionary.Keys.Contains(userLanguage))
                        {
                            //if (font.FamilyNames[userLanguage] == font.FamilyNames[userLanguage])
                            //{
                            if (!temp.ContainsName(font))
                                temp.Add(font);

                            //}
                        }
                    }
                    catch { }
                }
            }
            else
            {
                foreach (FontFamily font in collection)
                {
                    try
                    {
                        LanguageSpecificStringDictionary dictionary = font.FamilyNames;
                        if (dictionary.Keys.Contains(userLanguage))
                        {
                            //if (font.FamilyNames[userLanguage] == font.FamilyNames[userLanguage])
                            //{
                            temp.Add(font);

                            //}
                        }
                    }
                    catch { }
                }
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
            //            if (list2.ContainsKey("ItemForegroundBrush"))
            //            {
            //                Foreground = list2["ItemForegroundBrush"] as Brush;
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
        //        Background = m_defaultBackground;
        //        Foreground = m_defaultForeground;
        //        BorderBrush = m_defaultBorderBrush;
        //    }
        //    else if (e.Property == BackgroundProperty)
        //    {
        //        m_defaultBackground = Background;
        //    }
        //    else if (e.Property == ForegroundProperty)
        //    {
        //        m_defaultForeground = Foreground;
        //    }
        //    else if (e.Property == BorderBrushProperty)
        //    {
        //        m_defaultBorderBrush = BorderBrush;
        //    }
        //}
        */
        #endregion
    }
}