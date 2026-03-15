#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

#if SILVERLIGHT
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Primitives;
#endif

#if WPF

using Syncfusion.Licensing;
using System.Globalization;

#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Enum for Palette Theme
    /// </summary>
    public enum PaletteTheme
    {
        /// <summary>
        /// To select Office Theme.(Default Theme)
        /// </summary>
        Office,

        /// <summary>
        /// To select GrayScale Theme
        /// </summary>
        Grayscale,

        /// <summary>
        /// To select Apex Theme
        /// </summary>
        Apex,

        /// <summary>
        /// To select Aspect Theme
        /// </summary>
        Aspect,

        /// <summary>
        /// To select Civic Theme
        /// </summary>
        Civic,

        /// <summary>
        /// To select Equity Theme
        /// </summary>
        Equity,

        /// <summary>
        /// To select Flow Theme
        /// </summary>
        Flow,

        /// <summary>
        /// To select Foundary Theme
        /// </summary>
        Foundary,

        /// <summary>
        /// To select Median Theme
        /// </summary>
        Median,

        /// <summary>
        /// To select Metro Theme
        /// </summary>
        Metro,
    }

    /// <summary>
    /// Represents the Enum for BlackWhiteVisible
    /// </summary>
    public enum BlackWhiteVisible
    {
        /// <summary>
        /// To make only Black Visible
        /// </summary>
        Black,

        /// <summary>
        /// To make only White Visible
        /// </summary>
        White,

        /// <summary>
        /// To collapse both Black and White
        /// </summary>
        None,

        /// <summary>
        /// To make both black and White Visible
        /// </summary>
        Both
    }

    /// <summary>
    /// Color Picker(Palette)
    /// </summary>
#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Blend;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Office2007Black;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Office2010Black;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Default;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Office2003;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.VS2010;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Metro;component/ColorPickerPalette.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
   Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Theming.Transparent;component/ColorPickerPalette.xaml")]
#endif

#if WPF

    /// <summary>
    /// TileViewControl Control helps to arrange its children in tile layout. It has built in animaton and drag/drop operations. TileViewItem can be hosted inside the TileViewControl.
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/Office2007Blue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/Office2007Black.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/Office2007Silver.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/Office2003.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
  Type = typeof(ColorPickerPalette), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPickerPalette/Themes/TransparentStyle.xaml")]
#endif
    public class ColorPickerPalette : Control
    {
        #region Dependency Properties

        /// <summary>
        /// Identifies the AutomaticColor dependency property
        /// </summary>
        public static readonly DependencyProperty AutomaticColorProperty = DependencyProperty.Register("AutomaticColor", typeof(Brush), typeof(ColorPickerPalette), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(IsAutomaticColorChanged)));

        /// <summary>
        /// Identifies the <see cref="Color"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorPickerPalette), new PropertyMetadata(Colors.Black, new PropertyChangedCallback(IsColorChanged)));

        /// <summary>
        /// Identifies the <see cref="ThemeHeaderBackGround"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeHeaderBackGroundProperty = DependencyProperty.Register("ThemeHeaderBackGround", typeof(Brush), typeof(ColorPickerPalette), new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 221, 231, 238))));

        /// <summary>
        /// Identifies the <see cref="SetCustomColorsProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty SetCustomColorsProperty = DependencyProperty.Register("SetCustomColors", typeof(bool), typeof(ColorPickerPalette), new PropertyMetadata(false, new PropertyChangedCallback(OnSetCustomColorsChanged)));

        /// <summary>
        /// Identifies the <see cref="CustomColorsCollectionProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomColorsCollectionProperty = DependencyProperty.Register("CustomColorsCollection", typeof(ObservableCollection<CustomColor>), typeof(ColorPickerPalette), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CustomColorsCollectionProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomHeaderVisibilityProperty = DependencyProperty.Register("CustomHeaderVisibility", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="AutomaticColorVisibilityProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty AutomaticColorVisibilityProperty = DependencyProperty.Register("AutomaticColorVisibility", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="MoreColorOptionVisibilityProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty MoreColorOptionVisibilityProperty = DependencyProperty.Register("MoreColorOptionVisibility", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="CustomHeaderTextProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomHeaderTextProperty = DependencyProperty.Register("CustomHeaderText", typeof(string), typeof(ColorPickerPalette), new PropertyMetadata("CustomColors"));

        /// <summary>
        /// Identifies the <see cref="ThemeHeaderForeGround"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeHeaderForeGroundProperty = DependencyProperty.Register("ThemeHeaderForeGround", typeof(Brush), typeof(ColorPickerPalette), new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 21, 110))));

#if WPF
        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ColorPickerPalette));

        internal static readonly DependencyProperty ExpandedMoreColorsBorderPressedProperty = DependencyProperty.Register("ExpandedMoreColorsBorderPressed", typeof(bool), typeof(ColorPickerPalette));
