// <copyright file="ColorEdit.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Tools;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class for color generating. ColorEdit control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/SyncOrange.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/ShinyRed.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/ShinyBlue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(ColorEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/TransparentStyle.xaml")]
    public class ColorEdit : Control
    {
        #region Constants

        /// <summary>
        /// Contains color bar control name.
        /// </summary>
        private const string C_pickerColorBar = "PickerColorBar";

        internal bool changeHSVBackground = true;

        internal bool changeColor = false;

        internal int Hcount;

        internal String CurrentHSV;

        /// <summary>
        /// Contains color toggle button name.
        /// </summary>
        private const string C_colorToggleButton = "colorToggleButton";

        /// <summary>
        /// Contains color palette name.
        /// </summary>
        private const string C_colorPalette = "ColorPalitte";

        /// <summary>
        /// Contains Skin name.
        /// </summary>
        private const string C_defaultSkinName = "Default";

        internal bool CanChange = true;
        internal int count = 0;
        internal bool allow = true;

        /// <summary>
        /// Contains Edit container brush.
        /// </summary>
        private const string C_colorEditContainerBrush = "ColorEditContainerBrush";

        /// <summary>
        /// Contains System colors name.
        /// </summary>
        private const string C_systemColors = "systemColors";

        /// <summary>
        /// Contains buttonH name.
        /// </summary>
        private const string C_buttomH = "ButtomH";

        /// <summary>
        /// Contains c_buttomS name.
        /// </summary>
        private const string C_buttomS = "ButtomS";

        /// <summary>
        /// Contains c_buttomV name.
        /// </summary>
        private const string C_buttomV = "ButtomV";

        /// <summary>
        /// Contains c_wordKnownColorsTextBox name.
        /// </summary>
        private const string C_wordKnownColorsTextBox = "WordKnownColorsTextBox";

        /// <summary>
        /// Contains c_colorStringEditor name.
        /// </summary>
        private const string C_colorStringEditor = "PART_ColorStringEditor";

        /// <summary>
        /// Contains c_suchInRed name.
        /// </summary>
        private const string C_suchInRed = "Such in Red:";

        /// <summary>
        /// Contains c_suchInGreen name.
        /// </summary>
        private const string C_suchInGreen = "Such in Green:";

        /// <summary>
        /// Contains c_suchInBlue name.
        /// </summary>
        private const string C_suchInBlue = "Such in Blue:";

        /// <summary>
        /// Used to check the loop state.
        /// </summary>
        private bool breakLoop = true;

        /// <summary>
        /// Contain the previous selected Brush.
        /// </summary>
        internal Brush previousSelectedBrush = new SolidColorBrush(Colors.White);

        /// <summary>
        /// Used to identify the Mouse Left buton down event in Color Edit.
        /// </summary>
        private bool mouseLeftDown = false;

        #endregion Constants

        #region Private Members

        #region ColorModeRGB

        /// <summary>
        /// Red parameter for RGB model.
        /// </summary>
        internal float m_r;

        /// <summary>
        /// Green parameter for RGB model.
        /// </summary>
        internal float m_g;

        /// <summary>
        /// The Blue parameter for RGB model..
        /// </summary>
        internal float m_b;

        /// <summary>
        /// Alpha or opacity parameter for RGB model.
        /// </summary>
        internal float m_a;

        /// <summary>
        /// Command for white color change. RGB model.
        /// </summary>
        public static RoutedCommand M_changeColorWhite;

        /// <summary>
        /// Command for black color change. RGB model.
        /// </summary>
        public static RoutedCommand M_changeColorBlack;

        #endregion ColorModeRGB

        #region ColorModeHSV

        /// <summary>
        /// Color palette for HSV model.
        /// </summary>
        private FrameworkElement m_colorPalette;

        /// <summary>
        /// Text box for WordKnownColors.
        /// </summary>
        private TextBox m_wordKnownColorsTextBox;

        internal Popup m_wordKnownColorPopup;

        /// <summary>
        /// Checks H mode.
        /// </summary>
        private RadioButton m_buttomH;

        /// <summary>
        /// Checks S mode.
        /// </summary>
        private RadioButton m_buttomS;

        /// <summary>
        /// Checks V mode.
        /// </summary>
        private RadioButton m_buttomV;

        #endregion ColorModeHSV

        /// <summary>
        /// Cached value of the TestProperty property.
        /// </summary>
        internal Color m_color;

        internal float A_value;

        internal bool Allow = false;

        /// <summary>
        /// Cached value of the TestProperty property.
        /// </summary>
        private ColorSelectionMode m_visualizationStyle = ColorSelectionMode.RGB;

        /// <summary>
        /// Contains comboBox with system colors
        /// </summary>
        private ComboBox m_systemColors;

        /// <summary>
        /// Contains toggle button element of the colorPicker.
        /// </summary>
        private ToggleButton m_colorToggleButton;

        /// <summary>
        /// Specifies color, the color editor should revert to in case user cancels the eye dropping
        /// </summary>
        private Color m_colorBeforeEyeDropStart;

        /// <summary>
        /// Indicates whether colors are updating at the moment.
        /// </summary>
        private bool m_bColorUpdating = false;

        /// <summary>
        /// Displays the color string.
        /// </summary>
        private TextBox m_colorStringEditor;

        /// <summary>
        /// Contains ColorBar of the ColorEdit.
        /// </summary>
        private ColorBar m_editColorBar = null;

        /// <summary>
        /// Is need to HSv is checked
        /// </summary>
        private bool m_bNeedChangeHSV = true;

        /// <summary>
        /// Gets or sets the rect bar.
        /// </summary>
        /// <value>The rect bar.</value>
        private Rectangle rectBar { get; set; }

        /// <summary>
        /// Gets or sets the canvas bar.
        /// </summary>
        /// <value>The canvas bar.</value>
        internal Canvas canvasBar { get; set; }

        /// <summary>
        /// Gets or sets the gradient grid.
        /// </summary>
        /// <value>The gradient grid.</value>
        internal Grid gradientGrid { get; set; }

        /// <summary>
        /// private member for types of brushes
        /// </summary>
        private Button linear, radial, Reverse, Solid, Gradient;

        /// <summary>
        /// Buttin for popup
        /// </summary>
        private ToggleButton popupButton;

        /// <summary>
        /// button for popup
        /// </summary>
        private ToggleButton popupBtn;

        /// <summary>
        /// Checks is linear
        /// </summary>
        private bool isLinear = true;

        /// <summary>
        /// Gets or sets the gradient item collection.
        /// </summary>
        /// <value>The gradient item collection.</value>
        internal GradientItemCollection gradientItemCollection { get; set; }

        /// <summary>
        /// enabkes the switch
        /// </summary>
        internal StackPanel enableSwitch;

        /// <summary>
        /// contains the color edit border
        /// </summary>
        internal Border ColorEditBorder;

        /// <summary>
        /// contains the popup
        /// </summary>
        internal Popup GradPopup;

        internal ColorPicker m_colorPicker;
        internal bool bindedmanually = false, flag = false;

        private Rectangle currentColor;

        private Rectangle selectedColor;

        internal Path selectorEllipse;

        /// <summary>
        /// Gets or sets the color of the current.
        /// </summary>
        /// <value>The color of the current.</value>
        internal Rectangle CurrentColor
        {
            get
            {
                return currentColor;
            }
            set
            {
                currentColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the selected.
        /// </summary>
        /// <value>The color of the selected.</value>
        internal Rectangle SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
            }
        }

        #endregion Private Members

        #region Properties

        #region ColorModeRGB

        /// <summary>
        /// Gets or sets a value indicating whether this instance is
        /// ScRGB color.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// True, if this instance is Sc RGB color; otherwise, false.
        /// </value>
        public bool IsScRGBColor
        {
            get
            {
                return (bool)GetValue(IsScRGBColorProperty);
            }

            set
            {
                SetValue(IsScRGBColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Red parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// Red.
        /// </value>
        /// <property name="flag" value="Finished"/>
        public float R
        {
            get
            {
                return m_r;
            }

            set
            {
                SetValue(RProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Green parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Green.
        /// </value>
        public float G
        {
            get
            {
                return m_g;
            }

            set
            {
                SetValue(GProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Blue parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Blue.
        /// </value>
        public float B
        {
            get
            {
                return m_b;
            }

            set
            {
                SetValue(BProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Alpha or opacity parameter for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Alpha.
        /// </value>
        public float A
        {
            get
            {
                return m_a;
            }

            set
            {
                SetValue(AProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Alpha slider for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Alpha.
        /// </value>
        public Brush BackgroundA
        {
            get
            {
                return (Brush)GetValue(BackgroundAProperty);
            }

            set
            {
                SetValue(BackgroundAProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Red slider for RGB model..
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Red.
        /// </value>
        public Brush BackgroundR
        {
            get
            {
                return (Brush)GetValue(BackgroundRProperty);
            }

            set
            {
                SetValue(BackgroundRProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Green slider for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Green.
        /// </value>
        public Brush BackgroundG
        {
            get
            {
                return (Brush)GetValue(BackgroundGProperty);
            }

            set
            {
                SetValue(BackgroundGProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background Blue slider for RGB model.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// <para/>
        /// The background Blue.
        /// </value>
        public Brush BackgroundB
        {
            get
            {
                return (Brush)GetValue(BackgroundBProperty);
            }

            set
            {
                SetValue(BackgroundBProperty, value);
            }
        }

        #endregion ColorModeRGB

        #region ColorModeHSV

        /// <summary>
        /// Gets or sets the Hue parameter for HSV model, value range 0 -
        /// 360.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Hue.
        /// </value>
        public float H
        {
            get
            {
                return (float)GetValue(HProperty);
            }

            set
            {
                SetValue(HProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Saturation parameter for HSV model, value
        /// range 0 - 1(0 - 100%).
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Saturation.
        /// </value>
        public float S
        {
            get
            {
                return (float)GetValue(SProperty);
            }

            set
            {
                SetValue(SProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Value or Brightness parameter for HSV model,
        /// value range 0 - 1(0 - 100%).
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// Value.
        /// </value>
        public float V
        {
            get
            {
                return (float)GetValue(VProperty);
            }

            set
            {
                SetValue(VProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selector position X for HSV model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// The selector position X.
        /// </value>
        private float SelectorPositionX
        {
            get
            {
                return (float)GetValue(SelectorPositionXProperty);
            }

            set
            {
                SetValue(SelectorPositionXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selector position Y for HSV model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// The selector position Y.
        /// </value>
        private float SelectorPositionY
        {
            get
            {
                return (float)GetValue(SelectorPositionYProperty);
            }

            set
            {
                SetValue(SelectorPositionYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the slider value for HSV model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// The slider value.
        /// </value>
        private float SliderValueHSV
        {
            get
            {
                return (float)GetValue(SliderValueHSVProperty);
            }

            set
            {
                SetValue(SliderValueHSVProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the slider max value for HSV model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// The slider max value.
        /// </value>
        private float SliderMaxValueHSV
        {
            get
            {
                return (float)GetValue(SliderMaxValueHSVProperty);
            }

            set
            {
                SetValue(SliderMaxValueHSVProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter selected for visualization.
        /// </summary>
        /// <value>
        /// Type: <see cref="HSV"/> enum.
        /// <para/>
        /// The HSV.
        /// </value>
        public HSV HSV
        {
            get
            {
                return (HSV)GetValue(HSVProperty);
            }

            set
            {
                SetValue(HSVProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thumb template.
        /// </summary>
        /// <value>The thumb template.</value>
        public ControlTemplate ThumbTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(ThumbTemplateProperty);
            }

            set
            {
                SetValue(ThumbTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the WordKnownColors position X for HSV model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// The WordKnownColors position X.
        /// </value>
        private float WordKnownColorsPositionX
        {
            get
            {
                return (float)GetValue(WordKnownColorsPositionXProperty);
            }

            set
            {
                SetValue(WordKnownColorsPositionXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the WordKnownColors position Y for HSV model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// The WordKnownColors position Y.
        /// </value>
        private float WordKnownColorsPositionY
        {
            get
            {
                return (float)GetValue(WordKnownColorsPositionYProperty);
            }

            set
            {
                SetValue(WordKnownColorsPositionYProperty, value);
            }
        }

        #endregion ColorModeHSV

        /// <summary>
        /// Gets or sets the value of the VisualizationStyle dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ColorSelectionMode"/>
        /// </value>
        public ColorSelectionMode VisualizationStyle
        {
            get
            {
                return (ColorSelectionMode)GetValue(VisualizationStyleProperty);
            }

            set
            {
                SetValue(VisualizationStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
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
        /// Gets or sets the startpoint.
        /// </summary>
        /// <value>The startpoint.</value>
        public Point Startpoint
        {
            get
            {
                return (Point)GetValue(StartPointProperty);
            }
            set
            {
                SetValue(StartPointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius X.
        /// </summary>
        /// <value>The radius X.</value>
        public Point RadiusX
        {
            get
            {
                return (Point)GetValue(RadiusXProperty);
            }
            set
            {
                SetValue(RadiusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius Y.
        /// </summary>
        /// <value>The radius Y.</value>
        public Point RadiusY
        {
            get
            {
                return (Point)GetValue(RadiusYProperty);
            }
            set
            {
                SetValue(RadiusYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gradient origin.
        /// </summary>
        /// <value>The gradient origin.</value>
        public Point GradientOrigin
        {
            get
            {
                return (Point)GetValue(GradientOriginProperty);
            }
            set
            {
                SetValue(GradientOriginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        public Point Endpoint
        {
            get
            {
                return (Point)GetValue(EndPointProperty);
            }
            set
            {
                SetValue(EndPointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the centre point.
        /// </summary>
        /// <value>The centre point.</value>
        public Point CentrePoint
        {
            get
            {
                return (Point)GetValue(CentrePointProperty);
            }
            set
            {
                SetValue(CentrePointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        /// <value>The brush.</value>
        public Brush Brush
        {
            get
            {
                return (Brush)GetValue(BrushProperty);
            }
            //set
            //{
            //    SetValue(BrushProperty, value);
            //}
            set
            {
                try
                {
                    SetValue(BrushProperty, value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the invert color dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        public Color InvertColor
        {
            get
            {
                return (Color)GetValue(InvertColorProperty);
            }

            set
            {
                SetValue(InvertColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value of the ColorEdit IsAlphaVisible dependency property.
        /// </summary>
        ///  <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public bool IsAlphaVisible
        {
            get
            {
                return (bool)GetValue(IsAlphaVisibleProperty);
            }
            set
            {
                SetValue(IsAlphaVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush mode.
        /// </summary>
        /// <value>The brush mode.</value>
        public BrushModes BrushMode
        {
            get
            {
                return (BrushModes)GetValue(BrushModeProperty);
            }
            set
            {
                SetValue(BrushModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gradient property editor mode.
        /// </summary>
        /// <value>The gradient property editor mode.</value>
        public GradientPropertyEditorMode GradientPropertyEditorMode
        {
            get
            {
                return (GradientPropertyEditorMode)GetValue(GradientPropertyEditorModeProperty);
            }
            set
            {
                SetValue(GradientPropertyEditorModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open gradient property editor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is open gradient property editor; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpenGradientPropertyEditor
        {
            get
            {
                return (bool)GetValue(IsOpenGradientPropertyEditorProperty);
            }
            set
            {
                SetValue(IsOpenGradientPropertyEditorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is gradient property enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is gradient property enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsGradientPropertyEnabled
        {
            get
            {
                return (bool)GetValue(IsGradientPropertyEnabledProperty);
            }
            set
            {
                SetValue(IsGradientPropertyEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable gradient to solid switch].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable gradient to solid switch]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableGradientToSolidSwitch
        {
            get
            {
                return (bool)GetValue(GradientToSolidSwitchProperty);
            }
            set
            {
                SetValue(GradientToSolidSwitchProperty, value);
            }
        }

        #endregion Properties

        #region Dependency Properties

        #region ColorModeRGB

        /// <summary>
        /// Identifies ColorPicker. Alpha dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty AProperty =
            DependencyProperty.Register("A", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAChanged)));

        /// <summary>
        /// Identifies ColorPicker. Red dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register("R", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnRChanged)));

        /// <summary>
        /// Identifies ColorPicker. Green dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register("G", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGChanged)));

        /// <summary>
        /// Identifies ColorPicker. Blue dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register("B", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBChanged)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundR dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundRProperty =
            DependencyProperty.Register("BackgroundR", typeof(Brush), typeof(ColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 255, 0, 0), 0)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundG dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundGProperty =
            DependencyProperty.Register("BackgroundG", typeof(Brush), typeof(ColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0), 0)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundB dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundBProperty =
            DependencyProperty.Register("BackgroundB", typeof(Brush), typeof(ColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 0, 255), 0)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundA dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundAProperty =
            DependencyProperty.Register("BackgroundA", typeof(Brush), typeof(ColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 0, 0), 0)));

        /// <summary>
        /// Identifies ColorPicker. IsScRGBColor dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty IsScRGBColorProperty =
            DependencyProperty.Register("IsScRGBColor", typeof(bool), typeof(ColorEdit), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies ColorPicker. IsAlphaVisible dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty IsAlphaVisibleProperty =
            DependencyProperty.Register("IsAlphaVisible", typeof(bool), typeof(ColorEdit), new FrameworkPropertyMetadata(true));

        #endregion ColorModeRGB

        #region ColorModeHSV

        /// <summary>
        /// DependencyProperty is used as the backing store for HSV. This
        /// enables animation, styling, binding, etc...
        /// </summary>
        /// <value>
        /// Type: <see cref="HSV"/> enum.
        /// </value>
        public static readonly DependencyProperty HSVProperty =
                DependencyProperty.Register("HSV", typeof(HSV), typeof(ColorEdit), new UIPropertyMetadata(HSV.H));

        /// <summary>
        /// Identifies ColorPicker. Hue dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty HProperty =
            DependencyProperty.Register("H", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHChanged)));

        /// <summary>
        /// Identifies ColorPicker. Saturation dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SProperty =
            DependencyProperty.Register("S", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(0.1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSChanged)));

        /// <summary>
        /// Identifies ColorPicker. Value dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty VProperty =
            DependencyProperty.Register("V", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnVChanged)));

        /// <summary>
        /// Identifies ColorPicker. SliderValueHSV dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SliderValueHSVProperty =
            DependencyProperty.Register("SliderValueHSV", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnSliderValueHSVChanged)));

        /// <summary>
        /// Identifies ColorPicker. SliderMaxValueHSV dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SliderMaxValueHSVProperty =
            DependencyProperty.Register("SliderMaxValueHSV", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(360f));

        /// <summary>
        /// Identifies ColorPicker. SelectorPositionX dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SelectorPositionXProperty =
            DependencyProperty.Register("SelectorPositionX", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(0f));

        /// <summary>
        /// Identifies ColorPicker. SelectorPositionY dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SelectorPositionYProperty =
            DependencyProperty.Register("SelectorPositionY", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(0f));

        /// <summary>
        /// Identifies ColorPicker. WordKnownColorsPositionX dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty WordKnownColorsPositionXProperty =
            DependencyProperty.Register("WordKnownColorsPositionX", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(5f));

        /// <summary>
        /// Identifies ColorPicker. WordKnownColorsPositionY dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty WordKnownColorsPositionYProperty =
            DependencyProperty.Register("WordKnownColorsPositionY", typeof(float), typeof(ColorEdit), new FrameworkPropertyMetadata(5f));

        #endregion ColorModeHSV

        /// <summary>
        /// Identifies ColorPicker. VisualizationStyle dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ColorSelectionMode"/> enum.
        /// </value>
        internal static readonly DependencyProperty VisualizationStyleProperty =
            DependencyProperty.Register("VisualizationStyle", typeof(ColorSelectionMode), typeof(ColorEdit), new FrameworkPropertyMetadata(ColorSelectionMode.RGB, new PropertyChangedCallback(OnVisualizationStyleChanged)));

        /// <summary>
        /// Identifies ColorPicker. Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        public static readonly DependencyProperty ThumbTemplateProperty =
            DependencyProperty.Register("ThumbTemplate", typeof(ControlTemplate), typeof(ColorEdit), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the Color peoperty
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorEdit), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnColorChanged)));

        /// <summary>
        /// Identifies the Start point property
        /// </summary>
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register("StartPoint", typeof(Point), typeof(ColorEdit), new FrameworkPropertyMetadata(new Point(0.5, 0.0)));

        /// <summary>
        /// Identifies the rnd point property
        /// </summary>
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(Point), typeof(ColorEdit), new FrameworkPropertyMetadata(new Point(0.5, 1)));

        /// <summary>
        /// Identifies the centerpoint property
        /// </summary>
        public static readonly DependencyProperty CentrePointProperty = DependencyProperty.Register("CentrePoint", typeof(Point), typeof(ColorEdit), new FrameworkPropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Identifies the Gradient origin property
        /// </summary>
        public static readonly DependencyProperty GradientOriginProperty = DependencyProperty.Register("GradientOrigin", typeof(Point), typeof(ColorEdit), new FrameworkPropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Identifies the Radius X property
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(ColorEdit), new FrameworkPropertyMetadata(0.5d));

        /// <summary>
        /// Identifies the radius Y property
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(ColorEdit), new FrameworkPropertyMetadata(0.5d));

        /// <summary>
        /// Identifies the brush property
        /// </summary>
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(Brush), typeof(ColorEdit), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnSelectedBrushChanged)));

        /// <summary>
        /// Identifies the brush mode property
        /// </summary>
        public static readonly DependencyProperty BrushModeProperty =
            DependencyProperty.Register("BrushMode", typeof(BrushModes), typeof(ColorEdit), new FrameworkPropertyMetadata(BrushModes.Solid, new PropertyChangedCallback(OnBrushModeChanged)));

        /// <summary>
        /// Identifies the gradient to solid switch property
        /// </summary>
        public static readonly DependencyProperty GradientToSolidSwitchProperty =
            DependencyProperty.Register("EnableGradientToSolidSwitch", typeof(bool), typeof(ColorEdit), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnEnableSwitchChanged)));

        /// <summary>
        /// Identifies the gradient property editor model property
        /// </summary>
        public static readonly DependencyProperty GradientPropertyEditorModeProperty =
            DependencyProperty.Register("GradientPropertyEditorMode", typeof(GradientPropertyEditorMode), typeof(ColorEdit), new FrameworkPropertyMetadata(GradientPropertyEditorMode.Popup, new PropertyChangedCallback(OnGradientPropertyEditorModeChanged)));

        /// <summary>
        /// Identifies the is open gradient property editor property
        /// </summary>
        public static readonly DependencyProperty IsOpenGradientPropertyEditorProperty =
             DependencyProperty.Register("IsOpenGradientPropertyEditor", typeof(bool), typeof(ColorEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsOpenGradientPropertyEditorChanged)));

        /// <summary>
        /// Identifies the is open gradient property enable property
        /// </summary>
        public static readonly DependencyProperty IsGradientPropertyEnabledProperty =
             DependencyProperty.Register("IsGradientPropertyEnabled", typeof(bool), typeof(ColorEdit), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsGradientPropertyEnabledChanged)));

        ///<summary>
        /// Identifies ColorPicker. Inverts color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        internal static readonly DependencyProperty InvertColorProperty =
            DependencyProperty.Register("InvertColor", typeof(Color), typeof(ColorEdit), new FrameworkPropertyMetadata(Colors.Green));

        #endregion Dependency Properties

        /// <summary>
        ///
        /// </summary>
        public bool EnableToolTip
        {
            get { return (bool)GetValue(EnableToolTipProperty); }
            set { SetValue(EnableToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableToolTip.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty EnableToolTipProperty =
            DependencyProperty.Register("EnableToolTip", typeof(bool), typeof(ColorEdit), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableToolTipChanged)));

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes static members of the <see cref="ColorEdit"/> class.
        /// </summary>
        static ColorEdit()
        {
            //EnvironmentTest.ValidateLicense(typeof(ColorEdit));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorEdit), new FrameworkPropertyMetadata(typeof(ColorEdit)));
            M_changeColorWhite = new RoutedCommand();
            M_changeColorBlack = new RoutedCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorEdit"/> class.. Creates ColorEditor.
        /// </summary>
        public ColorEdit()
        {
            //if (EnvironmentTestTools.IsSecurityGranted)
            //{
            //    EnvironmentTestTools.StartValidateLicense(typeof(ColorEdit));
            //}
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ColorEdit));
            }
            Initialize();
            mys = new GradientStartPoint() { X = 0.5, Y = 0 };
            mye = new GradientStartPoint() { X = 0.5, Y = 1 };
            Centre = new GradientStartPoint() { X = 0.5, Y = 0.5 };
            Gradientorigin = new GradientStartPoint() { X = 0.5, Y = 0.5 };
            //mys.PropertyChanged += new PropertyChangedEventHandler(mys_PropertyChanged);
            //mye.PropertyChanged += new PropertyChangedEventHandler(mye_PropertyChanged);
            //Centre.PropertyChanged += new PropertyChangedEventHandler(Centre_PropertyChanged);
            //Gradientorigin.PropertyChanged += new PropertyChangedEventHandler(Gradientorigin_PropertyChanged);
            try
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                        System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                }
            }
            catch (InvalidOperationException e)
            {

            }
            this.Loaded += new RoutedEventHandler(ColorEdit_Loaded);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        //protected override void OnInitialized(EventArgs e)
        //{
        //    //base.OnInitialized(e);
        //    //mys.PropertyChanged -= new PropertyChangedEventHandler(mys_PropertyChanged);
        //    //mye.PropertyChanged -= new PropertyChangedEventHandler(mye_PropertyChanged);
        //    //Centre.PropertyChanged -= new PropertyChangedEventHandler(Centre_PropertyChanged);
        //    //Gradientorigin.PropertyChanged -= new PropertyChangedEventHandler(Gradientorigin_PropertyChanged);
        //    //RemoveHandler(BorderEyeDrop.BeginColorPickingEvent, new RoutedEventHandler(ProcessColorPickingStart));
        //    //RemoveHandler(BorderEyeDrop.CancelColorPickingEvent, new RoutedEventHandler(ProcessColorPickingCancel));

        //    //AddHandler(BorderEyeDrop.BeginColorPickingEvent, new RoutedEventHandler(ProcessColorPickingStart));
        //    //AddHandler(BorderEyeDrop.CancelColorPickingEvent, new RoutedEventHandler(ProcessColorPickingCancel));
        //    //mys.PropertyChanged += new PropertyChangedEventHandler(mys_PropertyChanged);
        //    //mye.PropertyChanged += new PropertyChangedEventHandler(mye_PropertyChanged);
        //    //Centre.PropertyChanged += new PropertyChangedEventHandler(Centre_PropertyChanged);
        //    //Gradientorigin.PropertyChanged += new PropertyChangedEventHandler(Gradientorigin_PropertyChanged);

        //}

        private void ColorEdit_Loaded(object sender, RoutedEventArgs e)
        {
            mys.PropertyChanged -= new PropertyChangedEventHandler(mys_PropertyChanged);
            mye.PropertyChanged -= new PropertyChangedEventHandler(mye_PropertyChanged);
            Centre.PropertyChanged -= new PropertyChangedEventHandler(Centre_PropertyChanged);
            Gradientorigin.PropertyChanged -= new PropertyChangedEventHandler(Gradientorigin_PropertyChanged);
            RemoveHandler(BorderEyeDrop.BeginColorPickingEvent, new RoutedEventHandler(ProcessColorPickingStart));
            RemoveHandler(BorderEyeDrop.CancelColorPickingEvent, new RoutedEventHandler(ProcessColorPickingCancel));

            AddHandler(BorderEyeDrop.BeginColorPickingEvent, new RoutedEventHandler(ProcessColorPickingStart));
            AddHandler(BorderEyeDrop.CancelColorPickingEvent, new RoutedEventHandler(ProcessColorPickingCancel));
            mys.PropertyChanged += new PropertyChangedEventHandler(mys_PropertyChanged);
            mye.PropertyChanged += new PropertyChangedEventHandler(mye_PropertyChanged);
            Centre.PropertyChanged += new PropertyChangedEventHandler(Centre_PropertyChanged);
            Gradientorigin.PropertyChanged += new PropertyChangedEventHandler(Gradientorigin_PropertyChanged);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Gradientorigin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Gradientorigin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.GradientOrigin = new Point((sender as GradientStartPoint).X, (sender as GradientStartPoint).Y);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Centre control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Centre_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CentrePoint = new Point((sender as GradientStartPoint).X, (sender as GradientStartPoint).Y);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the mye control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void mye_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Endpoint = new Point((sender as GradientStartPoint).X, (sender as GradientStartPoint).Y);
        }

        private CommandBinding colorWhiteBinding;
        private CommandBinding colorBlackBinding;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            if (colorWhiteBinding != null)
            {
                colorWhiteBinding.Executed -= new ExecutedRoutedEventHandler(ChangeColorWhite);
            }
            if (colorBlackBinding != null)
            {
                colorBlackBinding.Executed -= new ExecutedRoutedEventHandler(ChangeColorBlack);
            }

            SizeChanged -= new SizeChangedEventHandler(ColorEdit_SizeChanged);

            colorWhiteBinding = new CommandBinding(M_changeColorWhite);
            colorWhiteBinding.Executed += new ExecutedRoutedEventHandler(ChangeColorWhite);
            colorBlackBinding = new CommandBinding(M_changeColorBlack);
            colorBlackBinding.Executed += new ExecutedRoutedEventHandler(ChangeColorBlack);
            CommandBindings.Add(colorWhiteBinding);
            CommandBindings.Add(colorBlackBinding);
            SizeChanged += new SizeChangedEventHandler(ColorEdit_SizeChanged);
            //OnColorChanged(new DependencyPropertyChangedEventArgs(ColorProperty, Color, Colors.White));
        }

        #endregion Class Initialize/Finalize methods

        #region Events

        #region ColorModeRGB

        /// <summary>
        /// Event that is raised when R property is changed.
        /// </summary>
        public event PropertyChangedCallback RChanged;

        /// <summary>
        /// Event that is raised when G property is changed.
        /// </summary>
        public event PropertyChangedCallback GChanged;

        /// <summary>
        /// Event that is raised when B property is changed.
        /// </summary>
        public event PropertyChangedCallback BChanged;

        /// <summary>
        /// Event that is raised when A property is changed.
        /// </summary>
        public event PropertyChangedCallback AChanged;

        #endregion ColorModeRGB

        #region ColorModeHSV

        /// <summary>
        /// Event that is raised when H property is changed.
        /// </summary>
        public event PropertyChangedCallback HChanged;

        /// <summary>
        /// Event that is raised when S property is changed.
        /// </summary>
        public event PropertyChangedCallback SChanged;

        /// <summary>
        /// Event that is raised when V/B property is changed.
        /// </summary>
        public event PropertyChangedCallback VChanged;

        /// <summary>
        /// Event that is raised when SliderValueHSV property is changed.
        /// </summary>
        public event PropertyChangedCallback SliderValueHSVChanged;

        #endregion ColorModeHSV

        /// <summary>
        /// Event that is raised when VisualizationStyle property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback VisualizationStyleChanged;

        /// <summary>
        /// Event that is raised when Color property is changed.
        /// </summary>
        public event PropertyChangedCallback ColorChanged;

        /// <summary>
        /// Occurs when [gradient property editor mode changed].
        /// </summary>
        public event PropertyChangedCallback GradientPropertyEditorModeChanged;

        /// <summary>
        /// Occurs when [is open gradient property editor changed].
        /// </summary>
        public event PropertyChangedCallback IsOpenGradientPropertyEditorChanged;

        /// <summary>
        /// Occurs when [is gradient property enabled changed].
        /// </summary>
        public event PropertyChangedCallback IsGradientPropertyEnabledChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback EnableToolTipChanged;

        #endregion Events

        #region Static Methods

        #region ColorModeRGB

        /// <summary>
        /// Called when parameter R is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnRChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnRChanged(e);
        }

        /// <summary>
        /// Called when parameter G is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnGChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnGChanged(e);
        }

        /// <summary>
        /// Called when parameter B is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnBChanged(e);
        }

        /// <summary>
        /// Called when parameter A is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnAChanged(e);
        }

        #endregion ColorModeRGB

        #region ColorModeHSV

        /// <summary>
        /// Called when parameter H is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnHChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnHChanged(e);
        }

        /// <summary>
        /// Called when parameter S is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnSChanged(e);
        }

        /// <summary>
        /// Called when parameter V is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnVChanged(e);
        }

        /// <summary>
        /// Called when slider value HSV is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnSliderValueHSVChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnSliderValueHSVChanged(e);
        }

        #endregion ColorModeHSV

        /// <summary>
        /// Generates the default HSV brush.
        /// </summary>
        /// <returns>
        /// Generated HSV brush.
        /// </returns>
        private static Brush GenerateHSVBrushStatic()
        {
            DrawingBrush drawingBrush = new DrawingBrush();
            DrawingGroup drawingGroup = new DrawingGroup();

            RectangleGeometry myRectGeometry = new RectangleGeometry();
            myRectGeometry.Rect = new Rect(0, 0, 200, 300);

            GeometryDrawing geometreDrawing1 = new GeometryDrawing();

            LinearGradientBrush firstBrush = new LinearGradientBrush();
            firstBrush.StartPoint = new Point(0, 0.5);
            firstBrush.EndPoint = new Point(1, 0.5);
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 000, 000), 0.000));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 000), 0.166));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 000, 255, 000), 0.333));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 000, 255, 255), 0.500));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 000, 000, 255), 0.666));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 000, 255), 0.833));
            firstBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 000, 000), 1.000));

            geometreDrawing1.Brush = firstBrush;
            geometreDrawing1.Geometry = myRectGeometry;

            GeometryDrawing geometreDrawing2 = new GeometryDrawing();

            LinearGradientBrush secondBrush = new LinearGradientBrush();
            secondBrush.StartPoint = new Point(0.5, 0);
            secondBrush.EndPoint = new Point(0.5, 1);
            secondBrush.GradientStops.Add(new GradientStop(Color.FromArgb(000, 125, 125, 125), 0));
            secondBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 125, 125, 125), 1));

            geometreDrawing2.Brush = secondBrush;
            geometreDrawing2.Geometry = myRectGeometry;

            drawingGroup.Children.Add(geometreDrawing1);
            drawingGroup.Children.Add(geometreDrawing2);

            drawingBrush.Drawing = drawingGroup;

            return drawingBrush;
        }

        /// <summary>
        /// Calls OnVisualizationStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnVisualizationStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnVisualizationStyleChanged(e);
        }

        /// <summary>
        /// Calls OnColorChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnColorChanged(e);
        }

        /// <summary>
        /// Called when [selected brush changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnSelectedBrushChanged(e);
        }

        /// <summary>
        /// Called when [enable switch changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableSwitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnEnableSwitchChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:EnableSwitchChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnEnableSwitchChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (this.enableSwitch != null)
            //{
            //    if (this.EnableGradientToSolidSwitch)
            //    {
            //        enableSwitch.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        enableSwitch.Visibility = Visibility.Collapsed;
            //    }
            //}
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedBrushChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                if (this.m_colorPicker == null)
                {
                    if (e.NewValue is SolidColorBrush)
                    {
                        this.Color = ((SolidColorBrush)e.NewValue).Color;
                        if (this.SelectedColor != null && CurrentColor != null)
                        {
                            this.CurrentColor.Fill = this.previousSelectedBrush;
                            this.SelectedColor.Fill = this.Brush;
                            Hcount = 0;
                        }
                        this.BrushMode = BrushModes.Solid;
                    }
                    else if (e.NewValue is GradientBrush)
                    {
                        if (((GradientBrush)e.NewValue).GradientStops.Count > 0 && breakLoop)
                            this.Color = (((GradientBrush)e.NewValue).GradientStops[((GradientBrush)e.NewValue).GradientStops.Count - 1]).Color;
                        if (this.SelectedColor != null)
                        {
                            this.CurrentColor.Fill = this.previousSelectedBrush;
                            this.SelectedColor.Fill = this.Brush;
                            Hcount = 0;
                        }
                        this.BrushMode = BrushModes.Gradient;
                    }
                }
                else if (m_colorPicker == null)
                    Hcount = 0;
            }
        }

        /// <summary>
        /// Called when [brush mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBrushModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;
            instance.OnBrushModeChanged(e);
        }

        /// <summary>
        /// Called when [gradient property editor mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGradientPropertyEditorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;

            instance.OnGradientPropertyEditorModeChanged(e);
        }

        /// <summary>
        /// Called when [is open gradient property editor changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsOpenGradientPropertyEditorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;

            instance.OnIsOpenGradientPropertyEditorChanged(e);
        }

        /// <summary>
        /// Called when [is gradient property enabled changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;

            instance.OnEnableToolTipChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:GradientPropertyEditorModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnGradientPropertyEditorModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (gradientItemCollection != null && gradientItemCollection.Items.Count != 0)
            {
                canvasBar.Children.Clear();
            }
            if (GradientPropertyEditorModeChanged != null)
            {
                GradientPropertyEditorModeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsOpenGradientPropertyEditorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnIsOpenGradientPropertyEditorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GradPopup != null)
                GradPopup.IsOpen = !GradPopup.IsOpen;
            if (IsOpenGradientPropertyEditorChanged != null)
            {
                IsOpenGradientPropertyEditorChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsGradientPropertyEnabledChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnIsGradientPropertyEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                if (Gradient != null)
                {
                    Gradient.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (Gradient != null)
                {
                    Gradient.Visibility = Visibility.Collapsed;
                    IsOpenGradientPropertyEditor = false;
                }
            }
            if (IsGradientPropertyEnabledChanged != null)
            {
                IsGradientPropertyEnabledChanged(this, e);
            }
        }

        private static void OnIsGradientPropertyEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorEdit instance = (ColorEdit)d;

            instance.OnIsGradientPropertyEnabledChanged(e);
        }

        private void OnEnableToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                CalculateHSVSelectorPosition();
            if (m_wordKnownColorPopup != null)
                m_wordKnownColorPopup.IsOpen = (bool)e.NewValue;

            if (EnableToolTipChanged != null)
            {
                EnableToolTipChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:BrushModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnBrushModeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Searches for known and similar colors.
        /// </summary>
        /// <param name="color">Identification color.</param>
        /// <returns>
        /// Array of known or similar colors.
        /// </returns>
        public static string[] SuchColor(Color color)
        {
            int lastMinDiff = int.MaxValue;
            int red = int.MaxValue;
            int green = int.MaxValue;
            int blue = int.MaxValue;
            int resIdRed = 0;
            int resIdGreen = 0;
            int resIdBlue = 0;
            int resId = 0;
            uint[] wordKnownColors = InitColorTable();

            for (int i = 0; i < wordKnownColors.Length; i++)
            {
                Color iColor = FromUInt32(wordKnownColors[i]);

                int redDiff = Math.Abs(iColor.R - color.R);
                int greenDiff = Math.Abs(iColor.G - color.G);
                int blueDiff = Math.Abs(iColor.B - color.B);
                int ADiff = Math.Abs(iColor.A - color.A);

                int rgbDiff = redDiff + blueDiff + greenDiff;

                if (rgbDiff < lastMinDiff)
                {
                    if (iColor.A.Equals(color.A))
                    {
                        resId = i;
                        lastMinDiff = rgbDiff;
                    }
                    else if (color.A != 0)
                    {
                        resId = i;
                        lastMinDiff = rgbDiff;
                    }
                }

                if (redDiff < red)
                {
                    resIdRed = i;
                    red = redDiff;
                }

                if (greenDiff < green)
                {
                    resIdGreen = i;
                    green = greenDiff;
                }

                if (blueDiff < blue)
                {
                    resIdBlue = i;
                    blue = blueDiff;
                }
            }

            // return FromUInt32(WordKnownColors[resId]);
            KnownColor color1 = (KnownColor)wordKnownColors[resId];
            KnownColor color2 = (KnownColor)wordKnownColors[resIdRed];
            KnownColor color3 = (KnownColor)wordKnownColors[resIdGreen];
            KnownColor color4 = (KnownColor)wordKnownColors[resIdBlue];

            string[] result = new string[4];
            result[0] = color1.ToString();
            result[1] = color2.ToString();
            result[2] = color3.ToString();
            result[3] = color4.ToString();

            return result;
        }

        /// <summary>
        /// Creates table of colors.
        /// </summary>
        /// <returns>
        /// Table of colors.
        /// </returns>
        private static uint[] InitColorTable()
        {
            uint[] numArray = new uint[142];
            numArray[0] = 0xfff0f8ff;
            numArray[1] = 0xfffaebd7;
            numArray[2] = 0xff00ffff;
            numArray[3] = 0xff7fffd4;
            numArray[4] = 0xfff0ffff;
            numArray[5] = 0xfff5f5dc;
            numArray[6] = 0xffffe4c4;
            numArray[7] = 0xff000000;
            numArray[8] = 0xffffebcd;
            numArray[9] = 0xff0000ff;
            numArray[10] = 0xff8a2be2;
            numArray[11] = 0xffa52a2a;
            numArray[12] = 0xffdeb887;
            numArray[13] = 0xff5f9ea0;
            numArray[14] = 0xff7fff00;
            numArray[15] = 0xffd2691e;
            numArray[16] = 0xffff7f50;
            numArray[17] = 0xff6495ed;
            numArray[18] = 0xfffff8dc;
            numArray[19] = 0xffdc143c;
            numArray[20] = 0xff00ffff;
            numArray[21] = 0xff00008b;
            numArray[22] = 0xff008b8b;
            numArray[23] = 0xffb8860b;
            numArray[24] = 0xffa9a9a9;
            numArray[25] = 0xff006400;
            numArray[26] = 0xffbdb76b;
            numArray[27] = 0xff8b008b;
            numArray[28] = 0xff556b2f;
            numArray[29] = 0xffff8c00;
            numArray[30] = 0xff9932cc;
            numArray[31] = 0xff8b0000;
            numArray[32] = 0xffe9967a;
            numArray[33] = 0xff8fbc8f;
            numArray[34] = 0xff483d8b;
            numArray[35] = 0xff2f4f4f;
            numArray[36] = 0xff00ced1;
            numArray[37] = 0xff9400d3;
            numArray[38] = 0xffff1493;
            numArray[39] = 0xff00bfff;
            numArray[40] = 0xff696969;
            numArray[41] = 0xff1e90ff;
            numArray[42] = 0xffb22222;
            numArray[43] = 0xfffffaf0;
            numArray[44] = 0xff228b22;
            numArray[45] = 0xffff00ff;
            numArray[46] = 0xffdcdcdc;
            numArray[47] = 0xfff8f8ff;
            numArray[48] = 0xffffd700;
            numArray[49] = 0xffdaa520;
            numArray[50] = 0xff808080;
            numArray[51] = 0xff008000;
            numArray[52] = 0xffadff2f;
            numArray[53] = 0xfff0fff0;
            numArray[54] = 0xffff69b4;
            numArray[55] = 0xffcd5c5c;
            numArray[56] = 0xff4b0082;
            numArray[57] = 0xfffffff0;
            numArray[58] = 0xfff0e68c;
            numArray[59] = 0xffe6e6fa;
            numArray[60] = 0xfffff0f5;
            numArray[61] = 0xff7cfc00;
            numArray[62] = 0xfffffacd;
            numArray[63] = 0xffadd8e6;
            numArray[64] = 0xfff08080;
            numArray[65] = 0xffe0ffff;
            numArray[66] = 0xfffafad2;
            numArray[67] = 0xffd3d3d3;
            numArray[68] = 0xff90ee90;
            numArray[69] = 0xffffb6c1;
            numArray[70] = 0xffffa07a;
            numArray[71] = 0xff20b2aa;
            numArray[72] = 0xff87cefa;
            numArray[73] = 0xff778899;
            numArray[74] = 0xffb0c4de;
            numArray[75] = 0xffffffe0;
            numArray[76] = 0xff00ff00;
            numArray[77] = 0xff32cd32;
            numArray[78] = 0xfffaf0e6;
            numArray[79] = 0xffff00ff;
            numArray[80] = 0xff800000;
            numArray[81] = 0xff66cdaa;
            numArray[82] = 0xff0000cd;
            numArray[83] = 0xffba55d3;
            numArray[84] = 0xff9370db;
            numArray[85] = 0xff3cb371;
            numArray[86] = 0xff7b68ee;
            numArray[87] = 0xff00fa9a;
            numArray[88] = 0xff48d1cc;
            numArray[89] = 0xffc71585;
            numArray[90] = 0xff191970;
            numArray[91] = 0xfff5fffa;
            numArray[92] = 0xffffe4e1;
            numArray[93] = 0xffffe4b5;
            numArray[94] = 0xffffdead;
            numArray[95] = 0xff000080;
            numArray[96] = 0xfffdf5e6;
            numArray[97] = 0xff808000;
            numArray[98] = 0xff6b8e23;
            numArray[99] = 0xffffa500;
            numArray[100] = 0xffff4500;
            numArray[101] = 0xffda70d6;
            numArray[102] = 0xffeee8aa;
            numArray[103] = 0xff98fb98;
            numArray[104] = 0xffafeeee;
            numArray[105] = 0xffdb7093;
            numArray[106] = 0xffffefd5;
            numArray[107] = 0xffffdab9;
            numArray[108] = 0xffcd853f;
            numArray[109] = 0xffffc0cb;
            numArray[110] = 0xffdda0dd;
            numArray[111] = 0xffb0e0e6;
            numArray[112] = 0xff800080;
            numArray[113] = 0xffff0000;
            numArray[114] = 0xffbc8f8f;
            numArray[115] = 0xff4169e1;
            numArray[116] = 0xff8b4513;
            numArray[117] = 0xfffa8072;
            numArray[118] = 0xfff4a460;
            numArray[119] = 0xff2e8b57;
            numArray[120] = 0xfffff5ee;
            numArray[121] = 0xffa0522d;
            numArray[122] = 0xffc0c0c0;
            numArray[123] = 0xff87ceeb;
            numArray[124] = 0xff6a5acd;
            numArray[125] = 0xff708090;
            numArray[126] = 0xfffffafa;
            numArray[127] = 0xff00ff7f;
            numArray[128] = 0xff4682b4;
            numArray[129] = 0xffd2b48c;
            numArray[130] = 0xff008080;
            numArray[131] = 0xffd8bfd8;
            numArray[132] = 0xffff6347;
            numArray[133] = 0xffffffff;
            numArray[134] = 0xff40e0d0;
            numArray[135] = 0x00ffffff;
            numArray[136] = 0xffee82ee;
            numArray[137] = 0xfff5deb3;
            numArray[138] = 0xffffffff;
            numArray[139] = 0xfff5f5f5;
            numArray[140] = 0xffffff00;
            numArray[141] = 0xff9acd32;

            return numArray;
        }

        /// <summary>
        /// Converts from UInt32 to color.
        /// </summary>
        /// <param name="argb">ARGB channel for color.</param>
        /// <returns>
        /// Result color.
        /// </returns>
        private static Color FromUInt32(UInt32 argb)
        {
            Color color1 = new Color();
            color1.A = (byte)((argb & 0xff000000) >> 0x18);
            color1.R = (byte)((argb & 0xff0000) >> 0x10);
            color1.G = (byte)((argb & 0xff00) >> 8);
            color1.B = (byte)(argb & 0xff);
            return color1;
        }

        #endregion Static Methods

        #region Internals

        #region ColorModeRGB

        /// <summary>
        /// Called when parameter R is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void OnRChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_r != (float)e.NewValue)
            {
                m_r = (float)e.NewValue;
                if (RChanged != null)
                {
                    RChanged(this, e);
                }
                UpdateColor();
            }
        }

        /// <summary>
        /// Called when parameter G is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void OnGChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_g != (float)e.NewValue)
            {
                m_g = (float)e.NewValue;
                if (GChanged != null)
                {
                    GChanged(this, e);
                }

                UpdateColor();
            }
        }

        /// <summary>
        /// Called when parameter B is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnBChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_b != (float)e.NewValue)
            {
                m_b = (float)e.NewValue;
                if (BChanged != null)
                {
                    BChanged(this, e);
                }

                UpdateColor();
            }
        }

        /// <summary>
        /// Updates color property value, background and color bar slider.
        /// </summary>
        private void UpdateColor()
        {
            if (!m_bColorUpdating)
            {
                Color = Color.FromScRgb(m_a, m_r, m_g, m_b);
                CalculateBackground();
            }
        }

        /// <summary>
        /// Gets H part of the HSV color.
        /// </summary>
        /// <returns>H color part</returns>
        private float GetHPart()
        {
            HsvColor hsvColor = HsvColor.ConvertRgbToHsv(Color.R, Color.B, Color.G);
            return (float)hsvColor.H;
        }

        /// <summary>
        /// Called when parameter A is changed.
        /// </summary>
        /// <param name="e">The  routed event args  <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnAChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_a != (float)e.NewValue)
            {
                m_a = (float)e.NewValue;
                A_value = (float)(e.NewValue);
                Allow = false;

                if (AChanged != null)
                {
                    AChanged(this, e);
                }

                if (!m_bColorUpdating)
                {
                    Color = Color.FromScRgb(m_a, m_r, m_g, m_b);
                    CalculateBackground();
                }
            }
        }

        /// <summary>
        /// Changes the white color.
        /// </summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">The routed event args <see cref="T:System.Windows.Input.ExecutedRoutedEventArgs" />
        /// instance containing the event data.</param>
        private void ChangeColorWhite(object sender, ExecutedRoutedEventArgs e)
        {
            blackWhitePressed = true;
            Color = Colors.White;
        }

        /// <summary>
        /// Changes black color.
        /// </summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">The routed event args  <see cref="T:System.Windows.Input.ExecutedRoutedEventArgs" />
        /// instance containing the event data.</param>
        private void ChangeColorBlack(object sender, ExecutedRoutedEventArgs e)
        {
            blackWhitePressed = true;
            Color = Colors.Black;
        }

        /// <summary>
        /// Updates color bar slider value.
        /// </summary>
        private void UpdateColorBarSlider()
        {
            if (m_editColorBar != null)
            {
                m_editColorBar.SliderValue = GetHPart();
            }
        }

        #endregion ColorModeRGB

        #region ColorModeHSV

        /// <summary>
        /// Called when parameter H is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void OnHChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HSV == HSV.H)
            {
                SliderValueHSV = H;
            }

            if (HChanged != null)
            {
                HChanged(this, e);
            }

            if (!m_bColorUpdating)
            {
                Color = HsvColor.ConvertHsvToRgb(H, S, V);
            }
        }

        /// <summary>
        /// Called when parameter S is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnSChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HSV == HSV.S)
            {
                SliderValueHSV = S;
            }

            if (SChanged != null)
            {
                SChanged(this, e);
            }

            if (!m_bColorUpdating)
            {
                Color = HsvColor.ConvertHsvToRgb(H, S, V);
            }
            CalculateHSVBackground();
        }

        /// <summary>
        /// Called when parameter V is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnVChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HSV == HSV.V)
            {
                SliderValueHSV = V;
            }

            if (VChanged != null)
            {
                VChanged(this, e);
            }

            if (!m_bColorUpdating)
            {
                Color = HsvColor.ConvertHsvToRgb(H, S, V);
            }
            CalculateHSVBackground();
        }

        /// <summary>
        /// Called when slider value HSV is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnSliderValueHSVChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SliderValueHSVChanged != null)
            {
                SliderValueHSVChanged(this, e);
            }
            SliderValueHSV = (float)e.NewValue;
            if (!mouseLeftDown && this.SelectedColor != null && CanChange == true)
            {
                if (m_colorPicker == null)
                {
                    this.previousSelectedBrush = this.SelectedColor.Fill;
                }
                else if (allow)
                    this.previousSelectedBrush = this.SelectedColor.Fill;
                allow = false;
            }
            CanChange = true;

            switch (HSV)
            {
                case HSV.H:
                    H = SliderValueHSV;
                    break;

                case HSV.S:
                    S = SliderValueHSV;
                    break;

                case HSV.V:
                    V = SliderValueHSV;
                    break;

                default:
                    H = SliderValueHSV;
                    break;
            }
        }

        /// <summary>
        /// Called when modificator for HSV model is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:System.Windows.RoutedEventArgs" />
        /// instance containing the event data.</param>
        private void HSVSelected(object sender, RoutedEventArgs args)
        {
            Point selectorPosition = new Point(0, 0);
            switch ((sender as RadioButton).Content.ToString())
            {
                case "H":
                    HSV = HSV.H;
                    SliderMaxValueHSV = 360f;
                    SliderValueHSV = H;
                    selectorPosition.X = GetXPositionForH();
                    selectorPosition.Y = GetYPositionForHS();
                    break;

                case "S":
                    HSV = HSV.S;
                    SliderMaxValueHSV = 1f;
                    SliderValueHSV = S;
                    selectorPosition.X = GetXPositionForSV();
                    selectorPosition.Y = GetYPositionForHS();
                    break;

                case "V":
                    HSV = HSV.V;
                    SliderMaxValueHSV = 1f;
                    SliderValueHSV = V;
                    selectorPosition.X = GetXPositionForSV();
                    selectorPosition.Y = GetYPositionForV();
                    break;
            }

            CalculateWordKnownColorsPosition(selectorPosition);
        }

        #endregion ColorModeHSV

        /// <summary>
        /// Stores current color.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Routed Event data</param>
        private void ProcessColorPickingStart(object sender, RoutedEventArgs e)
        {
            m_colorBeforeEyeDropStart = this.Color;
        }

        /// <summary>
        /// Reverts current color to the one that was stored before eye-dropping.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Routed Event data</param>
        private void ProcessColorPickingCancel(object sender, RoutedEventArgs e)
        {
            this.Color = m_colorBeforeEyeDropStart;
        }

        /// <summary>
        /// member which has the corrdinates
        /// </summary>
        internal UpDown startx, starty, endx, endy, centrex, centrey, gradx, grady, radiusx, radiusy;

        /// <summary>
        /// member which has the mys
        /// </summary>
        private GradientStartPoint mys;

        /// <summary>
        /// member which has the RGB changed flag
        /// </summary>
        internal bool rgbChanged = false;

        /// <summary>
        /// member which has the black white flag
        /// </summary>
        internal bool blackWhitePressed = false;

        /// <summary>
        /// gradient points
        /// </summary>
        private GradientStartPoint mye, Centre, Gradientorigin;

        /// <summary>
        /// member which has the text boxes
        /// </summary>
        internal TextBox tb, rval, gval, bval, hval, sval, vval;

        /// <summary>
        /// member which has thegrid layout
        /// </summary>
        private Grid RadialGrid, LinearGrid;

        /// <summary>
        /// member which has theborder
        /// </summary>
        private Border GradBorder;

        /// <summary>
        /// member which has the path p
        /// </summary>
        internal Path p;

        /// <summary>
        /// When implemented in a derived class, will be invoked whenever
        /// application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (m_systemColors != null)
            {
                m_systemColors.Items.CurrentChanged -= new EventHandler(Items_CurrentChanged);
            }
            if (rectBar != null)
            {
                rectBar.MouseLeftButtonDown -= new MouseButtonEventHandler(rectBar_MouseLeftButtonDown);
            }
            if (popupButton != null)
            {
                popupButton.Click -= new RoutedEventHandler(popupButton_Click);
            }
            if (popupBtn != null)
            {
                popupBtn.Click -= new RoutedEventHandler(popupBtn_Click);
            }
            if (Solid != null)
            {
                Solid.Click -= new RoutedEventHandler(Solid_Click);
            }
            if (Gradient != null)
            {
                Gradient.Click -= new RoutedEventHandler(Gradient_Click);
            }
            if (linear != null)
            {
                linear.Click -= new RoutedEventHandler(linear_Click);
            }
            if (radial != null)
            {
                radial.Click -= new RoutedEventHandler(radial_Click);
            }
            if (Reverse != null)
            {
                Reverse.Click -= new RoutedEventHandler(Reverse_Click);
            }
            if (m_colorPalette != null)
            {
                m_colorPalette.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseLeftButtonDown);
                m_colorPalette.PreviewMouseMove -= new MouseEventHandler(OnMouseMove);
                m_colorPalette.MouseLeftButtonUp -= new MouseButtonEventHandler(m_colorPalette_MouseLeftButtonUp);
            }
            if (SelectedColor != null)
            {
                SelectedColor.MouseLeftButtonUp -= new MouseButtonEventHandler(SelectedColor_MouseLeftButtonDown);
            }
            if (CurrentColor != null)
            {
                CurrentColor.MouseLeftButtonUp -= new MouseButtonEventHandler(CurrentColor_MouseLeftButtonDown);
            }
            if (m_buttomH != null)
            {
                m_buttomH.Checked -= new RoutedEventHandler(HSVSelected);
            }
            if (m_buttomS != null)
            {
                m_buttomS.Checked -= new RoutedEventHandler(HSVSelected);
            }
            if (m_buttomV != null)
            {
                m_buttomV.Checked -= new RoutedEventHandler(HSVSelected);
            }
            if (hval != null)
            {
                hval.LostFocus -= new RoutedEventHandler(sval_LostFocus);
            }
            if (vval != null)
            {
                vval.LostFocus -= new RoutedEventHandler(sval_LostFocus);
            }
            if (sval != null)
            {
                sval.LostFocus -= new RoutedEventHandler(sval_LostFocus);
            }
            if (rval != null)
            {
                rval.LostFocus -= new RoutedEventHandler(tb_LostFocus);
            }
            if (gval != null)
            {
                gval.LostFocus -= new RoutedEventHandler(tb_LostFocus);
            }
            if (bval != null)
            {
                bval.LostFocus -= new RoutedEventHandler(tb_LostFocus);
            }
            if (tb != null)
            {
                tb.LostFocus -= new RoutedEventHandler(tb_LostFocus);
            }

#if SyncfusionFramework4_0
            if (m_colorPalette != null)
            {
                m_colorPalette.TouchUp -= new EventHandler<TouchEventArgs>(m_colorPalette_TouchUp);
            }

            if (m_colorPalette != null)
            {
                m_colorPalette.TouchDown -= new EventHandler<TouchEventArgs>(m_colorPalette_TouchDown);
            }
#endif

            if (m_editColorBar != null)
            {
                m_editColorBar.ColorChanged -= new PropertyChangedCallback(PickerColorBar_ColorChanged);
            }
            if (m_colorStringEditor != null)
            {
                m_colorStringEditor.LostFocus -= new RoutedEventHandler(ColorStringEditor_LostFocus);
                m_colorStringEditor.TextChanged -= new TextChangedEventHandler(m_colorStringEditor_TextChanged);
                m_colorStringEditor.MouseDown -= new MouseButtonEventHandler(m_colorStringEditor_MouseDown);
                m_colorStringEditor.GotFocus -= new RoutedEventHandler(m_colorStringEditor_GotFocus);
                m_colorStringEditor.KeyDown -= new KeyEventHandler(m_colorStringEditor_KeyDown);
            }

            base.OnApplyTemplate();

            m_colorToggleButton = FindName(C_colorToggleButton) as ToggleButton;
            m_systemColors = FindName(C_systemColors) as ComboBox;
            if (m_systemColors != null)
            {
                m_systemColors.Items.CurrentChanged += new EventHandler(Items_CurrentChanged);
            }
            p = GetTemplateChild("ColorEditPath") as Path;
            rectBar = GetTemplateChild("GradRect") as Rectangle;
            rectBar.MouseLeftButtonDown += new MouseButtonEventHandler(rectBar_MouseLeftButtonDown);
            canvasBar = GetTemplateChild("GradientBar") as Canvas;
            gradientGrid = GetTemplateChild("GridGradient") as Grid;
            linear = GetTemplateChild("linear") as Button;
            radial = GetTemplateChild("radial") as Button;
            Solid = GetTemplateChild("Solid") as Button;
            startx = GetTemplateChild("startx") as UpDown;
            starty = GetTemplateChild("starty") as UpDown;
            endx = GetTemplateChild("endx") as UpDown;
            endy = GetTemplateChild("endy") as UpDown;
            centrex = GetTemplateChild("centrex") as UpDown;
            centrey = GetTemplateChild("centrey") as UpDown;
            gradx = GetTemplateChild("gradx") as UpDown;
            grady = GetTemplateChild("grady") as UpDown;
            radiusx = GetTemplateChild("radiusx") as UpDown;
            radiusy = GetTemplateChild("radiusy") as UpDown;
            Gradient = GetTemplateChild("Gradient") as Button;
            Reverse = GetTemplateChild("Reverse") as Button;
            popupButton = GetTemplateChild("PopButton") as ToggleButton;
            if (popupButton != null)
                popupButton.Click += new RoutedEventHandler(popupButton_Click);
            popupBtn = GetTemplateChild("ButtonExt") as ToggleButton;
            if (popupBtn != null)
                popupBtn.Click += new RoutedEventHandler(popupBtn_Click);
            ColorEditBorder = GetTemplateChild("ColorEditBorder") as Border;
            enableSwitch = GetTemplateChild("EnableSwitch") as StackPanel;
            GradPopup = GetTemplateChild("GradPopup") as Popup;
            GradBorder = GetTemplateChild("GradPopup") as Border;
            RadialGrid = GetTemplateChild("RadialGrid") as Grid;
            LinearGrid = GetTemplateChild("LinearGrid") as Grid;
            m_wordKnownColorPopup = GetTemplateChild("WordKnownColorsPopup") as Popup;
            selectorEllipse = GetTemplateChild("SelectorEllipse") as Path;
            Hcount = 0;

            if (GradBorder != null)
            {
                if (GradientPropertyEditorMode == GradientPropertyEditorMode.Extended)
                {
                    if (GradBorder.Visibility == Visibility.Collapsed)
                    {
                        GradBorder.Visibility = Visibility.Visible;
                        if (LinearGrid != null)
                        {
                            LinearGrid.Visibility = Visibility.Visible;
                        }
                    }
                }
            }

            if (Solid != null)
                Solid.Click += new RoutedEventHandler(Solid_Click);
            if (Gradient != null)
                Gradient.Click += new RoutedEventHandler(Gradient_Click);
            if (linear != null)
                linear.Click += new RoutedEventHandler(linear_Click);
            if (radial != null)
                radial.Click += new RoutedEventHandler(radial_Click);
            if (Reverse != null)
                Reverse.Click += new RoutedEventHandler(Reverse_Click);
            //if(GradPopup!=null)
            //GradPopup.StaysOpen = false;
            //if (GradBorder != null)
            //    GradBorder.Visibility = Visibility.Collapsed;
            Binding b = new Binding();
            b.Source = mys;
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath("X");
            double d = mys.X;
            BindingOperations.SetBinding(startx as UpDown, UpDown.ValueProperty, b);
            Binding b1 = new Binding();
            b1.Source = mys;
            b1.Mode = BindingMode.TwoWay;
            b1.Path = new PropertyPath("Y");
            BindingOperations.SetBinding(starty as UpDown, UpDown.ValueProperty, b1);
            Binding b2 = new Binding();
            b2.Source = mye;
            b2.Mode = BindingMode.TwoWay;
            b2.Path = new PropertyPath("X");
            BindingOperations.SetBinding(endx as UpDown, UpDown.ValueProperty, b2);
            Binding b3 = new Binding();
            b3.Source = mye;
            b3.Mode = BindingMode.TwoWay;
            b3.Path = new PropertyPath("Y");
            BindingOperations.SetBinding(endy as UpDown, UpDown.ValueProperty, b3);
            Binding b4 = new Binding();
            b4.Source = Centre;
            b4.Mode = BindingMode.TwoWay;
            b4.Path = new PropertyPath("X");
            BindingOperations.SetBinding(centrex as UpDown, UpDown.ValueProperty, b4);
            Binding b5 = new Binding();
            b5.Source = Centre;
            b5.Mode = BindingMode.TwoWay;
            b5.Path = new PropertyPath("Y");
            BindingOperations.SetBinding(centrey as UpDown, UpDown.ValueProperty, b5);
            Binding b6 = new Binding();
            b6.Source = Gradientorigin;
            b6.Mode = BindingMode.TwoWay;
            b6.Path = new PropertyPath("X");
            BindingOperations.SetBinding(gradx as UpDown, UpDown.ValueProperty, b6);

            Binding b7 = new Binding();
            b7.Source = Gradientorigin;
            b7.Mode = BindingMode.TwoWay;
            b7.Path = new PropertyPath("Y");
            BindingOperations.SetBinding(grady as UpDown, UpDown.ValueProperty, b7);
            Binding b8 = new Binding();
            b8.Source = this;
            b8.Mode = BindingMode.TwoWay;
            b8.Path = new PropertyPath("RadiusX");
            BindingOperations.SetBinding(radiusx as UpDown, UpDown.ValueProperty, b8);
            Binding b9 = new Binding();
            b9.Source = this;
            b9.Mode = BindingMode.TwoWay;
            b9.Path = new PropertyPath("RadiusY");
            BindingOperations.SetBinding(radiusy as UpDown, UpDown.ValueProperty, b9);

            if (VisualizationStyle == ColorSelectionMode.HSV || VisualizationStyle == ColorSelectionMode.ClassicHSV || VisualizationStyle == ColorSelectionMode.RGB)
            {
                m_colorPalette = GetTemplateChild(C_colorPalette) as FrameworkElement;
                if (m_colorPalette != null)
                {
                    CalculateHSVSelectorPosition();
                    m_colorPalette.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                    m_colorPalette.MouseLeftButtonUp += new MouseButtonEventHandler(m_colorPalette_MouseLeftButtonUp);
                    m_colorPalette.PreviewMouseMove += new MouseEventHandler(OnMouseMove);
#if SyncfusionFramework4_0
                    if (SkinStorage.GetEnableTouch(this))
                    {
                        m_colorPalette.TouchUp += new EventHandler<TouchEventArgs>(m_colorPalette_TouchUp);
                        m_colorPalette.TouchDown += new EventHandler<TouchEventArgs>(m_colorPalette_TouchDown);
                    }
#endif
                }
                m_wordKnownColorsTextBox = GetTemplateChild(C_wordKnownColorsTextBox) as TextBox;
                //m_wordKnownColorsTextBox.Visibility = Visibility.Collapsed;
                if (VisualizationStyle != ColorSelectionMode.ClassicHSV)
                {
                    CurrentColor = GetTemplateChild("CurrentColor") as Rectangle;
                    SelectedColor = GetTemplateChild("SelectedColor") as Rectangle;
                    if (SelectedColor != null)
                    {
                        SelectedColor.MouseLeftButtonUp += new MouseButtonEventHandler(SelectedColor_MouseLeftButtonDown);
                    }
                    if (CurrentColor != null)
                    {
                        CurrentColor.MouseLeftButtonUp += new MouseButtonEventHandler(CurrentColor_MouseLeftButtonDown);
                    }
                }
            }

            if (VisualizationStyle == ColorSelectionMode.HSV || VisualizationStyle == ColorSelectionMode.ClassicHSV)
            {
                if (this.m_buttomH != null && this.m_buttomS != null && m_buttomV != null)
                {
                    if (this.m_buttomH.IsChecked == true)
                        CurrentHSV = "H";
                    if (this.m_buttomS.IsChecked == true)
                        CurrentHSV = "S";
                    if (this.m_buttomV.IsChecked == true)
                        CurrentHSV = "V";
                }
                m_buttomH = GetTemplateChild(C_buttomH) as RadioButton;
                m_buttomS = GetTemplateChild(C_buttomS) as RadioButton;
                m_buttomV = GetTemplateChild(C_buttomV) as RadioButton;
                if (this.m_buttomH != null && this.m_buttomS != null && m_buttomV != null)
                {
                    if (this.m_buttomH.IsChecked == false && this.m_buttomS.IsChecked == false && this.m_buttomV.IsChecked == false)
                    {
                        this.m_buttomH.IsChecked = true;
                    }
                    if (CurrentHSV == "H")
                        this.m_buttomH.IsChecked = true;
                    else if (CurrentHSV == "S")
                        this.m_buttomS.IsChecked = true;
                    else if (CurrentHSV == "V")
                        this.m_buttomV.IsChecked = true;
                }
                if (m_buttomH != null)
                {
                    m_buttomH.Checked += new RoutedEventHandler(HSVSelected);
                }
                if (m_buttomS != null)
                {
                    m_buttomS.Checked += new RoutedEventHandler(HSVSelected);
                }
                if (m_buttomV != null)
                {
                    m_buttomV.Checked += new RoutedEventHandler(HSVSelected);
                }

                hval = GetTemplateChild("Hval") as TextBox;
                sval = GetTemplateChild("Sval") as TextBox;
                vval = GetTemplateChild("Vval") as TextBox;

                if (hval != null)
                {
                    hval.LostFocus += new RoutedEventHandler(sval_LostFocus);
                }
                if (vval != null)
                {
                    vval.LostFocus += new RoutedEventHandler(sval_LostFocus);
                }
                if (sval != null)
                {
                    sval.LostFocus += new RoutedEventHandler(sval_LostFocus);
                }

                SliderS = this.GetTemplateChild("SliderS") as Slider;
                SliderV = this.GetTemplateChild("SliderV") as Slider;
                if (VisualizationStyle == ColorSelectionMode.ClassicHSV)
                {
                    tb = GetTemplateChild("AlphaVal") as TextBox;
                    rval = GetTemplateChild("Rval") as TextBox;
                    gval = GetTemplateChild("Gval") as TextBox;
                    bval = GetTemplateChild("Bval") as TextBox;
                    if (tb != null)
                    {
                        tb.LostFocus += new RoutedEventHandler(tb_LostFocus);
                    }
                    if (rval != null)
                    {
                        rval.LostFocus += new RoutedEventHandler(tb_LostFocus);
                    }
                    if (gval != null)
                    {
                        gval.LostFocus += new RoutedEventHandler(tb_LostFocus);
                    }
                    if (bval != null)
                    {
                        bval.LostFocus += new RoutedEventHandler(tb_LostFocus);
                    }
                }
            }
            else
            {
                m_editColorBar = GetTemplateChild(C_pickerColorBar) as ColorBar;
                if (m_editColorBar != null)
                {
                    m_editColorBar.ColorChanged += new PropertyChangedCallback(PickerColorBar_ColorChanged);
                }
            }

            m_colorStringEditor = GetTemplateChild(C_colorStringEditor) as TextBox;
            if (m_colorStringEditor != null)
            {
                m_colorStringEditor.LostFocus += new RoutedEventHandler(ColorStringEditor_LostFocus);
                m_colorStringEditor.TextChanged += new TextChangedEventHandler(m_colorStringEditor_TextChanged);
                m_colorStringEditor.MouseDown += new MouseButtonEventHandler(m_colorStringEditor_MouseDown);
                m_colorStringEditor.GotFocus += new RoutedEventHandler(m_colorStringEditor_GotFocus);
                m_colorStringEditor.KeyDown += new KeyEventHandler(m_colorStringEditor_KeyDown);
            }
            if (BrushMode == BrushModes.Solid)
            {
                gradientGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (gb != null)
                {
                    this.Brush = gb;
                    if (m_colorPicker != null)
                    {
                        flag = true;
                        m_colorPicker.flag = true;
                        if (m_colorPicker.IsGradientPropertyEnabled == false)
                        {
                            SolidColorBrush sb = new SolidColorBrush();
                            Gradient.Visibility = Visibility.Collapsed;
                            m_colorPicker.Brush = sb;
                        }
                        else
                        {
                            LinearGradientBrush gb1 = new LinearGradientBrush();
                            Gradient.Visibility = Visibility.Visible;
                            m_colorPicker.Brush = gb1;
                        }
                    }
                }
            }

            if (IsGradientPropertyEnabled == false)
            {
                if (Gradient != null)
                    Gradient.Visibility = Visibility.Collapsed;
            }
            //if(GradPopup!=null)
            //GradPopup.IsOpen = false;
            //if (GradBorder != null)
            //    GradBorder.Visibility = Visibility.Collapsed;
            if (gradientItemCollection == null)
            {
                gradientItemCollection = new GradientItemCollection();
                this.applygradient();
            }
            else
            {
                if (canvasBar != null)
                {
                    canvasBar.Width = this.rectBar.Width;
                }

                foreach (GradientStopItem i in gradientItemCollection.Items)
                {
                    try
                    {
                        canvasBar.Children.Add(i.gradientitem);
                    }
                    catch { }
                }
                fillGradient(this.gradientItemCollection.gradientItem);
            }
        }
        void m_colorPalette_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EnableToolTip)
            {
                if (this.m_wordKnownColorPopup != null)
                    this.m_wordKnownColorPopup.IsOpen = false;
                if (m_wordKnownColorsTextBox != null)
                {
                    m_wordKnownColorsTextBox.Visibility = Visibility.Collapsed;
                }
            }
        }
#if SyncfusionFramework4_0

        private void m_colorPalette_TouchUp(object sender, TouchEventArgs e)
        {
            if (!selectorEllipse.IsVisible && selectorEllipse != null)
            {
                selectorEllipse.Visibility = Visibility.Collapsed;
            }
        }

        private void m_colorPalette_TouchDown(object sender, TouchEventArgs e)
        {
            if (!selectorEllipse.IsVisible && selectorEllipse != null)
            {
                selectorEllipse.Visibility = Visibility.Visible;
            }
        }

#endif

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the SelectedColor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void SelectedColor_MouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            SwapColors();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the CurrentColor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        internal void CurrentColor_MouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            SwapColor();
        }

        /// <summary>
        /// Swaps the colors.
        /// </summary>
        internal void SwapColors()
        {
            if (CurrentColor != null && SelectedColor != null)
            {
                CurrentColor.Fill = SelectedColor.Fill;
            }
        }

        /// <summary>
        /// Swaps the color.
        /// </summary>
        internal void SwapColor()
        {
            if (SelectedColor != null && CurrentColor != null)
            {
                CanChange = false;
                SelectedColor.Fill = CurrentColor.Fill;
                this.Brush = this.SelectedColor.Fill;
                if (m_colorPicker != null)
                {
                    m_colorPicker.Brush = this.Brush;
                }
            }
        }

        /// <summary>
        /// Sets the brush.
        /// </summary>
        /// <param name="gbrush">The gbrush.</param>
        internal void SetBrush(GradientBrush gbrush)
        {
            if (gradientItemCollection == null)
                gradientItemCollection = new GradientItemCollection();

            if (gbrush != null)
            {
                if (gbrush is LinearGradientBrush)
                {
                    isLinear = true;
                }
                else
                {
                    isLinear = false;
                }
                foreach (GradientStop stop in gbrush.GradientStops)
                {
                    GradientStopItem newitem = new GradientStopItem(stop.Color, true, stop.Offset, this);
                    newitem.gradientitem.SetValue(Canvas.TopProperty, 20d);
                    if (null != rectBar)
                        newitem.gradientitem.SetValue(Canvas.LeftProperty, stop.Offset * rectBar.Width);

                    gradientItemCollection.Items.Add(newitem);
                    gradientItemCollection.gradientItem = newitem;
                }
                if (rectBar != null)
                    gradientItemCollection.gradientItem.gradientitem.SetValue(Canvas.LeftProperty, rectBar.Width);
            }
            else
            {
                if (Brush is SolidColorBrush)
                {
                    GradientStopItem newitem = new GradientStopItem(((SolidColorBrush)Brush).Color, true, 0, this);
                    newitem.gradientitem.SetValue(Canvas.TopProperty, 20d);
                    gradientItemCollection.Items.Add(newitem);
                    gradientItemCollection.gradientItem = newitem;
                }
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the m_colorStringEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void m_colorStringEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            mousedown = true;
        }

        /// <summary>
        /// Handles the MouseDown event of the m_colorStringEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void m_colorStringEditor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousedown = true;
        }

        /// <summary>
        /// Handles the LostFocus event of the sval control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void sval_LostFocus(object sender, RoutedEventArgs e)
        {
            string str = (sender as TextBox).Text;
            if (str == String.Empty)
            {
                (sender as TextBox).Text = "1.00";
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the tb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            string str = (sender as TextBox).Text;
            if (str == String.Empty)
            {
                (sender as TextBox).Text = "255";
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the m_colorStringEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void m_colorStringEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (((sender as TextBox).Text.Length < 9 && (sender as TextBox).Text != ""))
            //{
            //    Edited = true;
            //}
            if (mousedown)
            {
                Edited = true;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the mys control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void mys_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Startpoint = new Point((sender as GradientStartPoint).X, (sender as GradientStartPoint).Y);
        }

        /// <summary>
        /// Handles the Click event of the popupBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void popupBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GradBorder != null)
            {
                if (GradBorder.Visibility == Visibility.Collapsed)
                {
                    if (this.gradientItemCollection.Items.Count > 0)
                    {
                        GradBorder.Visibility = Visibility.Visible;
                        // isLinear = true;
                        // this.fillGradient(this.gradientItemCollection.gradientItem);
                        if (isLinear)
                        {
                            LinearGrid.Visibility = Visibility.Visible;
                            RadialGrid.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            LinearGrid.Visibility = Visibility.Collapsed;
                            RadialGrid.Visibility = Visibility.Visible;
                        }
                    }
                    //    GeneralTransform gt;
                    //    Point offset;
                    //    //  popupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                    //    gt = GradPopup.TransformToVisual(Application.Current.MainWindow as UIElement);
                    //    offset = gt.Transform(new Point(0, 0));
                    //    GradPopup.Margin = new Thickness(0, (ColorEditBorder.ActualHeight), 0, 0);
                }
                else
                {
                    GradBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the popupButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void popupButton_Click(object sender, RoutedEventArgs e)
        {
            if (GradPopup != null)
            {
                if (GradPopup.IsOpen != true)
                {
                    GradPopup.IsOpen = true;
                    if (this.gradientItemCollection.Items.Count > 0)
                    {
                        // isLinear = true;
                        // this.fillGradient(this.gradientItemCollection.gradientItem);
                        if (isLinear)
                        {
                            LinearGrid.Visibility = Visibility.Visible;
                            RadialGrid.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            LinearGrid.Visibility = Visibility.Collapsed;
                            RadialGrid.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    GradPopup.IsOpen = false;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Gradient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Gradient_Click(object sender, RoutedEventArgs e)
        {
            gradientGrid.Visibility = Visibility.Visible;
            if (GradientPropertyEditorMode == GradientPropertyEditorMode.Extended)
            {
                GradBorder.Visibility = Visibility.Visible;
                LinearGrid.Visibility = Visibility.Visible;
                RadialGrid.Visibility = Visibility.Collapsed;
            }
            this.BrushMode = BrushModes.Gradient;
            this.Brush = gb;
            if (m_colorPicker != null)
            {
                //flag = true;
                m_colorPicker.flag = true;
                m_colorPicker.Brush = gb;
            }
        }

        /// <summary>
        /// Handles the Click event of the Solid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Solid_Click(object sender, RoutedEventArgs e)
        {
            gradientGrid.Visibility = Visibility.Collapsed;
            if (GradPopup != null)
                GradPopup.IsOpen = false;
            if (GradBorder != null)
                GradBorder.Visibility = Visibility.Collapsed;
            this.BrushMode = BrushModes.Solid;
            this.Brush = new SolidColorBrush(this.Color);
            if (m_colorPicker != null)
            {
                //flag = true;
                m_colorPicker.flag = true;
                m_colorPicker.BrushMode = BrushModes.Solid;
                m_colorPicker.Brush = new SolidColorBrush(this.Color);
            }
        }

        /// <summary>
        /// member which has the rev flag
        /// </summary>
        internal bool rev = false;

        /// <summary>
        /// Handles the Click event of the Reverse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Reverse_Click(object sender, RoutedEventArgs e)
        {
            if (this.gradientItemCollection.Items.Count > 0)
            {
                this.gradientItemCollection.Items.SortDescriptions.Add(new SortDescription("gradientItem", ListSortDirection.Descending));
                // this.gradientItemCollection.ItemsSource=(this.gradientItemCollection.ItemsSource.GetType(GradientStopItem)
                //List<GradientStopItem> list = new List<GradientStopItem>();
                //for (int i = 0; i < gradientItemCollection.Items.Count; i++)
                //{
                //    list.Add(this.gradientItemCollection.Items[i] as GradientStopItem);
                //}
                //list.Reverse();
                //this.gradientItemCollection.Items.Clear();

                //this.gradientItemCollection.Items = list;
                ////{
                //    (gradientItemCollection.Items[i] as GradientStopItem).offset = 1 - (gradientItemCollection.Items[i] as GradientStopItem).offset;
                //    (gradientItemCollection.Items[i] as GradientStopItem).gradientitem.SetValue(Canvas.LeftProperty, rectBar.Width * (gradientItemCollection.Items[i] as GradientStopItem).offset);
                //}
                rev = true;
                this.fillGradient(this.gradientItemCollection.gradientItem);
            }
        }

        /// <summary>
        /// Handles the Click event of the radial control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void radial_Click(object sender, RoutedEventArgs e)
        {
            if (this.gradientItemCollection.Items.Count > 0)
            {
                isLinear = false;
                this.fillGradient(this.gradientItemCollection.gradientItem);
                LinearGrid.Visibility = Visibility.Collapsed;
                RadialGrid.Visibility = Visibility.Visible;
            }
            if (GradPopup != null)
                GradPopup.IsOpen = false;
            if (GradBorder != null)
            {
                if (GradientPropertyEditorMode == GradientPropertyEditorMode.Extended)
                {
                    GradBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    GradBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the linear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void linear_Click(object sender, RoutedEventArgs e)
        {
            if (this.gradientItemCollection.Items.Count > 0)
            {
                isLinear = true;
                this.fillGradient(this.gradientItemCollection.gradientItem);
                LinearGrid.Visibility = Visibility.Visible;
                RadialGrid.Visibility = Visibility.Collapsed;
            }
            if (GradPopup != null)
                GradPopup.IsOpen = false;
            if (GradBorder != null)
            {
                if (GradientPropertyEditorMode == GradientPropertyEditorMode.Extended)
                {
                    GradBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    GradBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the rectBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void rectBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double offset = e.GetPosition(this.canvasBar).X / rectBar.Width;
            GradientStopItem gs = new GradientStopItem(this.Color, true, offset, this);
            gs.gradientitem.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasBar).X);
            gs.gradientitem.SetValue(Canvas.TopProperty, rectBar.Height + 4);
            this.canvasBar.Children.Add(gs.gradientitem);
            gradientItemCollection.Items.Add(gs);
            gradientItemCollection.gradientItem = gs;
            fillGradient(gs);
        }

        /// <summary>
        /// Removes the selection.
        /// </summary>
        internal void RemoveSelection()
        {
            for (int i = 0; i < gradientItemCollection.Items.Count; i++)
            {
                GradientStopItem gss = (GradientStopItem)gradientItemCollection.Items[i];
                gss.isselected = false;
                //((System.Windows.Shapes.Path)gss.gradientitem.Children[0]).Stroke = null;
                gss.gradientitem.SetValue(Canvas.ZIndexProperty, 0);
            }
        }

        /// <summary>
        /// member which has the gb
        /// </summary>
        private GradientBrush gb;

        /// <summary>
        /// Fills the gradient.
        /// </summary>
        /// <param name="gradstop">The gradstop.</param>
        internal void fillGradient(GradientStopItem gradstop)
        {
            if (isLinear)
            {
                Point x = this.Startpoint;
                gb = new LinearGradientBrush() { };
                //LinearGradientBrush gs = new LinearGradientBrush();

                Binding b = new Binding();
                b.Source = this;
                b.Mode = BindingMode.TwoWay;
                b.Path = new PropertyPath("StartPoint");
                //b.Converter = new DoubleToPointConverter();
                //b.ConverterParameter = this.starty.Value;
                BindingOperations.SetBinding(gb as LinearGradientBrush, LinearGradientBrush.StartPointProperty, b);

                Binding b1 = new Binding();
                b1.Source = this;
                b1.Mode = BindingMode.TwoWay;
                b1.Path = new PropertyPath("EndPoint");
                //b.Converter = new DoubleToPointConverter();
                //b.ConverterParameter = this.starty.Value;
                BindingOperations.SetBinding(gb as LinearGradientBrush, LinearGradientBrush.EndPointProperty, b1);

                //Binding b1 = new Binding();
                //b1.Source = this.starty;
                //b1.Mode = BindingMode.OneWay;
                //b1.Path = new PropertyPath("Value");
                //b1.Converter = new DoubleToPointConverterY();
                //b1.ConverterParameter = this.startx.Value;
                //BindingOperations.SetBinding(gb as LinearGradientBrush, LinearGradientBrush.StartPointProperty, b1);
            }
            else
            {
                gb = new RadialGradientBrush() { };
                Binding b = new Binding();
                b.Source = this;
                b.Mode = BindingMode.TwoWay;
                b.Path = new PropertyPath("CentrePoint");
                //b.Converter = new DoubleToPointConverter();
                //b.ConverterParameter = this.starty.Value;
                BindingOperations.SetBinding(gb as RadialGradientBrush, RadialGradientBrush.CenterProperty, b);

                Binding b1 = new Binding();
                b1.Source = this;
                b1.Mode = BindingMode.TwoWay;
                b1.Path = new PropertyPath("GradientOrigin");
                //b.Converter = new DoubleToPointConverter();
                //b.ConverterParameter = this.starty.Value;
                BindingOperations.SetBinding(gb as RadialGradientBrush, RadialGradientBrush.GradientOriginProperty, b1);
                Binding b2 = new Binding();
                b2.Source = this;
                b2.Mode = BindingMode.TwoWay;
                b2.Path = new PropertyPath("RadiusX");
                //b.Converter = new DoubleToPointConverter();
                //b.ConverterParameter = this.starty.Value;
                BindingOperations.SetBinding(gb as RadialGradientBrush, RadialGradientBrush.RadiusXProperty, b2);
                Binding b3 = new Binding();
                b3.Source = this;
                b3.Mode = BindingMode.TwoWay;
                b3.Path = new PropertyPath("RadiusY");
                //b.Converter = new DoubleToPointConverter();
                //b.ConverterParameter = this.starty.Value;
                BindingOperations.SetBinding(gb as RadialGradientBrush, RadialGradientBrush.RadiusYProperty, b3);
            }

            for (int i = 0; i < gradientItemCollection.Items.Count; i++)
            {
                GradientStopItem gss = (GradientStopItem)gradientItemCollection.Items[i];
                GradientStop gs = new GradientStop();
                if (gradstop == gss || this.gradientItemCollection.gradientItem == gss)
                {
                    gss.isselected = true;
                    this.gradientItemCollection.gradientItem = (GradientStopItem)gradientItemCollection.Items[i];
                    //((System.Windows.Shapes.Path)gss.gradientitem.Children[0]).Stroke =new SolidColorBrush(this.Color);
                    ((gss.gradientitem.Children[0]) as Control).SetValue(Canvas.ZIndexProperty, 1);
                }
                else
                {
                    gss.isselected = false;
                    //  ((System.Windows.Shapes.Path)gss.gradientitem.Children[0]).Stroke = null;
                    ((gss.gradientitem.Children[0]) as Control).SetValue(Canvas.ZIndexProperty, 0);
                }

                gs.Color = gss.color;
                if (rev)
                {
                    gss.offset = 1d - gss.offset;
                    gss.gradientitem.SetValue(Canvas.LeftProperty, gss.offset * rectBar.Width);
                }
                gs.Offset = gss.offset;

                gb.GradientStops.Add(gs);

            }
            rev = false;

            if (null != rectBar)
            {
                rectBar.Fill = gb;
            }

            if (!(this.BrushMode == BrushModes.Solid))
            {
                this.Brush = gb;
                if (m_colorPicker != null)
                {
                    if (!bindedmanually)
                    {
                        flag = true;
                        m_colorPicker.Brush = gb;
                    }
                    else
                    {
                        bindedmanually = false;
                    }
                }
            }
        }

        /// <summary>
        /// Applygradients this instance.
        /// </summary>
        private void applygradient()
        {
            canvasBar.Width = this.rectBar.Width;
            GradientStopItem gs = new GradientStopItem(Colors.Black, true, 1.0, this);
            //gs.cedit = this;
            GradientStopItem gs1 = new GradientStopItem(this.Color, true, 0.0, this);
            //gs1.cedit = this;
            gs.gradientitem.SetValue(Canvas.LeftProperty, rectBar.Width);
            gs.gradientitem.SetValue(Canvas.TopProperty, rectBar.Height + 4);
            gs1.gradientitem.SetValue(Canvas.LeftProperty, 0d);
            gs1.gradientitem.SetValue(Canvas.TopProperty, rectBar.Height + 4);
            //(gs1.gradientitem.Children[0] as Path).Stroke = new SolidColorBrush(Colors.Blue);
            //canvasBar.Children.Add(gs.gradientitem);
            //canvasBar.Children.Add(gs1.gradientitem);
            gradientItemCollection.Items.Add(gs);
            gradientItemCollection.Items.Add(gs1);
            gradientItemCollection.gradientItem = gs1;
            //canvasBar.Children.Add(gs.gradientitem);
            //canvasBar.Children.Add(gs1.gradientitem);
            foreach (GradientStopItem i in gradientItemCollection.Items)
            {
                canvasBar.Children.Add(i.gradientitem);
            }
            this.fillGradient(gs1);
        }

        /// <summary>
        /// Invoked whenever the color bar color is changed.
        /// </summary>
        /// <param name="d">Sender of the event.</param>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        private void PickerColorBar_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Color = (Color)e.NewValue;
        }

        /// <summary>
        /// Invoked when PART_ColorStringEditor visual child Key down.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Routed Event data</param>
        private void m_colorStringEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                m_colorStringEditor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        /// <summary>
        /// Invoked when PART_ColorStringEditor visual child lost focus.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Routed Event data</param>
        private void ColorStringEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            Color colorValue = Color;
            colorValue = Color.FromArgb(colorValue.A, colorValue.R, colorValue.G, colorValue.B);
            m_colorStringEditor.Text = colorValue.ToString();
        }

        /// <summary>
        /// Handles the CurrentChanged event of the ComboBox.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event data</param>
        private void Items_CurrentChanged(object sender, EventArgs e)
        {
            if (m_colorToggleButton != null)
            {
                ColorItem colorItem = (ColorItem)m_systemColors.SelectedValue;
                //if (m_colorToggleButton.IsChecked == true)
                //{
                //if(Color == colorItem.Brush.Color)
                //Color = colorItem.Brush.Color;
                //}
                if (changeColor)
                {
                    if (changeHSVBackground)
                        Color = colorItem.Brush.Color;
                }
                else if (this.m_colorPicker != null)
                {
                    this.m_colorPicker.SelectPaletteColor();
                }
                UpdateColorBarSlider();
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of the ColorEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs" />
        /// instance containing the event data.</param>
        private void ColorEdit_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (VisualizationStyle == ColorSelectionMode.HSV || VisualizationStyle == ColorSelectionMode.RGB || VisualizationStyle == ColorSelectionMode.ClassicHSV && m_colorPalette != null)
            {
                CalculateHSVSelectorPosition();
                //if (VisualizationStyle != ColorSelectionMode.RGB)
                CalculateHSVBackground();
                //else
                //    CalculateBackground();
            }
        }

        /// <summary>
        /// member which has the blackwhite flag
        /// </summary>
        internal bool blackWhite = false;

        /// <summary>
        /// member which has the loaded flag
        /// </summary>
        internal bool loaded = false;

        /// <summary>
        /// member which has the edited flag
        /// </summary>
        internal bool Edited = false;

        /// <summary>
        /// member which has the mouse down flag
        /// </summary>
        internal bool mousedown = false;

        /// <summary>
        /// Raises VisualizationStyleChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnVisualizationStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            m_visualizationStyle = (ColorSelectionMode)e.NewValue;

            if (m_colorPalette != null)
            {
                if (VisualizationStyle == ColorSelectionMode.RGB || VisualizationStyle == ColorSelectionMode.ClassicRGB)
                {
                    CalculateBackground();
                    UpdateColorBarSlider();
                }

                if (VisualizationStyle == ColorSelectionMode.HSV || VisualizationStyle == ColorSelectionMode.ClassicHSV && m_colorPalette != null && m_bNeedChangeHSV)
                {
                    CalculateHSVSelectorPosition();
                }
            }
            if (m_visualizationStyle == ColorSelectionMode.HSV)
            {
                //m_color.ScA = A;
                //m_color.ScG = G;
                //m_color.ScR = R;
                //m_color.ScB = B;
                //Color = m_color;
                CalculateHSVBackground();

                // hsventered = true;
            }

            //if (GradPopup != null
            //{
            //    GradPopup.IsOpen = false;
            //    //GradPopup.StaysOpen = true;
            //}
            if (GradBorder != null)
            {
                if (GradientPropertyEditorMode == GradientPropertyEditorMode.Extended)
                {
                    GradBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    GradBorder.Visibility = Visibility.Collapsed;
                }
            }
            if (VisualizationStyleChanged != null)
            {
                VisualizationStyleChanged(this, e);
            }
            if (gradientItemCollection != null && gradientItemCollection.Items.Count != 0)
            {
                if (canvasBar != null)
                {
                    canvasBar.Children.Clear();
                }
            }
        }

        /// <summary>
        /// Raises ColorChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Allow && (VisualizationStyle == ColorSelectionMode.RGB || VisualizationStyle == ColorSelectionMode.ClassicRGB))
                A_value = m_color.ScA;
            if (m_colorPalette == null)
                m_color = (Color)e.NewValue;

            else
            {
                if (Edited || mousedown)
                {
                    m_color = (Color)e.NewValue;
                    mousedown = false;
                    Edited = false;
                }
                else
                {
                    if ((!loaded) && VisualizationStyle == ColorSelectionMode.HSV || VisualizationStyle == ColorSelectionMode.ClassicHSV)
                    {
                        m_color = Color;
                        //m_color.ScA = m_color.A;
                        if (rgbChanged)
                        {
                            m_color = (Color)e.NewValue;
                        }
                        else
                        {
                            Color = m_color;
                        }
                        loaded = true;
                    }
                    else
                    {
                        m_color = (Color)e.NewValue;
                        if (VisualizationStyle == ColorSelectionMode.RGB || VisualizationStyle == ColorSelectionMode.ClassicRGB)
                        {
                            if (A_value != 255 && A_value != 0)
                            {
                                m_color.ScA = A_value;
                            }
                        }
                        if (!blackWhitePressed)
                        {
                            //m_color.ScA = m_color.A;
                        }
                        blackWhitePressed = false;
                        Color = m_color;

                        breakLoop = false;
                    }
                }
            }

            if (!breakLoop || VisualizationStyle == ColorSelectionMode.HSV || VisualizationStyle == ColorSelectionMode.ClassicHSV || VisualizationStyle == ColorSelectionMode.ClassicRGB || VisualizationStyle == ColorSelectionMode.RGB)
                ChangetheColorValues(e);
            else
                breakLoop = false;
        }

        /// <summary>
        /// Changethes the color values.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ChangetheColorValues(DependencyPropertyChangedEventArgs e)
        {
            if (this.m_colorPicker != null)
            {
                this.BrushMode = this.m_colorPicker.BrushMode;
            }
            if (!(this.BrushMode == BrushModes.Gradient))
            {
                Brush = new SolidColorBrush(m_color);
                if (m_colorPicker != null)
                {
                    m_colorPicker.flag = true;
                    flag = true;
                    m_colorPicker.Brush = new SolidColorBrush(m_color);
                }
            }
            Edited = false;

            m_bColorUpdating = true;

            m_r = m_color.ScR;
            m_g = m_color.ScG;
            m_b = m_color.ScB;
            m_a = m_color.ScA;
            R = m_color.ScR;
            G = m_color.ScG;
            B = m_color.ScB;
            A = m_color.ScA;

            if (VisualizationStyle == ColorSelectionMode.RGB || VisualizationStyle == ColorSelectionMode.ClassicRGB)
            {
                //if (count > 1)
                CalculateHSVSelectorPosition();
                CalculateBackground();
            }

            if (VisualizationStyle == ColorSelectionMode.HSV || VisualizationStyle == ColorSelectionMode.RGB || VisualizationStyle == ColorSelectionMode.ClassicHSV && m_colorPalette != null && m_bNeedChangeHSV)
            {
                //if (count > 1)
                CalculateHSVSelectorPosition();
                CalculateHSVBackground();
            }

            InvertColor = Color.FromRgb((byte)(255 - m_color.R), (byte)(255 - m_color.G), (byte)(255 - m_color.B));

            if (ColorChanged != null)
            {
                if (this.A == ((Color)e.NewValue).ScA)
                    ColorChanged(this, e);
            }

            m_bColorUpdating = false;
            if (!setnocolor)
            {
                if (this.gradientItemCollection != null)
                {
                    if (this.gradientItemCollection.gradientItem != null)
                    {
                        // (this.gradientItemCollection.gradientItem.gradientitem.Children[0] as Path).Fill = new SolidColorBrush(this.Color);
                        this.gradientItemCollection.gradientItem.color = this.Color;
                        this.fillGradient(this.gradientItemCollection.gradientItem);
                    }
                }
            }
            else
            {
                setnocolor = false;
            }

            if (m_systemColors != null)
            {
                ComboBox obj = m_systemColors as ComboBox;
                //changeHSVBackground = true;
                //Hcount = 0;
                if (obj != null)
                {
                    obj.DropDownOpened += new EventHandler(obj_DropDownOpened);
                    IList<ColorItem> list = obj.ItemsSource as IList<ColorItem>;
                    int index = -1;
                    if (list != null)
                    {
                        for (int i = 0, cnt = list.Count; i < cnt; i++)
                        {
                            ColorItem item = list[i] as ColorItem;

                            if (item.Name == ColorEdit.SuchColor((Color)e.NewValue)[0])
                            {
                                index = i;
                                count = 0;
                                allow = true;
                                break;
                            }
                        }

                        if (index != -1)
                        {
                            if (obj.SelectedIndex != index)
                            {
                                obj.SelectedIndex = index;
                            }
                        }
                    }
                }
            }
            UpdateColorBarSlider();

            breakLoop = true;
        }

        private void obj_DropDownOpened(object sender, EventArgs e)
        {
            changeHSVBackground = true;
            changeColor = true;
            Hcount = 0;
        }

        internal bool setnocolor = false;

        /// <summary>
        /// Calculates position of the selector in HSV.
        /// </summary>
        internal void CalculateHSVSelectorPosition()
        {
            HsvColor hsv = HsvColor.ConvertRgbToHsv(m_color.R, m_color.B, m_color.G);

            if (changeHSVBackground)
            {
                if (hsv.H != 0 || ((m_color.R != 0) || (m_color.G != 0) || (m_color.B != 0)))
                {
                    if (hsv.H == 0)
                    {
                        if (m_colorPicker == null && m_color.R != 0)
                            H = (float)hsv.H;
                        else
                        {
                            H = (float)hsv.H;
                            if (this.selectedColor != null)
                                this.selectedColor.Fill = this.Brush;
                            Hcount = 0;
                        }
                    }
                    else if (Hcount == 0)
                    {
                        H = (float)hsv.H;
                        Hcount++;
                    }
                }
                if (hsv.S != 0)
                    S = (float)hsv.S;
                if (hsv.V != 0)
                    V = (float)hsv.V;
            }
            else if (m_colorPicker == null)
            {
                if (H == 0)
                    H = (float)hsv.H;
                else if (m_color.R != 0 || m_color.G != 0 || m_color.B != 0)
                {
                    if (!mouseLeftDown)
                        H = (float)hsv.H;
                }
                if (S == 0)
                    S = (float)hsv.S;
                if (V == 0)
                    V = (float)hsv.V;
            }
            else
            {
                if (hsv.H != 0 || ((m_color.R > m_color.G) && (m_color.R > m_color.B)))
                {
                    if (Hcount == 0)
                    {
                        if (H == 0)
                            H = (float)hsv.H;
                        else if (m_color.R != 0 || m_color.G != 0 || m_color.B != 0)
                        {
                            if (!mouseLeftDown)
                                H = (float)hsv.H;
                        }
                        Hcount++;
                    }
                }
                if (hsv.S != 0)
                {
                    if (S == 0)
                        S = (float)hsv.S;
                }
                if (hsv.V != 0)
                    V = (float)hsv.V;
            }

            Point selectorPosition = new Point(0, 0);
            switch (HSV)
            {
                case HSV.H:
                    selectorPosition.X = GetXPositionForH();
                    selectorPosition.Y = GetYPositionForHS();
                    SliderValueHSV = H;
                    //Hcount++;
                    break;

                case HSV.S:
                    selectorPosition.X = GetXPositionForSV();
                    selectorPosition.Y = GetYPositionForHS();
                    SliderValueHSV = S;
                    break;

                case HSV.V:
                    selectorPosition.X = GetXPositionForSV();
                    selectorPosition.Y = GetYPositionForV();
                    SliderValueHSV = V;
                    break;
            }

            CalculateWordKnownColorsPosition(selectorPosition);
        }

        /// <summary>
        /// Gets X selector position in the color palette for S and V.
        /// </summary>
        /// <returns>X selector position for S and V</returns>
        private float GetXPositionForSV()
        {
            if (m_colorPalette != null)
            {
                return (H / 3.6f) * ((float)m_colorPalette.ActualWidth / 100);
            }
            else
                return 0;
        }

        /// <summary>
        /// Gets Y selector position in the color palette for V.
        /// </summary>
        /// <returns>Y selector position for V</returns>
        private float GetYPositionForV()
        {
            if (m_colorPalette != null)
            {
                return (float)m_colorPalette.ActualHeight - (S * (float)m_colorPalette.ActualHeight);
            }
            else
                return 0;
        }

        /// <summary>
        /// Gets Y selector position in the color palette for H and S.
        /// </summary>
        /// <returns>Y selector position for H and S</returns>
        private float GetYPositionForHS()
        {
            if (m_colorPalette != null)
            {
                return (float)m_colorPalette.ActualHeight - (V * (float)m_colorPalette.ActualHeight);
            }
            else
                return 0;
        }

        /// <summary>
        /// Gets X selector position in the color palette for H.
        /// </summary>
        /// <returns>X selector position for H</returns>
        private float GetXPositionForH()
        {
            if (m_colorPalette != null)
            {
                return S * (float)m_colorPalette.ActualWidth;
            }
            else
                return 0;
        }

        /// <summary>
        /// Called when mouse left button down.
        /// </summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />
        /// instance containing the event data.</param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            changeHSVBackground = false;
            Hcount = 0;
            mouseLeftDown = true;
            this.Focus();
            Point p = e.GetPosition(m_colorPalette);

            if (this.SelectedColor != null)
                this.previousSelectedBrush = this.SelectedColor.Fill;

            if (m_colorPalette == null)
            {
                return;
            }
            m_bColorUpdating = true;

            m_bNeedChangeHSV = false;
            p = FindColorHSV(p);
            m_bNeedChangeHSV = true;
            Color = HsvColor.ConvertHsvToRgb(H, S, V);
            if (this.SelectedColor != null)
                this.SelectedColor.Fill = this.Brush;

            CalculateWordKnownColorsPosition(p);

            if (SkinStorage.GetEnableTouch(this))
            {
                if (selectorEllipse != null)
                {
                    selectorEllipse.Visibility = Visibility.Visible;
                }
            }
            count++;
            mouseLeftDown = false;
        }

        /// <summary>
        /// Called when mouse moves.
        /// </summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" />
        /// instance containing the event data.</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.SelectedColor != null)
                    this.previousSelectedBrush = this.SelectedColor.Fill;
                changeHSVBackground = false;

                mouseLeftDown = true;
                Hcount = 0;
                Point p = e.GetPosition(m_colorPalette);

                if (p.X > m_colorPalette.ActualWidth)
                {
                    p.X = m_colorPalette.ActualWidth;
                    m_colorPalette.ReleaseMouseCapture();
                }

                if (p.X < 0)
                {
                    p.X = 0;
                    m_colorPalette.ReleaseMouseCapture();
                }

                if (p.Y > m_colorPalette.ActualHeight)
                {
                    p.Y = m_colorPalette.ActualHeight;
                    m_colorPalette.ReleaseMouseCapture();
                }

                if (p.Y < 0)
                {
                    p.Y = 0;
                    m_colorPalette.ReleaseMouseCapture();
                }
                m_bNeedChangeHSV = false;
                p = FindColorHSV(p);
                m_bNeedChangeHSV = true;

                Color = HsvColor.ConvertHsvToRgb(H, S, V);
                if (this.EnableToolTip == true && this.m_wordKnownColorPopup != null)
                {
                    this.m_wordKnownColorPopup.IsOpen = true;
                }
                CalculateWordKnownColorsPosition(p);
            }
            else
                mouseLeftDown = false;
        }

        /// <summary>
        /// Gets H or S or V color.
        /// </summary>
        /// <param name="p">Gets the Point in the color palette</param>
        /// <returns>Returns the Point in the color palette</returns>
        private Point FindColorHSV(Point p)
        {
            switch (HSV)
            {
                case HSV.H:
                    FindColorH(p);
                    break;

                case HSV.S:
                    FindColorS(p);
                    break;

                case HSV.V:
                    FindColorV(p);
                    break;

                default:
                    FindColorH(p);
                    break;
            }

            return p;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" />routed
        /// event is raised on this element. This method implements to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />that
        /// contains the event data. The event data
        /// reports that the left mouse button was
        /// pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (m_colorPalette != null && m_colorPalette.IsMouseOver)
            {
                m_colorPalette.CaptureMouse();
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (EnableToolTip)
            {
                if (this.m_wordKnownColorPopup != null)
                    this.m_wordKnownColorPopup.IsOpen = false;
                if (m_wordKnownColorsTextBox != null)
                {
                    m_wordKnownColorsTextBox.Visibility = Visibility.Collapsed;
                }
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp" />routed
        /// event reaches an element in its route that is derived from
        /// this class. Implements this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />that
        /// contains the event data. The event data
        /// reports that the left mouse button was
        /// released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (m_colorPalette != null && m_colorPalette.IsMouseOver)
            {
                m_colorPalette.ReleaseMouseCapture();
            }

            if (SkinStorage.GetEnableTouch(this))
            {
                if (selectorEllipse != null)
                {
                    selectorEllipse.Visibility = Visibility.Collapsed;
                }
            }
            if (EnableToolTip)
            {
                if (this.m_wordKnownColorPopup != null)
                    this.m_wordKnownColorPopup.IsOpen = false;
                if (m_wordKnownColorsTextBox != null)
                {
                    m_wordKnownColorsTextBox.Visibility = Visibility.Collapsed;
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            ////if (this.IsLoaded && e.Property == SkinStorage.VisualStyleProperty)
            ////{
            ////    string style = SkinStorage.GetVisualStyle(this);
            ////    Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            ////    if (list1 != null)
            ////    {
            ////        Shared.DictionaryList list2 = list1[style] as Shared.DictionaryList;

            ////        if (SkinStorage.GetVisualStyle(this) != C_defaultSkinName)
            ////        {
            ////            if (list2.ContainsKey(C_colorEditContainerBrush))
            ////            {
            ////                Background = list2[C_colorEditContainerBrush] as Brush;
            ////            }
            ////        }
            ////        else
            ////        {
            ////            if (m_defaultBackground == null && list2.ContainsKey(C_colorEditContainerBrush))
            ////            {
            ////                Background = list2[C_colorEditContainerBrush] as Brush;
            ////            }

            ////            Background = m_defaultBackground;
            ////        }
            ////    }
            ////}
            ////else if (e.Property == ColorEdit.BackgroundProperty)
            ////{
            ////    if (SkinStorage.GetVisualStyle(this) == C_defaultSkinName)
            ////    {
            ////        m_defaultBackground = Background;
            ////    }
            ////}
        }

        #endregion Internals

        #region Help Methods

        /// <summary>
        /// Calculates the word known colors position and Selector
        /// Position.
        /// </summary>
        /// <param name="point">The point.</param>
        private void CalculateWordKnownColorsPosition(Point point)
        {
            //m_wordKnownColorsTextBox.Visibility = Visibility.Visible;
            if (m_wordKnownColorsTextBox != null)
            {
                SelectorPositionX = (float)point.X - 5;
                SelectorPositionY = (float)point.Y - 5;

                if (SkinStorage.GetEnableTouch(this))
                {
                    SelectorPositionX = (float)point.X - 45;
                    SelectorPositionY = (float)point.Y - 45;
                    if (point.Y < 0)
                    {
                        SelectorPositionY = (float)-45.0;
                    }
                    if (point.X < 0)
                    {
                        SelectorPositionX = (float)-45.0;
                    }
                    if (point.X > m_colorPalette.ActualWidth)
                    {
                        SelectorPositionX = (float)(m_colorPalette.ActualWidth + 45.0);
                    }
                    if (point.Y > m_colorPalette.ActualHeight)
                    {
                        SelectorPositionY = (float)(m_colorPalette.ActualHeight + 45.0);
                    }
                }
                if (m_colorPicker != null && !m_colorPicker.EnableToolTip)
                {
                    if (m_colorPicker.m_colorEditor.m_wordKnownColorPopup != null)
                        m_colorPicker.m_colorEditor.m_wordKnownColorPopup.IsOpen = false;
                    m_wordKnownColorsTextBox.Visibility = Visibility.Collapsed;
                }
                else if (!(this.EnableToolTip))
                {
                    if (this.m_wordKnownColorPopup != null)
                        this.m_wordKnownColorPopup.IsOpen = false;
                    m_wordKnownColorsTextBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    double wordKnownColorsTextBoxWidth = m_wordKnownColorsTextBox.ActualWidth;
                    double wordKnownColorsTextBoxHeight = m_wordKnownColorsTextBox.ActualHeight;
                    m_wordKnownColorsTextBox.Visibility = Visibility.Visible;
                    if ((point.X - wordKnownColorsTextBoxWidth - 5) > 0)
                    {
                        WordKnownColorsPositionX = (float)(point.X - wordKnownColorsTextBoxWidth - 5);
                    }
                    else
                    {
                        WordKnownColorsPositionX = (float)point.X + 5;
                    }

                    if ((point.Y - wordKnownColorsTextBoxHeight - 5) > 0)
                    {
                        WordKnownColorsPositionY = (float)(point.Y - wordKnownColorsTextBoxHeight - 5);
                    }
                    else
                    {
                        WordKnownColorsPositionY = (float)point.Y + 5;
                    }

                    ToolTip toolTip = GetColorsTooltip();
                    m_wordKnownColorsTextBox.ToolTip = toolTip;
                }
            }
        }

        /// <summary>
        /// Creates tooltip for the selected color when mouse over.
        /// </summary>
        /// <returns>Tooltip created.</returns>
        private ToolTip GetColorsTooltip()
        {
            string[] colorString = ColorEdit.SuchColor(Color);

            ToolTip toolTip = new ToolTip();
            toolTip.Background = new SolidColorBrush(Colors.Transparent);
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            SolidColorBrush textBoxBrush = new SolidColorBrush(Color.FromArgb(125, 255, 255, 255));

            TextBox suchInRed = new TextBox();
            suchInRed.Background = textBoxBrush;
            suchInRed.BorderThickness = new Thickness(0);
            suchInRed.Text = C_suchInRed + colorString[1];

            TextBox suchInGreen = new TextBox();
            suchInGreen.Background = textBoxBrush;
            suchInRed.BorderThickness = new Thickness(0);
            suchInGreen.Text = C_suchInGreen + colorString[2];

            TextBox suchInBlue = new TextBox();
            suchInBlue.Background = textBoxBrush;
            suchInRed.BorderThickness = new Thickness(0);
            suchInBlue.Text = C_suchInBlue + colorString[3];

            panel.Children.Add(suchInRed);
            panel.Children.Add(suchInGreen);
            panel.Children.Add(suchInBlue);
            toolTip.Content = panel;

            return toolTip;
        }

        /// <summary>
        /// Finds color if Hue modificator is selected.
        /// </summary>
        /// <param name="p">The point.</param>
        private void FindColorH(Point p)
        {
            V = (float)(1 - (p.Y / m_colorPalette.ActualHeight));
            S = (float)(p.X / m_colorPalette.ActualWidth);
        }

        /// <summary>
        /// Finds color if Saturation modificator is selected.
        /// </summary>
        /// <param name="p">The point.</param>
        private void FindColorS(Point p)
        {
            H = (float)(360 * p.X / m_colorPalette.ActualWidth);
            V = (float)(1f - (p.Y / m_colorPalette.ActualHeight));
        }

        /// <summary>
        /// Finds color if Value modificator is selected.
        /// </summary>
        /// <param name="p">The point.</param>
        private void FindColorV(Point p)
        {
            H = (float)(360 * p.X / m_colorPalette.ActualWidth);
            S = (float)(1 - (p.Y / m_colorPalette.ActualHeight));
        }

        /// <summary>
        /// Calculates the background for RGB model.
        /// </summary>
        internal void CalculateBackground()
        {
            Color startColor = Color.FromScRgb(m_a, 0, m_g, m_b);
            Color endColor = Color.FromScRgb(m_a, 1, m_g, m_b);

            BackgroundR = new LinearGradientBrush(startColor, endColor, 0);
            startColor = Color.FromScRgb(m_a, m_r, 0, m_b);
            endColor = Color.FromScRgb(m_a, m_r, 1, m_b);

            BackgroundG = new LinearGradientBrush(startColor, endColor, 0);
            startColor = Color.FromScRgb(m_a, m_r, m_g, 0);
            endColor = Color.FromScRgb(m_a, m_r, m_g, 1);

            BackgroundB = new LinearGradientBrush(startColor, endColor, 0);
            startColor = Color.FromScRgb(0, m_r, m_g, m_b);
            endColor = Color.FromScRgb(1, m_r, m_g, m_b);
            BackgroundA = new LinearGradientBrush(startColor, endColor, 0);
        }

        #endregion Help Methods

        internal Slider SliderS;
        internal Slider SliderV;

        /// <summary>
        ///
        /// </summary>
        public void CalculateHSVBackground()
        {
            if (SliderS != null && SliderV != null)
            {
                LinearGradientBrush huesliderbrush = new LinearGradientBrush();
                GradientStop hue1 = new GradientStop();
                hue1.Color = Colors.White;
                hue1.Offset = 0;
                huesliderbrush.GradientStops.Add(hue1);
                GradientStop hue2 = new GradientStop();
                hue2.Color = Color;
                hue2.Offset = 1;
                huesliderbrush.GradientStops.Add(hue2);
                huesliderbrush.StartPoint = new Point(0, 1);
                huesliderbrush.EndPoint = new Point(1, 1);
                SliderS.Background = huesliderbrush;
                LinearGradientBrush huesliderbrush1 = new LinearGradientBrush();
                GradientStop hue11 = new GradientStop();
                hue11.Color = Colors.Black;
                hue11.Offset = 0;
                huesliderbrush1.GradientStops.Add(hue11);
                GradientStop hue21 = new GradientStop();
                hue21.Color = Color;
                hue21.Offset = 1;
                huesliderbrush1.GradientStops.Add(hue21);
                huesliderbrush1.StartPoint = new Point(0, 1);
                huesliderbrush1.EndPoint = new Point(1, 1);
                SliderV.Background = huesliderbrush1;
            }
        }
    }

    /// <summary>
    /// Class gradient start point.
    /// </summary>
    public class GradientStartPoint : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public double X
        {
            get
            { return x; }
            set
            {
                x = value;
                NotifyPropertyChanged("X");
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                NotifyPropertyChanged("Y");
            }
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="p">The p.</param>
        private void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        /// <summary>
        /// member which has the x value
        /// </summary>
        internal double x;

        /// <summary>
        /// member which has the y value
        /// </summary>
        internal double y;

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}