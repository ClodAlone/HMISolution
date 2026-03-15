// <copyright file="ColorEdit.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Class for color generating. ColorEdit control.
    /// </summary>
    internal class xColorEdit : Control
    {
        #region Constants
        /// <summary>
        /// Contains color bar control name.
        /// </summary>
        private const string C_pickerColorBar = "PickerColorBar";

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
        #endregion

        #region Private Members

        #region ColorModeRGB
        /// <summary>
        /// Red parameter for RGB model.
        /// </summary>
        private float m_r;

        /// <summary>
        /// Green parameter for RGB model.
        /// </summary>
        private float m_g;

        /// <summary>
        /// The Blue parameter for RGB model..
        /// </summary>
        private float m_b;

        /// <summary>
        /// Alpha or opacity parameter for RGB model.
        /// </summary>
        private float m_a;

        /// <summary>
        /// Command for white color change. RGB model.
        /// </summary>
        public static RoutedCommand M_changeColorWhite;

        /// <summary>
        /// Command for black color change. RGB model.
        /// </summary>
        public static RoutedCommand M_changeColorBlack;
        #endregion

        #region ColorModeHSV
        /// <summary>
        /// Hue parameter for HSV model, value range 0 - 360.
        /// </summary>
        private float m_h = 0f;

        /// <summary>
        /// Saturation parameter for HSV model, value range 0 - 1(0 -
        /// 100%).
        /// </summary>
        private float m_s = 0f;

        /// <summary>
        /// The Value or Brightness parameter for HSV model, value range
        /// 0 - 1(0 - 100%).
        /// </summary>
        private float m_v = 1f;

        // private HSV m_hsv;
        
        /// <summary>
        /// The slider value for HSV model.
        /// </summary>
        private float m_sliderValueHSV;

        /// <summary>
        /// Color palette for HSV model.
        /// </summary>
        private FrameworkElement m_colorPalette;

        /// <summary>
        /// Text box for WordKnownColors.
        /// </summary>
        private TextBox m_wordKnownColorsTextBox;

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
        #endregion

        /// <summary>
        /// Cached value of the TestProperty property.
        /// </summary>
        private Color m_color;

        /// <summary>
        /// Cached value of the TestProperty property.
        /// </summary>
        private xColorSelectionMode m_visualizationStyle = xColorSelectionMode.RGB;

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

        /////// <summary>
        /////// Contains the default background.
        /////// </summary>
        ////private Brush m_defaultBackground = null;

        /// <summary>
        /// Contains ColorBar of the ColorEdit.
        /// </summary>
        private ColorBar m_editColorBar = null;

        /// <summary>
        /// Is need to HSv is checked
        /// </summary>
        private bool m_bNeedChangeHSV = true;
        #endregion

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
        #endregion

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
                return m_h;
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
                return m_s;
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
                return m_v;
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
                return m_sliderValueHSV;
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
        #endregion

        /// <summary>
        /// Gets or sets the value of the VisualizationStyle dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ColorSelectionMode"/>
        /// </value>
        public xColorSelectionMode VisualizationStyle
        {
            get
            {
                return m_visualizationStyle;
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
                return m_color;
            }

            set
            {
                SetValue(ColorProperty, value);
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

        #endregion

        #region Dependency Properties

        #region ColorModeRGB
        /// <summary>
        /// Identifies ColorPicker. Alpha dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty AProperty =
            DependencyProperty.Register("A", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAChanged)));

        /// <summary>
        /// Identifies ColorPicker. Red dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register("R", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnRChanged)));

        /// <summary>
        /// Identifies ColorPicker. Green dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register("G", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGChanged)));

        /// <summary>
        /// Identifies ColorPicker. Blue dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register("B", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBChanged)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundR dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundRProperty =
            DependencyProperty.Register("BackgroundR", typeof(Brush), typeof(xColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 255, 0, 0), 0)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundG dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundGProperty =
            DependencyProperty.Register("BackgroundG", typeof(Brush), typeof(xColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 255, 0), 0)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundB dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundBProperty =
            DependencyProperty.Register("BackgroundB", typeof(Brush), typeof(xColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 0, 255), 0)));

        /// <summary>
        /// Identifies ColorPicker. BackgroundA dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty BackgroundAProperty =
            DependencyProperty.Register("BackgroundA", typeof(Brush), typeof(xColorEdit), new FrameworkPropertyMetadata(new LinearGradientBrush(Color.FromArgb(0, 0, 0, 0), Color.FromArgb(255, 0, 0, 0), 0)));

        /// <summary>
        /// Identifies ColorPicker. IsScRGBColor dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty IsScRGBColorProperty =
            DependencyProperty.Register("IsScRGBColor", typeof(bool), typeof(xColorEdit), new FrameworkPropertyMetadata(false));
        #endregion

        #region ColorModeHSV
        /// <summary>
        /// DependencyProperty is used as the backing store for HSV. This
        /// enables animation, styling, binding, etc...
        /// </summary>
        /// <value>
        /// Type: <see cref="HSV"/> enum.
        /// </value>
        public static readonly DependencyProperty HSVProperty =
                DependencyProperty.Register("HSV", typeof(HSV), typeof(xColorEdit), new UIPropertyMetadata(HSV.H));

        /// <summary>
        /// Identifies ColorPicker. Hue dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty HProperty =
            DependencyProperty.Register("H", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnHChanged)));

        /// <summary>
        /// Identifies ColorPicker. Saturation dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SProperty =
            DependencyProperty.Register("S", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSChanged)));

        /// <summary>
        /// Identifies ColorPicker. Value dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty VProperty =
            DependencyProperty.Register("V", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnVChanged)));

        /// <summary>
        /// Identifies ColorPicker. SliderValueHSV dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SliderValueHSVProperty =
            DependencyProperty.Register("SliderValueHSV", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnSliderValueHSVChanged)));

        /// <summary>
        /// Identifies ColorPicker. SliderMaxValueHSV dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SliderMaxValueHSVProperty =
            DependencyProperty.Register("SliderMaxValueHSV", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(360f));

        /// <summary>
        /// Identifies ColorPicker. SelectorPositionX dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SelectorPositionXProperty =
            DependencyProperty.Register("SelectorPositionX", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(0f));

        /// <summary>
        /// Identifies ColorPicker. SelectorPositionY dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SelectorPositionYProperty =
            DependencyProperty.Register("SelectorPositionY", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(0f));

        /// <summary>
        /// Identifies ColorPicker. WordKnownColorsPositionX dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty WordKnownColorsPositionXProperty =
            DependencyProperty.Register("WordKnownColorsPositionX", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(5f));

        /// <summary>
        /// Identifies ColorPicker. WordKnownColorsPositionY dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty WordKnownColorsPositionYProperty =
            DependencyProperty.Register("WordKnownColorsPositionY", typeof(float), typeof(xColorEdit), new FrameworkPropertyMetadata(5f));
        #endregion

        /// <summary>
        /// Identifies ColorPicker. VisualizationStyle dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ColorSelectionMode"/> enum.
        /// </value>
        internal static readonly DependencyProperty VisualizationStyleProperty =
            DependencyProperty.Register("VisualizationStyle", typeof(xColorSelectionMode), typeof(xColorEdit), new FrameworkPropertyMetadata(xColorSelectionMode.RGB, new PropertyChangedCallback(OnVisualizationStyleChanged)));

        /// <summary>
        /// Identifies ColorPicker. Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(xColorEdit), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnColorChanged)));

        /// <summary>
        /// Identifies ColorPicker. Inverts color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        internal static readonly DependencyProperty InvertColorProperty =
            DependencyProperty.Register("InvertColor", typeof(Color), typeof(xColorEdit), new FrameworkPropertyMetadata(Colors.Green));
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="ColorEdit"/> class.
        /// </summary>
        static xColorEdit()
        {
            EnvironmentTest.ValidateLicense(typeof(xColorEdit));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(xColorEdit), new FrameworkPropertyMetadata(typeof(xColorEdit)));
            M_changeColorWhite = new RoutedCommand();
            M_changeColorBlack = new RoutedCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorEdit"/> class.. Creates ColorEditor.
        /// </summary>
        public xColorEdit()
        {
            Initialize();

            AddHandler(xBorderEyeDrop.BeginColorPickingEvent, new RoutedEventHandler(ProcessColorPickingStart));
            AddHandler(xBorderEyeDrop.CancelColorPickingEvent, new RoutedEventHandler(ProcessColorPickingCancel));
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            CommandBinding colorWhiteBinding = new CommandBinding(M_changeColorWhite);
            colorWhiteBinding.Executed += new ExecutedRoutedEventHandler(ChangeColorWhite);
            CommandBinding colorBlackBinding = new CommandBinding(M_changeColorBlack);
            colorBlackBinding.Executed += new ExecutedRoutedEventHandler(ChangeColorBlack);
            CommandBindings.Add(colorWhiteBinding);
            CommandBindings.Add(colorBlackBinding);
            SizeChanged += new SizeChangedEventHandler(ColorEdit_SizeChanged);
            OnColorChanged(new DependencyPropertyChangedEventArgs(ColorProperty, Color, Colors.White));
        }
        #endregion

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
        #endregion

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
        #endregion

        /// <summary>
        /// Event that is raised when VisualizationStyle property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback VisualizationStyleChanged;

        /// <summary>
        /// Event that is raised when Color property is changed.
        /// </summary>
        public event PropertyChangedCallback ColorChanged;
        #endregion

        #region Static Methods

        #region ColorModeRGB
        /// <summary>
        /// Called when parameter R is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnRChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            xColorEdit instance = (xColorEdit)d;
            instance.OnRChanged(e);
        }

        /// <summary>
        /// Called when parameter G is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnGChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            xColorEdit instance = (xColorEdit)d;
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
            xColorEdit instance = (xColorEdit)d;
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
            xColorEdit instance = (xColorEdit)d;
            instance.OnAChanged(e);
        }

        #endregion

        #region ColorModeHSV
        /// <summary>
        /// Called when parameter H is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnHChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            xColorEdit instance = (xColorEdit)d;
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
            xColorEdit instance = (xColorEdit)d;
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
            xColorEdit instance = (xColorEdit)d;
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
            xColorEdit instance = (xColorEdit)d;
            instance.OnSliderValueHSVChanged(e);
        }
        #endregion

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
            xColorEdit instance = (xColorEdit)d;
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
            xColorEdit instance = (xColorEdit)d;
            instance.OnColorChanged(e);
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

                int rgbDiff = redDiff + blueDiff + greenDiff;

                if (rgbDiff < lastMinDiff)
                {
                    resId = i;
                    lastMinDiff = rgbDiff;
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
            xKnownColor color1 = (xKnownColor)wordKnownColors[resId];
            xKnownColor color2 = (xKnownColor)wordKnownColors[resIdRed];
            xKnownColor color3 = (xKnownColor)wordKnownColors[resIdGreen];
            xKnownColor color4 = (xKnownColor)wordKnownColors[resIdBlue];

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
            numArray[135] = 1;
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
        #endregion

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
        #endregion

        #region ColorModeHSV
        /// <summary>
        /// Called when parameter H is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void OnHChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_h != (float)e.NewValue)
            {
                m_h = (float)e.NewValue;

                if (HSV == HSV.H)
                {
                    SliderValueHSV = m_h;
                }

                if (HChanged != null)
                {
                    HChanged(this, e);
                }

                if (!m_bColorUpdating)
                {
                    Color = HsvColor.ConvertHsvToRgb(m_h, m_s, m_v);
                }
            }
        }

        /// <summary>
        /// Called when parameter S is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnSChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_s != (float)e.NewValue)
            {
                if ((float)e.NewValue <= 1.0)
                {
                    m_s = (float)e.NewValue;

                    if (HSV == HSV.S)
                    {
                        SliderValueHSV = m_s;
                    }

                    if (SChanged != null)
                    {
                        SChanged(this, e);
                    }

                    if (!m_bColorUpdating)
                    {
                        Color = HsvColor.ConvertHsvToRgb(m_h, m_s, m_v);
                    }
                }
            }
        }

        /// <summary>
        /// Called when parameter V is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnVChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_v != (float)e.NewValue)
            {
                m_v = (float)e.NewValue;

                if (HSV == HSV.V)
                {
                    SliderValueHSV = m_v;
                }

                if (VChanged != null)
                {
                    VChanged(this, e);
                }

                if (!m_bColorUpdating)
                {
                    Color = HsvColor.ConvertHsvToRgb(m_h, m_s, m_v);
                }
            }
        }

        /// <summary>
        /// Called when slider value HSV is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnSliderValueHSVChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_sliderValueHSV != (float)e.NewValue)
            {
                m_sliderValueHSV = (float)e.NewValue;

                if (SliderValueHSVChanged != null)
                {
                    SliderValueHSVChanged(this, e);
                }
                
                switch (HSV)
                {
                    case HSV.H:
                        if (m_sliderValueHSV >= 1.0)
                        {
                            H = m_sliderValueHSV;
                        }
                        break;
                    case HSV.S:
                        if (m_sliderValueHSV <= 1.0)
                        {
                            S = m_sliderValueHSV;
                        }
                        break;
                    case HSV.V:
                        V = m_sliderValueHSV;
                        break;
                    default:
                        H = m_sliderValueHSV;
                        break;
                }
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
        #endregion

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
        /// When implemented in a derived class, will be invoked whenever
        /// application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_colorToggleButton = FindName(C_colorToggleButton) as ToggleButton;
            m_systemColors = FindName(C_systemColors) as ComboBox;
            if (m_systemColors != null)
            {
                m_systemColors.Items.CurrentChanged += new EventHandler(Items_CurrentChanged);
            }

            if (VisualizationStyle == xColorSelectionMode.HSV)
            {
                m_colorPalette = GetTemplateChild(C_colorPalette) as FrameworkElement;
                m_buttomH = GetTemplateChild(C_buttomH) as RadioButton;
                m_buttomS = GetTemplateChild(C_buttomS) as RadioButton;
                m_buttomV = GetTemplateChild(C_buttomV) as RadioButton;
                m_buttomH.Checked += new RoutedEventHandler(HSVSelected);
                m_buttomS.Checked += new RoutedEventHandler(HSVSelected);
                m_buttomV.Checked += new RoutedEventHandler(HSVSelected);
                m_colorPalette.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                m_colorPalette.PreviewMouseMove += new MouseEventHandler(OnMouseMove);
                m_wordKnownColorsTextBox = GetTemplateChild(C_wordKnownColorsTextBox) as TextBox;
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
            m_colorStringEditor.LostFocus += new RoutedEventHandler(ColorStringEditor_LostFocus);
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
            if (m_colorToggleButton.IsChecked == true)
            {
                xColorItem colorItem = (xColorItem)m_systemColors.SelectedValue;
                Color = colorItem.Brush.Color;
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
            if (VisualizationStyle == xColorSelectionMode.HSV && m_colorPalette != null)
            {
                CalculateHSVSelectorPosition();
            }
        }

        /// <summary>
        /// Raises VisualizationStyleChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnVisualizationStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            m_visualizationStyle = (xColorSelectionMode)e.NewValue;
            if (VisualizationStyleChanged != null)
            {
                VisualizationStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises ColorChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            m_color = (Color)e.NewValue;
            m_bColorUpdating = true;

            m_r = m_color.ScR;
            m_g = m_color.ScG;
            m_b = m_color.ScB;
            m_a = m_color.ScA;
            R = m_color.ScR;
            G = m_color.ScG;
            B = m_color.ScB;
            A = m_color.ScA;

            if (VisualizationStyle == xColorSelectionMode.RGB)
            {
                CalculateBackground();
            }

            if (VisualizationStyle == xColorSelectionMode.HSV && m_colorPalette != null && m_bNeedChangeHSV)
            {
                CalculateHSVSelectorPosition();
            }

            InvertColor = Color.FromRgb((byte)(255 - m_color.R), (byte)(255 - m_color.G), (byte)(255 - m_color.B));

            if (ColorChanged != null)
            {
                ColorChanged(this, e);
            }

            m_bColorUpdating = false;
            UpdateColorBarSlider();
        }

        /// <summary>
        /// Calculates position of the selector in HSV.
        /// </summary>
        private void CalculateHSVSelectorPosition()
        {
            HsvColor hsv = HsvColor.ConvertRgbToHsv(m_color.R, m_color.B, m_color.G);
            H = (float)hsv.H;
            S = (float)hsv.S;
            V = (float)hsv.V;
            SliderValueHSV = H;

            Point selectorPosition = new Point(0, 0);
            switch (HSV)
            {
                case HSV.H:
                    selectorPosition.X = GetXPositionForH();
                    selectorPosition.Y = GetYPositionForHS();
                    break;
                case HSV.S:
                    selectorPosition.X = GetXPositionForSV();
                    selectorPosition.Y = GetYPositionForHS();
                    break;
                case HSV.V:
                    selectorPosition.X = GetXPositionForSV();
                    selectorPosition.Y = GetYPositionForV();
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
            return (H / 3.6f) * ((float)m_colorPalette.ActualWidth / 100);
        }

        /// <summary>
        /// Gets Y selector position in the color palette for V.
        /// </summary>
        /// <returns>Y selector position for V</returns>
        private float GetYPositionForV()
        {
            return (float)m_colorPalette.ActualHeight - (S * (float)m_colorPalette.ActualHeight);
        }

        /// <summary>
        /// Gets Y selector position in the color palette for H and S.
        /// </summary>
        /// <returns>Y selector position for H and S</returns>
        private float GetYPositionForHS()
        {
            return (float)m_colorPalette.ActualHeight - (V * (float)m_colorPalette.ActualHeight);
        }

        /// <summary>
        /// Gets X selector position in the color palette for H.
        /// </summary>
        /// <returns>X selector position for H</returns>
        private float GetXPositionForH()
        {
            return S * (float)m_colorPalette.ActualWidth;
        }

        /// <summary>
        /// Called when mouse left button down.
        /// </summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" />
        /// instance containing the event data.</param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();

            if (m_colorPalette == null)
            {
                return;
            }

            Point p = e.GetPosition(m_colorPalette);

            m_bNeedChangeHSV = false;
            p = FindColorHSV(p);
            m_bNeedChangeHSV = true;

            Color = HsvColor.ConvertHsvToRgb(m_h, m_s, m_v);

            CalculateWordKnownColorsPosition(p);
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
                Point p = e.GetPosition(m_colorPalette);

                if (p.X > m_colorPalette.ActualWidth)
                {
                    p.X = m_colorPalette.ActualWidth;
                }

                if (p.X < 0)
                {
                    p.X = 0;
                }

                if (p.Y > m_colorPalette.ActualHeight)
                {
                    p.Y = m_colorPalette.ActualHeight;
                }

                if (p.Y < 0)
                {
                    p.Y = 0;
                }

                m_bNeedChangeHSV = false;
                p = FindColorHSV(p);
                m_bNeedChangeHSV = true;

                Color = HsvColor.ConvertHsvToRgb(m_h, m_s, m_v);
                CalculateWordKnownColorsPosition(p);
            }
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
            if (m_colorPalette != null)
            {
                m_colorPalette.ReleaseMouseCapture();
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

        #endregion

        #region Help Methods
        /// <summary>
        /// Calculates the word known colors position and Selector
        /// Position.
        /// </summary>
        /// <param name="point">The point.</param>
        private void CalculateWordKnownColorsPosition(Point point)
        {
            double wordKnownColorsTextBoxWidth = m_wordKnownColorsTextBox.ActualWidth;
            double wordKnownColorsTextBoxHeight = m_wordKnownColorsTextBox.ActualHeight;

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

            SelectorPositionX = (float)point.X - 5;
            SelectorPositionY = (float)point.Y - 5;

            ToolTip toolTip = GetColorsTooltip();
            m_wordKnownColorsTextBox.ToolTip = toolTip;
        }

        /// <summary>
        /// Creates tooltip for the selected color when mouse over.
        /// </summary>
        /// <returns>Tooltip created.</returns>
        private ToolTip GetColorsTooltip()
        {
            string[] colorString = xColorEdit.SuchColor(Color);

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
            H = m_h;
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
            S = m_s;
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
            V = m_v;
        }

        /// <summary>
        /// Calculates the background for RGB model.
        /// </summary>
        private void CalculateBackground()
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
        #endregion
    }
}