#endif
        internal static readonly DependencyProperty ExpandedAutomaticBorderPressedProperty = DependencyProperty.Register("ExpandedAutomaticBorderPressed", typeof(bool), typeof(ColorPickerPalette), new PropertyMetadata(false));
        internal static readonly DependencyProperty IsAutomaticBorderPressedProperty = DependencyProperty.Register("IsAutomaticBorderPressed", typeof(bool), typeof(ColorPickerPalette), new PropertyMetadata(false));
        internal static readonly DependencyProperty IsMoreColorsBorderPressedProperty = DependencyProperty.Register("IsMoreColorsBorderPressed", typeof(bool), typeof(ColorPickerPalette), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="PopupWidthProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupWidthProperty = DependencyProperty.Register("PopUpWidth", typeof(double), typeof(ColorPickerPalette), new PropertyMetadata(175d));

        /// <summary>
        /// Identifies the <see cref="PopupHeightProperty"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupHeightProperty = DependencyProperty.Register("PopUpHeight", typeof(double), typeof(ColorPickerPalette), new PropertyMetadata(200d, new PropertyChangedCallback(OnPopupHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="BorderWidthProperty"/>  dependency property.
        /// </summary>
        internal static readonly DependencyProperty BorderWidthProperty = DependencyProperty.Register("BorderWidth", typeof(double), typeof(ColorPickerPalette), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="BorderHeightProperty"/>  dependency property.
        /// </summary>
        internal static readonly DependencyProperty BorderHeightProperty = DependencyProperty.Register("BorderHeight", typeof(double), typeof(ColorPickerPalette), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="ColorName"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register("ColorName", typeof(string), typeof(ColorPickerPalette), new PropertyMetadata("Color"));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/>  dependency property.
        /// </summary>
        internal static readonly DependencyProperty ColorGroupItemProperty = DependencyProperty.Register("SelectedItem", typeof(ColorGroupItem), typeof(ColorPickerPalette), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedMoreColor"/>  dependency property.
        /// </summary>
        internal static readonly DependencyProperty MoreColorProperty = DependencyProperty.Register("SelectedMoreColor", typeof(PolygonItem), typeof(ColorPickerPalette), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ThemePanelVisibility"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemePanelVisibilityProperty = DependencyProperty.Register("ThemePanelVisibility", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(ThemeVisibilityChanged)));

        /// <summary>
        /// Identifies the <see cref="StandardPanelVisibility"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty StandardlVisibilityProperty = DependencyProperty.Register("StandardPanelVisibility", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(standard_visibility_changed)));

        /// <summary>
        /// Identifies the <see cref="IsStandardTabVisible"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty IsStandardTabVisibleProperty = DependencyProperty.Register("IsStandardTabVisible", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="IsCustomTabVisible"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCustomTabVisibleProperty = DependencyProperty.Register("IsCustomTabVisible", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="RecentlyUsedPanelVisibility"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty RecentlyUsedProperty = DependencyProperty.Register("RecentlyUsedPanelVisibility", typeof(Visibility), typeof(ColorPickerPalette), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(recentlyusedvisibilitychanged)));

        /// <summary>
        /// Identifies the <see cref="Themes"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Themes", typeof(PaletteTheme), typeof(ColorPickerPalette), new PropertyMetadata(PaletteTheme.Office, new PropertyChangedCallback(ThemeColorChanged)));

        /// <summary>
        /// Identifies the <see cref="BlackWhiteVisibility"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty BlackWhiteVisibleProperty = DependencyProperty.Register("BlackWhiteVisibility", typeof(BlackWhiteVisible), typeof(ColorPickerPalette), new PropertyMetadata(BlackWhiteVisible.None, new PropertyChangedCallback(BlackWhiteVisibilityChanged)));

        /// <summary>
        /// Identifies the <see cref="GenerateThemeVariants"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeVariantsProperty = DependencyProperty.Register("GenerateThemeVariants", typeof(bool), typeof(ColorPickerPalette), new PropertyMetadata(true, new PropertyChangedCallback(ThemeVariantsChanged)));

        /// <summary>
        /// Identifies the <see cref="GenerateStandardVariants"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty StandardVariantsProperty = DependencyProperty.Register("GenerateStandardVariants", typeof(bool), typeof(ColorPickerPalette), new PropertyMetadata(false, new PropertyChangedCallback(StandardVariantsChanged)));

        /// <summary>
        /// Identifies the <see cref="IsExpanded"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ColorPickerPalette), new PropertyMetadata(false, new PropertyChangedCallback(OnIsExpandedChanged)));

        /// <summary>
        ///
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ColorPickerPalette), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public Size IconSize
        {
            get { return (Size)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IconSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(Size), typeof(ColorPickerPalette), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public ImageSource MoreColorsIcon
        {
            get { return (ImageSource)GetValue(MoreColorsIconProperty); }
            set { SetValue(MoreColorsIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MoreColorsIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MoreColorsIconProperty =
            DependencyProperty.Register("MoreColorsIcon", typeof(ImageSource), typeof(ColorPickerPalette), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public Size MoreColorsIconSize
        {
            get { return (Size)GetValue(MoreColorsIconSizeProperty); }
            set { SetValue(MoreColorsIconSizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MoreColorsIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MoreColorsIconSizeProperty =
            DependencyProperty.Register("MoreColorsIconSize", typeof(Size), typeof(ColorPickerPalette), new PropertyMetadata(null));

        /// <summary>
        /// Collection having the set of theme colors
        /// </summary>
        public ObservableCollection<ColorGroupItem>[] col = new ObservableCollection<ColorGroupItem>[16];

        /// <summary>
        /// Collection having the set of Custom colors
        /// </summary>
        public ObservableCollection<ColorGroupItem>[] CustomCol = new ObservableCollection<ColorGroupItem>[20];

        /// <summary>
        /// Collection having Recenly Selected Colors
        /// </summary>
        public ObservableCollection<ColorGroupItem> RecentlyUsedCollection = new ObservableCollection<ColorGroupItem>();

        private ObservableCollection<ColorGroupItem> exp_recentlyusedcollection = new ObservableCollection<ColorGroupItem>();

        /// <summary>
        /// Collection having set of Standard Colors
        /// </summary>
        public ObservableCollection<ColorGroupItem> StdColorCollection;

        /// <summary>
        /// Collection which fetch and load colors for the corresponding panels
        /// </summary>
        public ObservableCollection<ColorGroupItem> ColorGroupCollection = new ObservableCollection<ColorGroupItem>();

        /// <summary>
        /// Collection which fetch and load colors for the corresponding panels
        /// </summary>
        public ObservableCollection<ColorGroupItem> CustomColorGroupCollection = new ObservableCollection<ColorGroupItem>();

        /// <summary>
        /// Event is raised when user changes the color
        /// </summary>
        public event PropertyChangedCallback ColorChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback PopupHeightChanged;

        /// <summary>
        ///
        /// </summary>
        private ColorGroup colorGroup = new ColorGroup();

        /// <summary>
        ///
        /// </summary>
        private ColorGroup colorGroup1 = new ColorGroup();

        #endregion Dependency Properties

        #region Local Variables

        internal bool IsAutomaticSelected;
        internal bool IsSelected = false;
        //To remove Warnings
#if WPF
        internal new bool IsMouseOver = false;
#endif
#if SILVERLIGHT
         internal bool IsMouseOver=false;
#endif
        internal MoreColorsWindow child;
        internal bool IsChecked = false;
        internal bool updownclick = false;
        internal bool IsUpDownSelected;
        internal bool IsColorBorderPressed;
        internal bool IsColorBorderSelected;
        internal bool Isloaded = false;
        internal bool click;
        internal bool Loadfinished;
        internal bool lclick;
        internal bool morecolorsmove;

#if SILVERLIGHT
        internal bool Resize;
          internal bool AtFirst=false;
#endif
        internal Border morecolorsborder;
        internal Border automaticborder;
        private Border autoColorBorder;
        private ColorGroup item1;
        private ColorGroup item2;
        private ColorGroup item3;
        private ColorGroup item4;

        private ColorGroup m_expItem1;
        private ColorGroup m_expItem2;
        private ColorGroup m_expItem3;
        private Border m_expBorder;
#if SILVERLIGHT
        private Button m_expMoreColorsBtn;
#endif
#if WPF
        internal Border m_expMoreColorsBtn;

#endif
        internal bool isPopupclosedOnAutomaticClick = false;
        private Border m_expAutomaticBorder;
        private Border m_expautoBorder;
        private ItemsControl m_expColorAreaBorder;

        private Border updown;
        private Border outborder;
        private Border colorBorder;
        private Border popupBorder;
        private Border OutBorder;
        private Border LayoutBorder;
        internal ItemsControl ColorArea;
        internal Grid Colorgrid;
        private Border color_border;

        #endregion Local Variables

        #region Properties

        /// <summary>
        /// Gets or sets the value of the AutomaticColor dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// AutomaticColor=&quot;Colors.Black&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.AutomaticColor=new SolidColorBrush(Colors.Black);</para>
        /// </remarks>
        /// <value>
        /// Type : Brush
        /// </value>
        public Brush AutomaticColor
        {
            get
            {
                return (Brush)GetValue(AutomaticColorProperty);
            }

            set
            {
                SetValue(AutomaticColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether custom colors is enables or not.
        /// </summary>
        public bool SetCustomColors
        {
            get
            {
                return (bool)GetValue(SetCustomColorsProperty);
            }

            set
            {
                SetValue(SetCustomColorsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for Custom Color Collection.
        /// </summary>
        public ObservableCollection<CustomColor> CustomColorsCollection
        {
            get
            {
                return (ObservableCollection<CustomColor>)GetValue(CustomColorsCollectionProperty);
            }

            set
            {
                SetValue(CustomColorsCollectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Color dependency property
        /// </summary>
        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the PopupWidth dependency property
        /// </summary>
        public double PopupWidth
        {
            get
            {
                return (double)GetValue(PopupWidthProperty);
            }

            set
            {
                SetValue(PopupWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the PopupHeight dependency property
        /// </summary>
        public double PopupHeight
        {
            get
            {
                return (double)GetValue(PopupHeightProperty);
            }

            set
            {
                SetValue(PopupHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>The width of the border.</value>
        public double BorderWidth
        {
            get
            {
                return (double)GetValue(BorderWidthProperty);
            }

            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the border.
        /// </summary>
        /// <value>The height of the border.</value>
        public double BorderHeight
        {
            get
            {
                return (double)GetValue(BorderHeightProperty);
            }

            set
            {
                SetValue(BorderHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ColorName dependency property.
        /// </summary>
        public string ColorName
        {
            get
            {
                return (string)GetValue(ColorNameProperty);
            }

            set
            {
                SetValue(ColorNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the SelectedItem dependency property.
        /// </summary>
        public ColorGroupItem SelectedItem
        {
            get
            {
                return (ColorGroupItem)GetValue(ColorGroupItemProperty);
            }

            internal set
            {
                SetValue(ColorGroupItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ThemeBackGround dependency property.
        /// </summary>
        public Brush ThemeHeaderBackGround
        {
            get
            {
                return (Brush)GetValue(ThemeHeaderBackGroundProperty);
            }

            set
            {
                SetValue(ThemeHeaderBackGroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsAutomaticBorderPressed
        {
            get
            {
                return (bool)GetValue(IsAutomaticBorderPressedProperty);
            }

            private set
            {
                SetValue(IsAutomaticBorderPressedProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsMoreColorsBorderPressed
        {
            get
            {
                return (bool)GetValue(IsMoreColorsBorderPressedProperty);
            }

            private set
            {
                SetValue(IsMoreColorsBorderPressedProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ExpandedAutomaticBorderPressed
        {
            get
            {
                return (bool)GetValue(ExpandedAutomaticBorderPressedProperty);
            }

            private set
            {
                SetValue(ExpandedAutomaticBorderPressedProperty, value);
            }
        }

#if WPF

        public Brush MouseOverBackground
        {
            get
            {
                return (Brush)GetValue(MouseOverBackgroundProperty);
            }

            set
            {
                SetValue(MouseOverBackgroundProperty, value);
            }
        }

        public bool ExpandedMoreColorsBorderPressed
        {
            get
            {
                return (bool)GetValue(ExpandedMoreColorsBorderPressedProperty);
            }

            private set
            {
                SetValue(ExpandedMoreColorsBorderPressedProperty, value);
            }
        }

#endif

        /// <summary>
        /// Gets or sets the value of the ThemeForeGround dependency property.
        /// </summary>
        public Brush ThemeHeaderForeGround
        {
            get
            {
                return (Brush)GetValue(ThemeHeaderForeGroundProperty);
            }

            set
            {
                SetValue(ThemeHeaderForeGroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the SelectedMore dependency property.
        /// </summary>
        internal PolygonItem SelectedMoreColor
        {
            get
            {
                return (PolygonItem)GetValue(MoreColorProperty);
            }

            set
            {
                SetValue(MoreColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the AutomaticColorVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// AutomaticColorVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.AutomaticColorVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility AutomaticColorVisibility
        {
            get
            {
                return (Visibility)GetValue(AutomaticColorVisibilityProperty);
            }

            set
            {
                SetValue(AutomaticColorVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the MoreColorOptionVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// MoreColorOptionVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.MoreColorOptionVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility MoreColorOptionVisibility
        {
            get
            {
                return (Visibility)GetValue(MoreColorOptionVisibilityProperty);
            }

            set
            {
                SetValue(MoreColorOptionVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ThemePanelVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// ThemePanelVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.ThemePanelVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility ThemePanelVisibility
        {
            get
            {
                return (Visibility)GetValue(ThemePanelVisibilityProperty);
            }

            set
            {
                SetValue(ThemePanelVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the CustomHeaderVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// CustomHeaderVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.CustomHeaderVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility CustomHeaderVisibility
        {
            get
            {
                return (Visibility)GetValue(CustomHeaderVisibilityProperty);
            }

            set
            {
                SetValue(CustomHeaderVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for Custom Header text
        /// </summary>
        public string CustomHeaderText
        {
            get
            {
                return (string)GetValue(CustomHeaderTextProperty);
            }

            set
            {
                SetValue(CustomHeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the IsStandardTabVisible dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// IsStandardTabVisible=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.IsStandardTabVisible=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility IsStandardTabVisible
        {
            get
            {
                return (Visibility)GetValue(IsStandardTabVisibleProperty);
            }

            set
            {
                SetValue(IsStandardTabVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the IsCustomTabVisible dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// IsCustomTabVisible=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.IsCustomTabVisible=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility IsCustomTabVisible
        {
            get
            {
                return (Visibility)GetValue(IsCustomTabVisibleProperty);
            }

            set
            {
                SetValue(IsCustomTabVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the StandardPanelVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// StandardPanelVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.StandardPanelVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility StandardPanelVisibility
        {
            get
            {
                return (Visibility)GetValue(StandardlVisibilityProperty);
            }

            set
            {
                SetValue(StandardlVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the RecentlyUsedPanelVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// RecentlyUsedPanelVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.RecentlyUsedPanelVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility RecentlyUsedPanelVisibility
        {
            get
            {
                return (Visibility)GetValue(RecentlyUsedProperty);
            }

            set
            {
                SetValue(RecentlyUsedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Themes dependency property.
        /// </summary>
        /// <value>
        /// Type : Theme
        /// </value>
        public PaletteTheme Themes
        {
            get
            {
                return (PaletteTheme)GetValue(ThemeProperty);
            }

            set
            {
                SetValue(ThemeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Themes dependency property.
        /// </summary>
        /// <value>
        /// Type : BlackWhiteVisible
        /// </value>
        public BlackWhiteVisible BlackWhiteVisibility
        {
            get
            {
                return (BlackWhiteVisible)GetValue(BlackWhiteVisibleProperty);
            }

            set
            {
                SetValue(BlackWhiteVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the GenerateStandardVariants dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// GenerateStandardVariants=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.GenerateStandardVariants=true;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool GenerateStandardVariants
        {
            get
            {
                return (bool)GetValue(StandardVariantsProperty);
            }

            set
            {
                SetValue(StandardVariantsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the GenerateThemeVariants dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// GenerateThemeVariants=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorPickerPalette ColorPicker=new ColorPickerPalette();</para>
        /// <para>ColorPicker.GenerateThemeVariants=true;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool GenerateThemeVariants
        {
            get
            {
                return (bool)GetValue(ThemeVariantsProperty);
            }

            set
            {
                SetValue(ThemeVariantsProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Popup Popup
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Syncfusion.Windows.Tools.Controls.ColorPickerPalette"/> is in Expanded mode or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Creates the instance of ColorPickerPalette control
        /// </summary>
        public ColorPickerPalette()
        {
            DefaultStyleKey = typeof(ColorPickerPalette);
            //DefaultStyleKey = typeof(ColorPickerPalette);
            SetValue(CustomColorsCollectionProperty, new ObservableCollection<CustomColor>());
            this.Loaded += new RoutedEventHandler(ColorPickerPalette_Loaded);
#if SILVERLIGHT
            this.Unloaded += new RoutedEventHandler(ColorPickerPalette_Unloaded);
#endif
#if WPF
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ColorPickerPalette));
            }
#endif
        }

#if SILVERLIGHT
        void ColorPickerPalette_Unloaded(object sender, RoutedEventArgs e)
        {
            this.dispose();
            col = null;
            if (Application.Current != null && Application.Current.RootVisual != null && Application.Current.RootVisual is FrameworkElement)
                ((FrameworkElement)Application.Current.RootVisual).MouseLeftButtonDown -= new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);
            this.Unloaded -= new RoutedEventHandler(ColorPickerPalette_Unloaded);
        }
#endif

        /// <summary>
        /// Initializes the <see cref="ColorPickerPalette"/> class.
        /// </summary>
        static ColorPickerPalette()
        {
#if WPF
            //EnvironmentTest.ValidateLicense(typeof(ColorPickerPalette));
#endif
#if SILVERLIGHT
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }
#endif
        }

        #endregion Initialization

        #region Event

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void DropDownOpenedEventHandler(object sender, RoutedEventArgs args);

        /// <summary>
        ///
        /// </summary>
        public event DropDownOpenedEventHandler DropDownOpened;

        #endregion Event

        #region Overrides

        /// <summary>
        /// Applies the Template for the control
        /// </summary>

        public override void OnApplyTemplate()
        {
            if (m_expAutomaticBorder != null)
            {
                m_expAutomaticBorder.MouseLeave -= new MouseEventHandler(m_expAutomaticBorder_MouseLeave);
                m_expAutomaticBorder.MouseLeftButtonUp -= new MouseButtonEventHandler(m_expAutomaticBorder_MouseLeftButtonUp);
                m_expAutomaticBorder.MouseLeftButtonDown -= new MouseButtonEventHandler(m_expAutomaticBorder_MouseLeftButtonDown);
                m_expAutomaticBorder.MouseMove -= new MouseEventHandler(m_expAutomaticBorder_MouseMove);
            }
            if (automaticborder != null)
            {
                automaticborder.MouseLeftButtonUp -= new MouseButtonEventHandler(automaticborder_MouseLeftButtonUp);
                automaticborder.MouseLeftButtonDown -= new MouseButtonEventHandler(automaticborder_MouseLeftButtonDown);
                automaticborder.MouseMove -= new MouseEventHandler(automaticborder_MouseMove);
                automaticborder.MouseLeave -= new MouseEventHandler(automaticborder_MouseLeave);
            }
            if (m_expMoreColorsBtn != null)
            {
#if SILVERLIGHT
                m_expMoreColorsBtn.Click -= new RoutedEventHandler(m_expMoreColorsBtn_Click);
#endif
#if WPF
                m_expMoreColorsBtn.MouseLeftButtonUp -= new MouseButtonEventHandler(m_expMoreColorsBtn_MouseLeftButtonUp);
                m_expMoreColorsBtn.MouseLeftButtonDown -= new MouseButtonEventHandler(m_expMoreColorsBtn_MouseLeftButtonDown);
                this.MouseLeftButtonUp -= new MouseButtonEventHandler(ColorPickerPalette_MouseLeftButtonUp);
#endif
                m_expMoreColorsBtn.MouseMove -= new MouseEventHandler(m_expMoreColorsBtn_MouseMove);
                m_expMoreColorsBtn.MouseLeave -= new MouseEventHandler(m_expMoreColorsBtn_MouseLeave);
            }
            if (updown != null)
            {
                updown.MouseMove -= new MouseEventHandler(updownMouseMove);
                updown.MouseLeave -= new MouseEventHandler(updownMouseLeave);
                updown.MouseLeftButtonDown -= new MouseButtonEventHandler(updownMouseLeftButtonDown);
            }
            if (colorBorder != null)
            {
                colorBorder.MouseMove -= new MouseEventHandler(ColorBorderMouseMove);
                colorBorder.MouseLeave -= new MouseEventHandler(ColorBorderMouseLeave);
                colorBorder.MouseLeftButtonDown -= new MouseButtonEventHandler(updownMouseLeftButtonDown);
            }
            this.LostFocus -= new RoutedEventHandler(ColorPickerPalette_LostFocus);

            if (morecolorsborder != null)
            {
                morecolorsborder.MouseMove -= new MouseEventHandler(morecolorsborder_MouseMove);
                morecolorsborder.MouseLeave -= new MouseEventHandler(morecolorsborder_MouseLeave);
                morecolorsborder.MouseLeftButtonUp -= new MouseButtonEventHandler(morecolorsborder_MouseLeftButtonUp);
                morecolorsborder.MouseLeftButtonDown -= new MouseButtonEventHandler(morecolorsborder_MouseLeftButtonDown);
            }

            base.OnApplyTemplate();
            OutBorder = GetTemplateChild("ColorPaletteBorder") as Border;

            item1 = GetTemplateChild("item1") as ColorGroup;
            item2 = GetTemplateChild("item2") as ColorGroup;
            item3 = GetTemplateChild("item3") as ColorGroup;
            ColorArea = GetTemplateChild("ColorArea") as ItemsControl;
            automaticborder = GetTemplateChild("Automatic1") as Border;
            autoColorBorder = GetTemplateChild("aborder") as Border;
            morecolorsborder = GetTemplateChild("MoreColors1") as Border;
            popupBorder = GetTemplateChild("b") as Border;
            item4 = GetTemplateChild("item4") as ColorGroup;
            color_border = GetTemplateChild("color_border") as Border;

            Image icon = GetTemplateChild("image") as Image;
            if (icon != null)
            {
                if (icon.Source == null)
                    color_border.Margin = new Thickness(4);
            }

            m_expItem1 = GetTemplateChild("item1_Expanded") as ColorGroup;
            m_expItem2 = GetTemplateChild("item2_Expanded") as ColorGroup;
            m_expItem3 = GetTemplateChild("item3_Expanded") as ColorGroup;
            m_expColorAreaBorder = GetTemplateChild("ColorArea_Expanded") as ItemsControl;
            m_expAutomaticBorder = GetTemplateChild("Automatic1_Expanded") as Border;
            m_expautoBorder = GetTemplateChild("aborder_Expanded") as Border;
            m_expBorder = GetTemplateChild("b_Expanded") as Border;
#if SILVERLIGHT
            m_expMoreColorsBtn = GetTemplateChild("MoreColors1_Expanded") as Button;
#endif
#if WPF
            m_expMoreColorsBtn = GetTemplateChild("MoreColors1_Expanded") as Border;
#endif
            if (m_expBorder != null)
            {
                m_expBorder.Width = 175;
#if WPF
                if (SkinStorage.GetEnableTouch(this))
                    m_expBorder.Width = 300;
#endif
            }
            //if (item1 != null)
            //{
            //    item1.HeaderName = Themes.ToString() + " Theme";

            //}
            exp_recentlyusedcollection.Clear();
            RecentlyUsedCollection.Clear();
            ThemeColors();
            StandardColors();
            RecentlyUsed();

            if (ColorArea != null && !ColorArea.Items.Contains(colorGroup) && m_expColorAreaBorder != null && !m_expColorAreaBorder.Items.Contains(colorGroup1))
            {
                colorGroup = new ColorGroup();
                colorGroup1 = new ColorGroup();
            }

            ColorToBrushConverter conv = new ColorToBrushConverter();
            Binding binding = new Binding("Color");
            binding.Source = this;
            binding.Converter = conv;
            if (color_border != null)
            {
                color_border.SetBinding(Border.BackgroundProperty, binding);

                m_expAutomaticBorder.MouseLeave += new MouseEventHandler(m_expAutomaticBorder_MouseLeave);
                m_expAutomaticBorder.MouseLeftButtonUp += new MouseButtonEventHandler(m_expAutomaticBorder_MouseLeftButtonUp);
                m_expAutomaticBorder.MouseLeftButtonDown += new MouseButtonEventHandler(m_expAutomaticBorder_MouseLeftButtonDown);
                m_expAutomaticBorder.MouseMove += new MouseEventHandler(m_expAutomaticBorder_MouseMove);

                Button automaticbutton = GetTemplateChild("Automatic") as Button;
                automaticborder.MouseLeftButtonUp += new MouseButtonEventHandler(automaticborder_MouseLeftButtonUp);
                automaticborder.MouseLeftButtonDown += new MouseButtonEventHandler(automaticborder_MouseLeftButtonDown);
                automaticborder.MouseMove += new MouseEventHandler(automaticborder_MouseMove);
                automaticborder.MouseLeave += new MouseEventHandler(automaticborder_MouseLeave);

                if (m_expMoreColorsBtn != null)
                {
#if SILVERLIGHT
                m_expMoreColorsBtn.Click += new RoutedEventHandler(m_expMoreColorsBtn_Click);
#endif
#if WPF
                    m_expMoreColorsBtn.MouseLeftButtonUp += new MouseButtonEventHandler(m_expMoreColorsBtn_MouseLeftButtonUp);
                    m_expMoreColorsBtn.MouseLeftButtonDown += new MouseButtonEventHandler(m_expMoreColorsBtn_MouseLeftButtonDown);
#endif
                    m_expMoreColorsBtn.MouseMove += new MouseEventHandler(m_expMoreColorsBtn_MouseMove);
                    m_expMoreColorsBtn.MouseLeave += new MouseEventHandler(m_expMoreColorsBtn_MouseLeave);
                }

                Colorgrid = GetTemplateChild("lay") as Grid;
                Popup = GetTemplateChild("pop") as Popup;
                outborder = GetTemplateChild("OutBorder") as Border;
                updown = GetTemplateChild("UpDownBorder") as Border;
                if (updown != null)
                {
                    updown.MouseMove += new MouseEventHandler(updownMouseMove);
                    updown.MouseLeave += new MouseEventHandler(updownMouseLeave);
                }
                LayoutBorder = GetTemplateChild("ColorPickerBorder") as Border;

                if (updown != null)
                    updown.MouseLeftButtonDown += new MouseButtonEventHandler(updownMouseLeftButtonDown);
                colorBorder = GetTemplateChild("ColorBorder") as Border;
                colorBorder.MouseMove += new MouseEventHandler(ColorBorderMouseMove);
                colorBorder.MouseLeave += new MouseEventHandler(ColorBorderMouseLeave);
                colorBorder.MouseLeftButtonDown += new MouseButtonEventHandler(colorBorder_MouseLeftButtonDown);
                colorBorder.MouseLeftButtonUp += new MouseButtonEventHandler(colorBorder_MouseLeftButtonUp);
#if SILVERLIGHT
            //this.Unloaded += new RoutedEventHandler(ColorPickerPalette_Unloaded);
#endif
#if WPF
                this.MouseLeftButtonUp += new MouseButtonEventHandler(ColorPickerPalette_MouseLeftButtonUp);
#endif
                this.LostFocus += new RoutedEventHandler(ColorPickerPalette_LostFocus);
#if WPF
                this.Unloaded += new RoutedEventHandler(ColorPickerPalette_Unloaded);
#endif
#if SILVERLIGHT
            IEnumerable<DependencyObject> rootVisualChildren = Application.Current.RootVisual.GetVisualChildren();
            foreach (DependencyObject obj in rootVisualChildren)
            {
                if (obj is Panel)
                {
                    if ((obj as Panel).Background == null)
                    {
                        (obj as Panel).Background = new SolidColorBrush(Colors.Transparent);
                    }
                }
                else if (obj is Border)
                {
                    if ((obj as Border).Background == null)
                    {
                        (obj as Border).Background = new SolidColorBrush(Colors.Transparent);
                    }
                }
            }
#endif
#if WPF
                //this.MouseLeftButtonDown += new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);

                //Style style = new Style();
                //style = Colorgrid.Resources["ch1"] as Style;
                exp_child = new MoreColorsWindow();
#endif
                //if(style!=null)
                //    child.Style = style;

                if (morecolorsborder != null)
                {
                    morecolorsborder.MouseMove += new MouseEventHandler(morecolorsborder_MouseMove);
                    morecolorsborder.MouseLeave += new MouseEventHandler(morecolorsborder_MouseLeave);
                    morecolorsborder.MouseLeftButtonUp += new MouseButtonEventHandler(morecolorsborder_MouseLeftButtonUp);
                    morecolorsborder.MouseLeftButtonDown += new MouseButtonEventHandler(morecolorsborder_MouseLeftButtonDown);
                }
                if (IsExpanded)
                {
                    if (m_expBorder != null)
                    {
                        m_expBorder.Visibility = Visibility.Visible;
                    }
                    if (OutBorder != null)
                        OutBorder.Visibility = Visibility.Collapsed;
                    LoadInExpandedMode(true);
                    //Rough();
                    if (this.Color.Equals(((SolidColorBrush)this.AutomaticColor).Color))
                    {
                        VisualStateManager.GoToState(this, "MouseOverAutomatic", true);
                    }
                }
            }
        }

#if WPF

        private void m_expMoreColorsBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExpandedMoreColorsBorderPressed = true;
        }

#endif

        private void m_expAutomaticBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExpandedAutomaticBorderPressed = true;
        }

        private void morecolorsborder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMoreColorsBorderPressed = true;
        }

        private void automaticborder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsAutomaticBorderPressed = true;
        }

        //void ColorPickerPalette_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    this.child.Close();
        //}

        private void colorBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsColorBorderPressed = false;
            this.IsColorBorderSelected = true;
            UpdateVisualState(true);
        }

#if WPF

        private void ColorPickerPalette_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (!isPopupclosedOnAutomaticClick && !IsExpanded)
            //{
            //    OpenPopup();
            //
            //}
            //isPopupclosedOnAutomaticClick = false;
        }

#endif
#if WPF

        private void ColorPickerPalette_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.child != null)
            {
                this.child.Close();
            }
            if (this.exp_child != null)
            {
                this.exp_child.Close();
            }

            this.Unloaded -= new RoutedEventHandler(ColorPickerPalette_Unloaded);
        }

#endif

        private void colorBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsColorBorderPressed = true;
            UpdateVisualState(true);
#if WPF
            if (!isPopupclosedOnAutomaticClick && !IsExpanded)
            {
                OpenPopup();
            }
            isPopupclosedOnAutomaticClick = false;
#endif
        }

        /// <summary>
        /// This method get called during measure pass of the layout system.
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsExpanded)
            {
                //LoadInExpandedMode(true);
            }
            try
            {
                return base.MeasureOverride(availableSize);
            }
            catch { return availableSize; }
        }

        #endregion Overrides

        #region Implementation

        /// <summary>
        /// Handles the MouseLeave event of the m_expMoreColorsBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_expMoreColorsBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.morecolorsmove = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Handles the MouseMove event of the m_expMoreColorsBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_expMoreColorsBtn_MouseMove(object sender, MouseEventArgs e)
        {
            this.morecolorsmove = true;
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Method called when mouse leave occurs in Morecolors border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEvent</param>
        private void morecolorsborder_MouseLeave(object sender, MouseEventArgs e)
        {
            this.morecolorsmove = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Method called when mouse move occurs in Morecolors border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEvent</param>
        private void morecolorsborder_MouseMove(object sender, MouseEventArgs e)
        {
            this.morecolorsmove = true;
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Handles the MouseMove event of the m_expAutomaticBorder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_expAutomaticBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Color.Equals(((SolidColorBrush)this.AutomaticColor).Color))
            {
                VisualStateManager.GoToState(this, "MouseOverAutomatic", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the m_expAutomaticBorder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_expAutomaticBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Color.Equals(((SolidColorBrush)this.AutomaticColor).Color))
            {
                VisualStateManager.GoToState(this, "MouseOverAutomatic", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        /// <summary>
        ///  Method called when mouse leave occurs in Automatic border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEvent</param>
        private void automaticborder_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Method called when mouse move occurs in Automatic border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEvent</param>
        private void automaticborder_MouseMove(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = true;
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Method called when automatic border is clicked
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseButtonEvent</param>
        private void automaticborder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPopupclosedOnAutomaticClick = true;
            IsAutomaticBorderPressed = false;
            this.ColorName = "Automatic color";
            IsAutomaticSelected = true;
            this.Color = ((SolidColorBrush)autoColorBorder.Background).Color;

            this.IsChecked = false;
            this.IsColorBorderSelected = false;
            this.IsUpDownSelected = false;
            this.IsSelected = true;
            this.UpdateVisualState(true);
            if (Popup != null)
                Popup.IsOpen = false;
            dispose();
            if (SelectedItem != null)
            {
                SelectedItem.IsSelected = false;
                SelectedItem.UpdateVisualState(false);
                SelectedItem = null;
            }
        }

#if WPF
        private MoreColorsWindow exp_child;
#endif
        /// <summary>
        /// Handles the Click event of the m_expMoreColorsBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
#if SILVERLIGHT
 private void m_expMoreColorsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenMoreColorsWindow();
        }
#endif
#if WPF

        private void m_expMoreColorsBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ExpandedMoreColorsBorderPressed = false;
            OpenMoreColorsWindow();
        }

#endif

#if WPF
        internal string themestyle;
#endif

        /// <summary>
        /// This method is raised when the MoreColors button is clicked
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEvent</param>
        private void morecolorsborder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPopupclosedOnAutomaticClick = true;
            IsMoreColorsBorderPressed = false;
            OpenMoreColorsWindow();
        }

        /// <summary>
        /// Opens the more colors window.
        /// </summary>
        private void OpenMoreColorsWindow()
        {
            child = new MoreColorsWindow();
            child.palette = this;
            if (Popup != null)
                Popup.IsOpen = false;
            dispose();

            int flag = 1;
            child.palette = this;
#if SILVERLIGHT
            child.OverlayOpacity = 1;
            child.OverlayBrush = new SolidColorBrush(Colors.Transparent);
#endif
#if WPF
            child.Opacity = 1;
#endif
            child.polygonitem = null;
            var obj = from more in child.morecolorcollection where ((SolidColorBrush)more.color).Color.Equals(this.Color) == true select more;
            int i = 0;
            foreach (PolygonItem asde in obj)
            {
                SelectedMoreColor = asde;
                child.polygonitem = asde;
                if (i == 0)
                {
                    child.path.Stroke = new SolidColorBrush(Colors.Black);
                    child.path.Fill = new SolidColorBrush(Colors.White);
                    child.path.Data = child.polygonitem.DrawPath(child.polygonitem.Points);
                    i = 1;
                }
                else
                {
                    child.path1.Stroke = new SolidColorBrush(Colors.Black);
                    child.path1.Fill = new SolidColorBrush(Colors.White);
                    child.path1.Data = child.polygonitem.DrawPath(child.polygonitem.Points);
                }

                Binding bind = new Binding();
                bind.Source = SelectedMoreColor.color;
                bind.Mode = BindingMode.OneWay;
                child.Current.SetBinding(Border.BackgroundProperty, bind);
                child.New.SetBinding(Border.BackgroundProperty, bind);
                flag = 0;
            }

            if (flag == 1)
            {
                child.polygonitem = null;
                this.SelectedMoreColor = null;

                child.tab.SelectedIndex = 1;

                child.path.Data = null;
                child.path1.Data = null;
                child.Current.Background = new SolidColorBrush(this.Color);
                child.New.Background = new SolidColorBrush(this.Color);
            }
            else
            {
                child.tab.SelectedIndex = 0;
            }
            if (child.tab.SelectedIndex == 0 && IsStandardTabVisible != Visibility.Visible)
            {
                child.tab.SelectedIndex = 1;
            }
            if (child.tab.SelectedIndex == 1 && IsCustomTabVisible != Visibility.Visible)
            {
                child.tab.SelectedIndex = 0;
            }
            if (SelectedMoreColor != null)
            {
                if (((SolidColorBrush)SelectedMoreColor.color).Color != Colors.White)
                {
                    child.path1.Data = null;
                }
            }
#if SILVERLIGHT
            child.asd.SelectedBrush = new SolidColorBrush(this.Color);
#endif
#if WPF
            child.asd.Brush = new SolidColorBrush(this.Color);
#endif
            this.IsChecked = false;
            this.IsColorBorderSelected = false;
            this.IsUpDownSelected = false;
            this.UpdateVisualState(false);
            child.custompanel.Visibility = IsCustomTabVisible;
            child.custom.Visibility = IsCustomTabVisible;
            child.standard.Visibility = IsStandardTabVisible;
            child.standardPanel.Visibility = IsStandardTabVisible;
            child.WindowGrid.Height = 350d;
            child.hyp = 10;
            child.x = 90d;
            child.y = 20d;
            child.WindowGrid.Width = 424d;
#if WPF
            themestyle = SkinStorage.GetVisualStyle(this);
            if (themestyle != "")
            {
                SkinStorage.SetVisualStyle(this.child, themestyle);
            }
#endif
            //Style style = new Style();
            //style = Colorgrid.Resources["ch1"] as Style;

            //child = new MoreColorsWindow();
            //if (style != null)
            //    child.Style = style;
#if SILVERLIGHT
            child.Show();
#endif
#if WPF
            child.ShowActivated = true;
            child.ShowDialog();

#endif
        }

        /// <summary>
        /// Method called when control's focus is lost
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Routed event args</param>
        private void ColorPickerPalette_LostFocus(object sender, RoutedEventArgs e)
        {
            if (leave)
            {
                if (Popup != null)
                    Popup.IsOpen = false;
                dispose();
                IsChecked = false;
                //updownclick = false;
                this.IsColorBorderSelected = false;
                this.IsUpDownSelected = false;
                this.UpdateVisualState(false);
            }
            if (lclick)
                lclick = false;
            if (!leave)
            {
                //this.Focus();
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called when [is expanded changed].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsExpandedChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            ColorPickerPalette instance = target as ColorPickerPalette;
            if (instance != null)
            {
                if (instance.m_expBorder != null)
                {
                    //instance.Rough();
                    if ((bool)args.NewValue)
                    {
                        instance.LoadInExpandedMode(true);
                        if (instance.Color.Equals(((SolidColorBrush)instance.AutomaticColor).Color))
                        {
                            VisualStateManager.GoToState(instance, "MouseOverAutomatic", true);
                        }
                        instance.m_expBorder.Visibility = Visibility.Visible;
                        instance.OutBorder.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        instance.LoadInExpandedMode(false);
                        instance.m_expBorder.Visibility = Visibility.Collapsed;
                        if (instance.OutBorder != null)
                            instance.OutBorder.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Method called when click event occurs in applications controls
        /// </summary>
        /// <param name="sender">Object of teh sender</param>
        /// <param name="e">Handles Mouse button Event args</param>
        private void e_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Loadfinished)
            {
                click = false;
                Loadfinished = true;
            }
            if (Popup.IsOpen && !click)
            {
                Popup.IsOpen = false;
                dispose();
                IsChecked = false;
                updownclick = false;
                this.IsColorBorderSelected = false;
                this.IsUpDownSelected = false;
                this.UpdateVisualState(false);
                e.Handled = true;
            }
            if (click)
            {
                click = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called to dispatch the click event of the element's parents
        /// </summary>
        internal void dispose()
        {
            FrameworkElement e2 = this.Parent as FrameworkElement;
            FrameworkElement e1 = this.Parent as FrameworkElement;
            while (e2 != null)
            {
                e1 = e2;

                e1.MouseLeftButtonDown -= new MouseButtonEventHandler(e_MouseLeftButtonDown);
                e2 = VisualTreeHelper.GetParent(e2) as FrameworkElement;
                Loadfinished = false;
            }
        }

        /// <summary>
        /// Called when Root Visual element is clicked.
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles MouseEventArgs </param>
        private void RootVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!updownclick)
            {
                Popup.IsOpen = false;
                dispose();
                IsChecked = false;
                this.IsColorBorderSelected = false;
                this.IsUpDownSelected = false;
                if (!IsExpanded)
                    this.UpdateVisualState(false);
            }

            updownclick = false;
        }

        /// <summary>
        /// This method is raised when colorborder is pressed
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEventArgs</param>
        private void ColorBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup.IsOpen = false;
            dispose();
            IsChecked = false;
            IsUpDownSelected = false;
            IsColorBorderSelected = false;
            this.UpdateVisualState(false);
        }

        internal double RefWidth;
        internal bool widthchanged = false;

        /// <summary>
        /// Loads the in expanded mode.
        /// </summary>
        /// <param name="isexpanded">if set to <c>true</c> [isexpanded].</param>
        private void LoadInExpandedMode(bool isexpanded)
        {
            double defaultwidth = 175;
#if WPF
            if (SkinStorage.GetEnableTouch(this))
            {
                defaultwidth = 300;
                PopupWidth = 300;
            }
#endif
            if (!double.IsNaN(Width))
            {
                if (m_expBorder != null)
                {
                    m_expBorder.Width = double.NaN;
                }
                defaultwidth = Width;
            }

            if (isexpanded)
            {
                if (LayoutBorder != null)
                {
                    if (BlackWhiteVisibility == BlackWhiteVisible.None)
                    {
                        // width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 7) + 6)) / 8);
                        width = Math.Floor((defaultwidth - ((4 * 7) + 6)) / 8);
                    }
                    else if (BlackWhiteVisibility == BlackWhiteVisible.Both)
                    {
                        //width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 9) + 6)) / 10);
                        width = Math.Floor((defaultwidth - ((4 * 9) + 6)) / 10);
                    }
                    else
                    {
                        //width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 8) + 6)) / 9);
                        width = Math.Floor((defaultwidth - ((4 * 8) + 6)) / 9);
                    }
                }
            }
            else
            {
                if (LayoutBorder != null)
                {
                    if (BlackWhiteVisibility == BlackWhiteVisible.None)
                    {
                        // width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 7) + 6)) / 8);
                        width = Math.Floor((PopupWidth - ((4 * 7) + 6)) / 8);
                    }
                    else if (BlackWhiteVisibility == BlackWhiteVisible.Both)
                    {
                        //width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 9) + 6)) / 10);
                        width = Math.Floor((PopupWidth - ((4 * 9) + 6)) / 10);
                    }
                    else
                    {
                        //width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 8) + 6)) / 9);
                        width = Math.Floor((PopupWidth - ((4 * 8) + 6)) / 9);
                    }
                }
            }
            width = width < 0 ? 0 : width;
            RefWidth = width;
            height = width;
            BorderHeight = BorderHeight == 0.0 ? height : BorderHeight;
            BorderWidth = BorderWidth == 0.0 ? width : BorderWidth;
            this.ThemeColors();
            this.StandardColors();
            this.RecentlyUsed();
            if (SetCustomColors)
            {
                this.LoadCustomColors();
            }
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// This method is raised when updownborder is pressed
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEventArgs</param>
        private void updownMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if !WPF
            OpenPopup();
#else
            if (!isPopupclosedOnAutomaticClick && !IsExpanded)
            {
                OpenPopup();
            }
            isPopupclosedOnAutomaticClick = false;
#endif
        }

        private void OpenPopup()
        {
            this.Focus();
            updownclick = true;
            lclick = true;
            if (Popup != null)
            {
                if (width != RefWidth)
                {
                    width = RefWidth;
                    widthchanged = true;
                }
                else
                {
                    widthchanged = false;
                }
                morecolorsmove = false;
                this.UpdateVisualState(false);
                if (!this.IsChecked)
                {
                    this.IsChecked = true;
                }
                else
                {
                    this.IsChecked = false;
                }

                if (IsChecked)
                {
                    //Loadfinished = true;
                    if (!this.IsExpanded)
                        Popup.IsOpen = true;
#if SILVERLIGHT
                FrameworkElement e2 = this.Parent as FrameworkElement;
                FrameworkElement e1 = this.Parent as FrameworkElement;
                while (e2 != null)
                {
                    //e.MouseLeftButtonDown += new MouseButtonEventHandler(e_MouseLeftButtonDown);

                    e1 = e2;
                    // MessageBox.Show(e.ToString());
                    //e = e.Parent as FrameworkElement;
                    e1.MouseLeftButtonDown += new MouseButtonEventHandler(e_MouseLeftButtonDown);
                    e2 = VisualTreeHelper.GetParent(e2) as FrameworkElement;

                    // MessageBox.Show(e.ToString());
                }

                GeneralTransform gt;
                Point offset;
                //popupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#endif
                    Size size = new Size(175, 200);
#if SILVERLIGHT
                Popup.Visibility = Visibility.Collapsed;
                gt = Popup.TransformToVisual(Application.Current.RootVisual as UIElement);
                offset = gt.Transform(new Point(0, 0));
#endif
                    Popup.Margin = new Thickness(0, (OutBorder.ActualHeight), 0, 0);
#if WPF
                    if (SkinStorage.GetEnableTouch(this))
                        PopupWidth = 300;
#endif
                    if (width == 0)
                    {
                        if (LayoutBorder != null)
                        {
                            if (BlackWhiteVisibility == BlackWhiteVisible.None)
                            {
                                // width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 7) + 6)) / 8);
                                width = Math.Floor((PopupWidth - ((4 * 7) + 6)) / 8);
                            }
                            else if (BlackWhiteVisibility == BlackWhiteVisible.Both)
                            {
                                //width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 9) + 6)) / 10);
                                width = Math.Floor((PopupWidth - ((4 * 9) + 6)) / 10);
                            }
                            else
                            {
                                //width = Math.Floor((LayoutBorder.ActualWidth * 4 - ((4 * 8) + 6)) / 9);
                                width = Math.Floor((PopupWidth - ((4 * 8) + 6)) / 9);
                            }
                        }
                        width = width < 0 ? 0 : width;
                        RefWidth = width;
                        height = width;
                        BorderHeight = BorderHeight == 0.0 ? height : BorderHeight;
                        BorderWidth = BorderWidth == 0.0 ? width : BorderWidth;
                        this.ThemeColors();
                        this.StandardColors();
                        this.RecentlyUsed();
                        if (SetCustomColors)
                        {
                            this.LoadCustomColors();
                        }
                    }
                    else if (widthchanged)
                    {
                        height = width;
                        this.ThemeColors();
                        this.StandardColors();
                        this.RecentlyUsed();
                        if (SetCustomColors)
                        {
                            this.LoadCustomColors();
                        }
                    }
                    //popupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
#if SILVERLIGHT
                size = popupBorder.DesiredSize;

                Border popupchild = popupBorder;
                if (size.Width > Application.Current.Host.Content.ActualWidth)
                {
                    if (BlackWhiteVisibility == BlackWhiteVisible.None)
                    {
                        width = Math.Floor((Application.Current.Host.Content.ActualWidth - ((4 * 7) + 6)) / 8);
                    }
                    else if (BlackWhiteVisibility == BlackWhiteVisible.Both)
                    {
                        width = Math.Floor((Application.Current.Host.Content.ActualWidth - ((4 * 9) + 6)) / 10);
                    }
                    else
                    {
                        width = Math.Floor((Application.Current.Host.Content.ActualWidth - ((4 * 8) + 6)) / 9);
                    }
                    width = width < 0 ? 0 : width;
                    height = width;
                    this.ThemeColors();
                    this.StandardColors();
                    this.RecentlyUsed();
                    if (SetCustomColors)
                    {
                        this.LoadCustomColors();
                    }
                }
                while (!(size.Height < Application.Current.Host.Content.ActualHeight))
                {
                    width--;
                    height = width;
                    this.ThemeColors();
                    this.StandardColors();
                    this.RecentlyUsed();
                    //popupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    size = popupBorder.DesiredSize;
                    Resize = true;
                }
                if (offset.X + size.Width > Application.Current.Host.Content.ActualWidth)
                {
                    Popup.HorizontalOffset = -((offset.X + size.Width) - Application.Current.Host.Content.ActualWidth);
                }
                if (!AtFirst)
                {
                    offset.Y += LayoutBorder.ActualHeight;
                }
                if (offset.Y + size.Height > Application.Current.Host.Content.ActualHeight)
                {
                    Popup.VerticalOffset = -(PopupHeight - ActualHeight + ((this.ActualHeight)));
                }
                Popup.IsOpen = true;
                Popup.Visibility = Visibility.Visible;
                GeneralTransform objTransform = this.TransformToVisual(Application.Current.RootVisual as UIElement);
                Point point = objTransform.Transform(new Point(0, 0));
                double left = point.X;
                double top = point.Y;
                double opp = top - ActualHeight;
                double totalheight = top + PopupHeight;
                double totalwidth = left + PopupWidth;

                if (totalheight >= Application.Current.Host.Content.ActualHeight || totalheight >= 800)
                {
                    Popup.Margin = new Thickness(0, -(OutBorder.ActualHeight + PopupHeight), 0, 0);
                    double pop_top = Popup.Margin.Top;
                    double c_bottom = this.Margin.Bottom;
                    double val = c_bottom + OutBorder.ActualHeight;
                    double diff = pop_top + PopupHeight;
                    if (!AtFirst)
                    {
                        Popup.Margin = new Thickness(0, (pop_top - diff + 52), 0, 0);
                    }
                    if(this.IsSelected == false)
                        Popup.Margin = new Thickness(0, (pop_top - diff + 24), 0, 0);
                }
                if (totalwidth > Application.Current.Host.Content.ActualWidth)
                {
                    Popup.HorizontalOffset = -(totalwidth - Application.Current.Host.Content.ActualWidth);
                }
#endif
                    if (!this.IsExpanded)
                        Popup.IsOpen = true;
                    click = true;
#if WPF
                    Popup.Placement = PlacementMode.Bottom;
#endif
                }
                else
                {
                    Popup.IsOpen = false;
                    dispose();
                }
                IsUpDownSelected = true;
                this.UpdateVisualState(true);
                this.Focus();
                if (this.DropDownOpened != null)
                    this.DropDownOpened(this, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// This method is raised when mouse leaves  Color border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEventArgs</param>
        private void ColorBorderMouseLeave(object sender, MouseEventArgs e)
        {
            IsColorBorderPressed = false;
            if (!IsChecked)
            {
                IsUpDownSelected = false;
                IsColorBorderSelected = false;
                this.UpdateVisualState(false);
            }
        }

        /// <summary>
        /// Loads the custom colors.
        /// </summary>
        private void LoadCustomColors()
        {
            if (CustomColorsCollection != null && ColorArea != null && !ColorArea.Items.Contains(colorGroup) && m_expColorAreaBorder != null && !m_expColorAreaBorder.Items.Contains(colorGroup1))
            {
                ObservableCollection<ColorGroupItem> coll3 = new ObservableCollection<ColorGroupItem>();
                ObservableCollection<ColorGroupItem> coll3_1 = new ObservableCollection<ColorGroupItem>();

                colorGroup.HeaderName = this.CustomHeaderText;
                colorGroup.HeaderVisibility = this.CustomHeaderVisibility;
                colorGroup1.HeaderName = this.CustomHeaderText;
                colorGroup1.HeaderVisibility = this.CustomHeaderVisibility;
                // item4.colorGroupItemsControl.ItemsPanel = Colorgrid.Resources["ItemPanelTemplate2"] as ItemsPanelTemplate;

                //ItemsPanelTemplate DefaultPanel = new ItemsPanelTemplate();
                //i1.SetValue(ItemsControl.ItemsPanelProperty, ap);
                for (int i = 0; i < CustomColorsCollection.Count; i++)
                {
                    if (i % 8 == 0)
                    {
                        colorGroup.DataSource = coll3;
                        colorGroup.colorpicker = this;
                        ColorArea.Items.Add(colorGroup);

                        colorGroup = new ColorGroup();
                        colorGroup.HeaderVisibility = Visibility.Collapsed;
                        coll3 = null;

                        coll3 = new ObservableCollection<ColorGroupItem>();

                        colorGroup1.DataSource = coll3_1;
                        colorGroup1.colorpicker = this;
                        m_expColorAreaBorder.Items.Add(colorGroup1);

                        colorGroup1 = new ColorGroup();
                        colorGroup1.HeaderVisibility = Visibility.Collapsed;
                        coll3_1 = null;

                        coll3_1 = new ObservableCollection<ColorGroupItem>();
                    }
                    coll3.Add(new ColorGroupItem() { ColorName = CustomColorsCollection[i].ColorName, Color = new SolidColorBrush(CustomColorsCollection[i].Color), Variants = false, BorderMargin = new Thickness(2, 1, 2, 1), ItemMargin = new Thickness(2, 1, 2, 1) });
                    coll3_1.Add(new ColorGroupItem() { ColorName = CustomColorsCollection[i].ColorName, Color = new SolidColorBrush(CustomColorsCollection[i].Color), Variants = false, BorderMargin = new Thickness(2, 1, 2, 1), ItemMargin = new Thickness(2, 1, 2, 1) });
                }
                colorGroup.DataSource = coll3;
                colorGroup.colorpicker = this;
                ColorArea.Items.Add(colorGroup);

                colorGroup1.DataSource = coll3_1;
                colorGroup1.colorpicker = this;
                m_expColorAreaBorder.Items.Add(colorGroup1);
            }
        }

        /// <summary>
        /// This method is raised when mouse moves over Color border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEventArgs</param>
        private void ColorBorderMouseMove(object sender, MouseEventArgs e)
        {
            if (!Popup.IsOpen)
            {
                IsColorBorderSelected = true;
                this.UpdateVisualState(true);
            }
        }

        internal bool leave = true;

        /// <summary>
        /// This method is raised when mouse leaves updown border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEventArgs</param>
        private void updownMouseLeave(object sender, MouseEventArgs e)
        {
            if (!Popup.IsOpen)
            {
                IsUpDownSelected = false;
                IsColorBorderSelected = false;
                this.UpdateVisualState(false);
            }
            leave = true;
        }

        /// <summary>
        /// This method is raised when mouse moves over updown border
        /// </summary>
        /// <param name="sender">Object of the Sender</param>
        /// <param name="e">Handles MouseEventArgs</param>
        private void updownMouseMove(object sender, MouseEventArgs e)
        {
            IsUpDownSelected = true;
            this.UpdateVisualState(true);
            leave = false;
        }

        /// <summary>
        /// This method loads the Standard color panel
        /// </summary>
        /// <param name="variant">Indicates whether to generate Standard variants on not</param>
        public void LoadStandardColors(bool variant)
        {
            StdColorCollection = new ObservableCollection<ColorGroupItem>();
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Red", Color = new SolidColorBrush(Colors.Red), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Green", Color = new SolidColorBrush(Colors.Green), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Yellow", Color = new SolidColorBrush(Colors.Yellow), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Blue", Color = new SolidColorBrush(Colors.Blue), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Brown", Color = new SolidColorBrush(Colors.Brown), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Orange", Color = new SolidColorBrush(Colors.Orange), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Purple", Color = new SolidColorBrush(Colors.Purple), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Sky Blue", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 176, 240)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "White", Color = new SolidColorBrush(Colors.White), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            StdColorCollection.Add(new ColorGroupItem() { ColorName = "Black", Color = new SolidColorBrush(Colors.Black), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
        }

        /// <summary>
        /// This Method loads the Theme Color panel
        /// </summary>
        /// <param name="variant">Indicates whether to generate Theme variants on not</param>
        public void LoadThemeColors(bool variant)
        {
            if (col != null)
            {
                col[0] = new ObservableCollection<ColorGroupItem>();
                col[0].Add(new ColorGroupItem() { ColorName = "Tan,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 238, 236, 225)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[0].Add(new ColorGroupItem() { ColorName = "Dark Blue,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 31, 73, 125)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[0].Add(new ColorGroupItem() { ColorName = "Blue,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 79, 129, 189)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[0].Add(new ColorGroupItem() { ColorName = "Red,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 192, 80, 77)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[0].Add(new ColorGroupItem() { ColorName = "Olive Green,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 155, 187, 89)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[0].Add(new ColorGroupItem() { ColorName = "Purple,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 128, 100, 162)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[0].Add(new ColorGroupItem() { ColorName = "Aqua,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 75, 172, 198)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[0].Add(new ColorGroupItem() { ColorName = "Orange,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 247, 150, 70)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1] = new ObservableCollection<ColorGroupItem>();
                col[1].Add(new ColorGroupItem() { ColorName = "White,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 248, 248, 248)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1].Add(new ColorGroupItem() { ColorName = "Black,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 10, 10, 10)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1].Add(new ColorGroupItem() { ColorName = "Gray-25%,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 221, 221, 221)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1].Add(new ColorGroupItem() { ColorName = "Gray-25%,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 178, 178, 178)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1].Add(new ColorGroupItem() { ColorName = "Gray-50%,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 150, 150, 150)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1].Add(new ColorGroupItem() { ColorName = "Gray-50%,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 128, 128, 128)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1].Add(new ColorGroupItem() { ColorName = "Gray-80%,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 95, 95, 95)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[1].Add(new ColorGroupItem() { ColorName = "Gray-80%,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 77, 77, 77)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2] = new ObservableCollection<ColorGroupItem>();
                col[2].Add(new ColorGroupItem() { ColorName = "Lavender,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 201, 194, 209)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2].Add(new ColorGroupItem() { ColorName = "Gray-50%,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 105, 103, 109)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2].Add(new ColorGroupItem() { ColorName = "Tan,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 206, 185, 102)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2].Add(new ColorGroupItem() { ColorName = "Olive Green,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 156, 176, 132)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2].Add(new ColorGroupItem() { ColorName = "Aqua,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 107, 177, 201)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2].Add(new ColorGroupItem() { ColorName = "Blue,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 101, 133, 207)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2].Add(new ColorGroupItem() { ColorName = "Lavender,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 126, 107, 201)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[2].Add(new ColorGroupItem() { ColorName = "Lavender,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 163, 121, 187)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3] = new ObservableCollection<ColorGroupItem>();
                col[3].Add(new ColorGroupItem() { ColorName = "Tan,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 227, 222, 209)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3].Add(new ColorGroupItem() { ColorName = "Gray-80%,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 50, 50)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3].Add(new ColorGroupItem() { ColorName = "Orange,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 240, 127, 9)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3].Add(new ColorGroupItem() { ColorName = "Red,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 159, 41, 54)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3].Add(new ColorGroupItem() { ColorName = "Dark Blue,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 27, 88, 124)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3].Add(new ColorGroupItem() { ColorName = "Dark Green,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 78, 133, 66)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3].Add(new ColorGroupItem() { ColorName = "Dark Purple,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 72, 120)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[3].Add(new ColorGroupItem() { ColorName = "Tan,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 193, 152, 89)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4] = new ObservableCollection<ColorGroupItem>();
                col[4].Add(new ColorGroupItem() { ColorName = "Ice Blue,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 197, 209, 215)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4].Add(new ColorGroupItem() { ColorName = "Blue-Gray,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 100, 107, 134)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4].Add(new ColorGroupItem() { ColorName = "Red,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 209, 99, 73)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4].Add(new ColorGroupItem() { ColorName = "Dark Yellow,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 204, 180, 0)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4].Add(new ColorGroupItem() { ColorName = "Teal,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 140, 173, 174)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4].Add(new ColorGroupItem() { ColorName = "Brown,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 140, 123, 112)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4].Add(new ColorGroupItem() { ColorName = "Green,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 143, 176, 140)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[4].Add(new ColorGroupItem() { ColorName = "Orange,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 209, 144, 73)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5] = new ObservableCollection<ColorGroupItem>();
                col[5].Add(new ColorGroupItem() { ColorName = "Tan,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 233, 229, 220)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5].Add(new ColorGroupItem() { ColorName = "Gray-50,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 105, 100, 100)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5].Add(new ColorGroupItem() { ColorName = "Orange,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 211, 72, 23)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5].Add(new ColorGroupItem() { ColorName = "Dark Red,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 155, 45, 31)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5].Add(new ColorGroupItem() { ColorName = "Brown,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 162, 142, 106)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5].Add(new ColorGroupItem() { ColorName = "Brown,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 149, 98, 81)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5].Add(new ColorGroupItem() { ColorName = "Gray-50,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 145, 132, 133)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[5].Add(new ColorGroupItem() { ColorName = "Brown,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 133, 93, 93)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6] = new ObservableCollection<ColorGroupItem>();
                col[6].Add(new ColorGroupItem() { ColorName = "Light Turquoise,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 219, 245, 249)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6].Add(new ColorGroupItem() { ColorName = "Dark Teal,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 4, 97, 123)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6].Add(new ColorGroupItem() { ColorName = "Blue,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 15, 111, 198)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6].Add(new ColorGroupItem() { ColorName = "Turquoise,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 157, 217)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6].Add(new ColorGroupItem() { ColorName = "Turquoise,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 11, 208, 217)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6].Add(new ColorGroupItem() { ColorName = "Bright Green,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 16, 207, 155)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6].Add(new ColorGroupItem() { ColorName = "Green,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 124, 202, 98)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[6].Add(new ColorGroupItem() { ColorName = "Lime,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 165, 194, 73)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7] = new ObservableCollection<ColorGroupItem>();
                col[7].Add(new ColorGroupItem() { ColorName = "Tan,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 234, 235, 222)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7].Add(new ColorGroupItem() { ColorName = "Olive Green,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 103, 106, 85)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7].Add(new ColorGroupItem() { ColorName = "Green,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 114, 163, 118)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7].Add(new ColorGroupItem() { ColorName = "Light Green,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 176, 204, 176)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7].Add(new ColorGroupItem() { ColorName = "Sky Blue,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 168, 205, 215)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7].Add(new ColorGroupItem() { ColorName = "Tan,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 192, 190, 175)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7].Add(new ColorGroupItem() { ColorName = "Tan,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 206, 197, 151)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[7].Add(new ColorGroupItem() { ColorName = "Rose,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 232, 183, 183)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8] = new ObservableCollection<ColorGroupItem>();
                col[8].Add(new ColorGroupItem() { ColorName = "Tan,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 235, 221, 195)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8].Add(new ColorGroupItem() { ColorName = "Brown,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 119, 95, 85)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8].Add(new ColorGroupItem() { ColorName = "Ice Blue,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 148, 182, 210)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8].Add(new ColorGroupItem() { ColorName = "Orange,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 221, 128, 71)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8].Add(new ColorGroupItem() { ColorName = "Olive Green,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 165, 171, 129)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8].Add(new ColorGroupItem() { ColorName = "Gold,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 216, 178, 92)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8].Add(new ColorGroupItem() { ColorName = "Green,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 123, 167, 157)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[8].Add(new ColorGroupItem() { ColorName = "Gray-50%,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 150, 140, 140)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9] = new ObservableCollection<ColorGroupItem>();
                col[9].Add(new ColorGroupItem() { ColorName = "Light Blue,Background 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 214, 236, 255)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9].Add(new ColorGroupItem() { ColorName = "Blue-Gray,Text 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 78, 91, 111)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9].Add(new ColorGroupItem() { ColorName = "Green,Accent 1", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 127, 209, 59)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9].Add(new ColorGroupItem() { ColorName = "Pink,Accent 2", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 234, 21, 122)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9].Add(new ColorGroupItem() { ColorName = "Gold,Accent 3", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 254, 184, 10)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9].Add(new ColorGroupItem() { ColorName = "Turquoise,Accent 4", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 173, 220)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9].Add(new ColorGroupItem() { ColorName = "Periwinkle,Accent 5", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 115, 138, 200)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                col[9].Add(new ColorGroupItem() { ColorName = "Teal,Accent 6", Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 26, 179, 159)), Variants = variant, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
            }
        }

        /// <summary>
        /// Load the theme color panel with selected theme colors
        /// </summary>
        private void ThemeColors()
        {
            if (item1 != null && m_expItem1 != null)
            {
                item1.colorpicker = this;
                m_expItem1.colorpicker = this;

                LoadThemeColors(GenerateThemeVariants);
                if (col != null)
                    ColorGroupCollection = col[(int)this.Themes];

                item1.DataSource = ColorGroupCollection;
                m_expItem1.DataSource = ColorGroupCollection;

                if (this.BlackWhiteVisibility == BlackWhiteVisible.Both)
                {
                    ColorGroupCollection.Insert(0, (new ColorGroupItem() { ColorName = "White, Background 1", Color = new SolidColorBrush(Colors.White), Variants = GenerateThemeVariants, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) }));
                    ColorGroupCollection.Insert(1, (new ColorGroupItem() { ColorName = "Black, Text 1", Color = new SolidColorBrush(Colors.Black), Variants = GenerateThemeVariants, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) }));
                }
                else if (this.BlackWhiteVisibility == BlackWhiteVisible.White)
                {
                    ColorGroupCollection.Insert(0, (new ColorGroupItem() { ColorName = "White, Background 1", Color = new SolidColorBrush(Colors.White), Variants = GenerateThemeVariants, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) }));
                }
                else if (this.BlackWhiteVisibility == BlackWhiteVisible.Black)
                {
                    ColorGroupCollection.Insert(0, (new ColorGroupItem() { ColorName = "Black, Text 1", Color = new SolidColorBrush(Colors.Black), Variants = GenerateThemeVariants, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) }));
                }

                item1.PanelVisibility = this.ThemePanelVisibility;
                m_expItem1.PanelVisibility = this.ThemePanelVisibility;
            }
        }

        /// <summary>
        /// Load the Standard color panel with predefined standard colors
        /// </summary>
        private void StandardColors()
        {
            if (item2 != null && m_expItem2 != null)
            {
                item2.colorpicker = this;
                m_expItem2.colorpicker = this;

                LoadStandardColors(GenerateStandardVariants);
                ColorGroupCollection = StdColorCollection;

                if (this.BlackWhiteVisibility == BlackWhiteVisible.Black || this.BlackWhiteVisibility == BlackWhiteVisible.White)
                {
                    ColorGroupCollection.RemoveAt(ColorGroupCollection.Count - 1);
                }

                if (this.BlackWhiteVisibility == BlackWhiteVisible.None)
                {
                    ColorGroupCollection.RemoveAt(ColorGroupCollection.Count - 1);
                    ColorGroupCollection.RemoveAt(ColorGroupCollection.Count - 1);
                }

                item2.DataSource = ColorGroupCollection;
                item2.PanelVisibility = this.StandardPanelVisibility;

                m_expItem2.DataSource = ColorGroupCollection;
                m_expItem2.PanelVisibility = this.StandardPanelVisibility;
            }
        }

        /// <summary>
        /// Load the Recently Used panel with recently selected colors
        /// </summary>
        private void RecentlyUsed()
        {
            if (item3 != null && m_expItem3 != null)
            {
                item3.colorpicker = this;
                item3.DataSource = RecentlyUsedCollection;
                item3.PanelVisibility = this.RecentlyUsedPanelVisibility;

                m_expItem3.colorpicker = this;
                m_expItem3.DataSource = exp_recentlyusedcollection;
                m_expItem3.PanelVisibility = this.RecentlyUsedPanelVisibility;
            }
        }

        /// <summary>
        /// Called when automatic button is clicked
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Routed Event</param>
        private void automaticbuttonClick(object sender, RoutedEventArgs e)
        {
            this.ColorName = "Automatic color";
            IsAutomaticSelected = true;
            this.Color = ((SolidColorBrush)autoColorBorder.Background).Color;

            this.IsChecked = false;
            this.IsColorBorderSelected = false;
            this.IsUpDownSelected = false;
            this.IsSelected = true;
            this.UpdateVisualState(true);
            Popup.IsOpen = false;
            dispose();
            if (SelectedItem != null)
            {
                SelectedItem.IsSelected = false;
                SelectedItem.UpdateVisualState(false);
                SelectedItem = null;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the m_expAutomaticBorder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void m_expAutomaticBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ExpandedAutomaticBorderPressed = false;
            this.ColorName = "Automatic color";
            IsAutomaticSelected = true;
            this.Color = ((SolidColorBrush)m_expautoBorder.Background).Color;
            this.IsChecked = false;
            this.IsColorBorderSelected = false;
            this.IsUpDownSelected = false;
            this.IsSelected = true;
            if (SelectedItem != null)
            {
                SelectedItem.IsSelected = false;
                SelectedItem = null;
            }
        }

#if WPF

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

#endif
#if !WPF
        /// <summary>
        /// Event raised when mouse is clicked outside the control
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles mouse events</param>
        void ColorPickerPalette_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!updownclick)
            {
                Popup.IsOpen = false;
                dispose();
                IsChecked = false;
                this.IsColorBorderSelected = false;
                this.IsUpDownSelected = false;
                this.UpdateVisualState(false);
            }

            updownclick = false;
         }
#endif
        private double width;
        private double height;

        /// <summary>
        /// Event raised when the control is loaded
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles RoutedEventArgs</param>
        private void ColorPickerPalette_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Color.Equals(Colors.Black))
            {
                this.ColorName = "Automatic Color";
                this.Color = ((SolidColorBrush)AutomaticColor).Color;
                this.IsSelected = true;
                this.UpdateVisualState(true);
                if (col == null)
                    col = new ObservableCollection<ColorGroupItem>[16];
            }
            if (this.ColorName.Equals("Color") || this.IsSelected)
            {
                Isloaded = true;
            }
#if SILVERLIGHT
            if (Application.Current != null && Application.Current.RootVisual != null && Application.Current.RootVisual is FrameworkElement)
                ((FrameworkElement)Application.Current.RootVisual).MouseLeftButtonDown += new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);
#endif

#if WPF
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.MouseDown += MainWindow_MouseDown;
                Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            }
#endif
        }

#if WPF

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (this.Popup != null && this.Popup.IsOpen && !IsExpanded)
                Popup.IsOpen = false;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Popup != null && this.Popup.IsOpen && !IsExpanded)
                Popup.IsOpen = false;
        }

#endif

        /// <summary>
        /// Event raised when mouse leaves the automatic button
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles mouse events</param>
        private void automaticbutton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Event raised when mouse mones over the automatic button
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles mouse events</param>
        private void automaticbutton_MouseMove(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = true;
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Called when Automatic color is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e"> Property change details, such as old value and new value</param>
        private static void IsAutomaticColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.IsAutomaticColorChanged(e);
        }

        /// <summary>
        /// If automatic color is changed when loaded or when color not changed the color is set to the color of automatic color
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void IsAutomaticColorChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (SelectedItem == null)
            //{
            //    this.ColorName = "Automatic color";
            //    this.SelectedColor = AutomaticColor;
            //}
        }

        /// <summary>
        /// Called when Color is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void IsColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.IsColorChanged(e);
        }

        /// <summary>
        /// When color is changed,its presence is checked in RecentlyUsedCollection.If not present then added.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void IsColorChanged(DependencyPropertyChangedEventArgs e)
        {
            bool isPresentflag = false;
            foreach (ColorGroupItem cg in RecentlyUsedCollection)
            {
                if (((SolidColorBrush)cg.Color).Color == this.Color)
                {
                    isPresentflag = true;
                    break;
                }
            }

            if (!isPresentflag && !this.IsSelected && this.Color != ((SolidColorBrush)AutomaticColor).Color)
            {
                if (RecentlyUsedCollection.Count == 8)
                {
                    RecentlyUsedCollection.RemoveAt(0);
                    exp_recentlyusedcollection.RemoveAt(0);
                }

                this.IsMouseOver = false;
                this.IsSelected = false;
                this.UpdateVisualState(false);
                if (Isloaded)
                {
                    if (RecentlyUsedCollection.Count > 0)
                    {
                        ColorGroupItem cgi = RecentlyUsedCollection[0];
                    }
                    if (RecentlyUsedCollection.Count == 8)
                    {
                        RecentlyUsedCollection.RemoveAt(0);
                        exp_recentlyusedcollection.RemoveAt(0);
                    }
                    if (RecentlyUsedCollection.Count < 8 && exp_recentlyusedcollection.Count < 8)
                    {
                        RecentlyUsedCollection.Add(new ColorGroupItem() { ColorName = this.ColorName, Color = new SolidColorBrush(this.Color), Variants = false, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                        exp_recentlyusedcollection.Add(new ColorGroupItem() { ColorName = this.ColorName, Color = new SolidColorBrush(this.Color), Variants = false, BorderMargin = new Thickness(2, 2, 2, 2), ItemMargin = new Thickness(2, 2, 2, 2) });
                    }
                }
                if (!Isloaded)
                {
                    Isloaded = true;
                }
            }

            if (isPresentflag && !this.IsSelected && this.Color != ((SolidColorBrush)AutomaticColor).Color)
            {
                this.IsMouseOver = false;
                this.IsSelected = false;
                this.UpdateVisualState(false);
            }
            if (this.Color != ((SolidColorBrush)AutomaticColor).Color)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "MouseOverAutomatic", true);
            }

            if (ColorChanged != null)
            {
                ColorChanged(this, e);
            }
        }

        /// <summary>
        /// Event raised when visibility of Standard Color panel is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void standard_visibility_changed(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            if (g.item2 != null)
            {
                g.item2.PanelVisibility = g.StandardPanelVisibility;
                if (g.IsExpanded)
                {
                    g.StandardColors();
                }
            }
        }

        /// <summary>
        /// Event raised when Variants generation of theme colors is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void ThemeVariantsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.ThemeVariantsChanged(e);
        }

        /// <summary>
        /// Called when ThemeVariantsChanged Event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void ThemeVariantsChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ThemeColors();
        }

        /// <summary>
        /// Event raised when Theme is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void ThemeColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.ThemeColorChanged(e);
        }

        /// <summary>
        /// Called when ThemeColorChanged event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void ThemeColorChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ThemeColors();
        }

        /// <summary>
        /// Event raised when Visibility of blackwhite colors is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void BlackWhiteVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            if (g != null)
            {
                g.ThemeColors();
                g.StandardColors();
            }
        }

        /// <summary>
        /// Called when BlackWhiteVisibilityChanged is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void BlackWhiteVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when Variants generation of Standard colors is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void StandardVariantsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.StandardVariantsChanged(e);
        }

        /// <summary>
        /// Called when StandardVariantsChanged event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void StandardVariantsChanged(DependencyPropertyChangedEventArgs e)
        {
            this.StandardColors();
        }

        /// <summary>
        /// Event raised when visibility of Theme Color panel is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void ThemeVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.ThemeVisibilityChanged(e);
        }

        /// <summary>
        /// Called when ThemeVisibilityChanged event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void ThemeVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.item1 != null)
            {
                this.item1.PanelVisibility = this.ThemePanelVisibility;
                if (this.IsExpanded)
                {
                    this.ThemeColors();
                }
            }
        }

        private static void OnPopupHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.OnPopupHeightChanged(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPopupHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.PopupHeightChanged != null)
            {
                this.PopupHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Event raised when visibility of Recently Used Color panel is changed
        /// </summary>
        /// <param name="o">ColorPickerPalette object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void recentlyusedvisibilitychanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette g = (ColorPickerPalette)o;
            g.recentlyusedvisibilitychanged(e);
            if (g.item1 != null)
            {
                g.item3.PanelVisibility = g.RecentlyUsedPanelVisibility;
                if (g.IsExpanded)
                {
                    g.RecentlyUsed();
                }
            }
        }

        /// <summary>
        /// Called when recentlyusedvisibilitychanged event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void recentlyusedvisibilitychanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when changes in visual state of automatic button takes place
        /// </summary>
        /// <param name="useTransitions">Indicate whether to apply transition or not</param>
        /// <param name="stateNames">Contain the state name</param>
        internal void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Method is used to update the state of automatic button
        /// </summary>
        /// <param name="useTransitions">Update the state</param>
        internal void UpdateVisualState(bool useTransitions)
        {
            if (this.morecolorsmove)
            {
                this.GoToState(useTransitions, new string[] { "MouseOverBorder" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "NormalBorder" });
            }
            if (this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "Selected" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }

            if (this.IsMouseOver)
            {
                if (!this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "MouseOver" });
                }
                else
                {
                    this.GoToState(useTransitions, new string[] { "MouseOverAutomatic" });
                }
            }

            if (this.IsUpDownSelected)
            {
                this.GoToState(useTransitions, new string[] { "UpDownMouseOver" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "UpDownNormal" });
            }
            if (Popup != null)
            {
                if (Popup.IsOpen == true)
                {
                    this.GoToState(useTransitions, new string[] { "UpDown" });
                }
            }

            if (this.IsColorBorderSelected)
            {
                this.GoToState(useTransitions, new string[] { "ColorMouseOver" });
            }
            if (this.IsColorBorderPressed)
            {
                this.GoToState(useTransitions, new string[] { "ColorBorderPressed" });
            }
        }

        private static void OnSetCustomColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerPalette instance = d as ColorPickerPalette;
            if (instance != null && (bool)e.NewValue)
            {
                instance.LoadCustomColors();
            }
        }

        #endregion Implementation
    }

    /// <summary>
    /// Representing the class for CustomColor
    /// </summary>
    public class CustomColor
    {
        /// <summary>
        /// Gets or sets the Color Property.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets a Color Name Property.
        /// </summary>
        public string ColorName { get; set; }
    }

#if WPF

    public class SizeToDoubleConverter : IValueConverter
    {
        #region Implementation

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            Size size = (Size)value;
            string strParemeter = parameter.ToString();
            if (strParemeter == "width")
            {
                result = size.Width;
            }
            else
            {
                result = size.Height;
            }

            return result;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion Implementation
    }

#endif
}