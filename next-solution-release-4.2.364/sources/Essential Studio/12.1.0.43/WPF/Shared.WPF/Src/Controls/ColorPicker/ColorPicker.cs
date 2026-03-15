// <copyright file="ColorPicker.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Tools;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class for color generating. ColorPicker control.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Help Page</term>
    ///         <description>Syntax</description>
    ///     </listheader>
    ///     <example>
    ///       <list type="table">
    ///            <listheader>
    ///                  <description>C#</description>
    ///        </listheader>
    ///         <example><code>public class ColorPicker : Control</code></example>
    ///         </list>
    ///         <para/>
    ///         <list type="table">
    ///             <listheader>
    ///                <description>XAML Object Element Usage</description>
    ///             </listheader>
    ///             <example><code><s:ColorPicker Name="myColorPicker" xmlns:s="http://schemas.syncfusion.com/wpf"/></code></example>
    ///         </list>
    ///      </example>
    /// </list>
    /// <example>
    ///       <para/>This example shows how to create a ColorPicker in XAML.
    /// <code>
    ///        <Window x:Class="ColorPicker.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:s="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF" Title="ColorPicker" Height="300" Width="300">
    ///             <StackPanel HorizontalAlignment="Center">
    ///                <s:ColorPicker Name="myColorPicker" Width="200" BorderThickness="1" HorizontalAlignment="Left"></s:ColorPicker>
    ///             </StackPanel>
    ///        </Window>
    ///       </code>
    ///      <para/>This example shows how to create a ColorPicker in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace Sample1
    /// {
    /// public partial class Window1 : Window
    /// {
    /// public Window1()
    /// {
    /// InitializeComponent();
    /// ColorPicker myColorPicker = new ColorPicker();
    /// stackPanel.Children.Add( myColorPicker );
    /// }
    /// }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/SyncOrange.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/ShinyRed.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/ShinyBlue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(ColorPicker), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/TransparentStyle.xaml")]
    public class ColorPicker : Control
    {
        #region Constants

        /// <summary>
        /// Contains color toggle button name.
        /// </summary>
        private const string C_colorToggleButton = "colorToggleButton";

        /// <summary>
        /// Contains color edit popup name.
        /// </summary>
        private const string C_colorEditPopup = "colorEditPopup";

        /// <summary>
        /// Contains color edit control name.
        /// </summary>
        private const string C_colorEdit = "ColorEdit";

        /// <summary>
        /// Contains property color name. Is used for binding.
        /// </summary>
        private const string C_color = "Color";

        /// <summary>
        /// Contains default skin name.
        /// </summary>
        private const string C_defaultSkinName = "Default";

        /// <summary>
        /// Contains property color edit container brush.
        /// </summary>
        private const string C_colorEditContainerBrush = "ColorEditContainerBrush";

        internal LinearGradientBrush m_gradient;

        /// <summary>
        /// Contains system colors.
        /// </summary>
        private const string C_systemColors = "systemColors";

        #endregion Constants

        #region Private Members

        /// <summary>
        /// Color editor for this control.
        /// </summary>
        internal ColorEdit m_colorEditor;

        /// <summary>
        /// Contains comboBox with system colors
        /// </summary>
        private ComboBox m_systemColors;

        /// <summary>
        /// Popup for color editor.
        /// </summary>
        private Popup m_colorEditorPopup;

        /// <summary>
        /// Toggle Button for open popup.
        /// </summary>
        private ToggleButton m_colorToggleButton;

        /// <summary>
        /// Command for open popup.
        /// </summary>
        public static RoutedCommand M_displayPopup;

        private Brush m_tempBrush;
        internal bool flag = false;
        private Border m_selectedColorRect;
        private TextBlock m_selectedColorText;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Gets or sets value of the Color dependency property.
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
        /// Gets or sets a value indicating whether the ColorPalette is visible or not.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public bool IsColorPaletteVisible
        {
            get
            {
                return (bool)GetValue(IsColorPaletteVisibleProperty);
            }

            set
            {
                SetValue(IsColorPaletteVisibleProperty, value);
            }
        }

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
        /// Gets or sets value of the ColorEdit Background dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public Brush ColorEditBackground
        {
            get
            {
                return (Brush)GetValue(ColorEditBackgroundProperty);
            }

            set
            {
                SetValue(ColorEditBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value of the ColorPicker IsAlphaVisible dependency property.
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
        /// Gets or sets a value indicating whether [enable solid to gradient switch].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable solid to gradient switch]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSolidToGradientSwitch
        {
            get
            {
                return (bool)GetValue(EnableSolidToGradientSwitchProperty);
            }
            set
            {
                SetValue(EnableSolidToGradientSwitchProperty, value);
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
        /// Gets or sets the gradient property editor mode.
        /// </summary>
        /// <value>The gradient property editor mode.</value>
        public GradientBrushDisplayMode GradientBrushDisplayMode
        {
            get
            {
                return (GradientBrushDisplayMode)GetValue(GradientBrushDisplayModeProperty);
            }
            set
            {
                SetValue(GradientBrushDisplayModeProperty, value);
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

        #endregion Properties

        #region Dependency Properties

        /// <summary>
        /// Identifies ColorPicker.Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        internal static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnColorChanged)));

        //internal static readonly DependencyProperty ColorEditProperty =
        //    DependencyProperty.Register("ColorEdit", typeof(ColorEdit), typeof(ColorPicker), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies ColorPicker.IsColorPaletteVisible dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        internal static readonly DependencyProperty IsColorPaletteVisibleProperty =
           DependencyProperty.Register("IsColorPaletteVisible", typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Dependency property which has enable solid to gradient switch
        /// </summary>
        public static readonly DependencyProperty EnableSolidToGradientSwitchProperty =
           DependencyProperty.RegisterAttached("EnableSolidToGradientSwitch", typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Dependency property which has brush
        /// </summary>
        public static readonly DependencyProperty BrushProperty =
           DependencyProperty.Register("Brush", typeof(Brush), typeof(ColorPicker), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnSelectedBrushChanged)));

        /// <summary>
        /// Dependency property which has visualization
        /// </summary>
        public static readonly DependencyProperty VisualizationStyleProperty =
            DependencyProperty.Register("VisualizationStyle", typeof(ColorSelectionMode), typeof(ColorPicker), new FrameworkPropertyMetadata(ColorSelectionMode.RGB));

        /// <summary>
        /// Dependency property which has brush mode
        /// </summary>
        public static readonly DependencyProperty BrushModeProperty =
       DependencyProperty.Register("BrushMode", typeof(BrushModes), typeof(ColorPicker), new FrameworkPropertyMetadata(BrushModes.Solid, new PropertyChangedCallback(OnSelectedBrushModeChanged)));

        /// <summary>
        /// Dependency property which has gradient property editormodel
        /// </summary>
        public static readonly DependencyProperty GradientPropertyEditorModeProperty =
            DependencyProperty.Register("GradientPropertyEditorMode", typeof(GradientPropertyEditorMode), typeof(ColorPicker), new FrameworkPropertyMetadata(GradientPropertyEditorMode.Popup, new PropertyChangedCallback(OnGradientPropertyEditorModeChanged)));

        /// <summary>
        /// Dependency property which has is open gradient property editor
        /// </summary>
        public static readonly DependencyProperty IsOpenGradientPropertyEditorProperty =
           DependencyProperty.Register("IsOpenGradientPropertyEditor", typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsOpenGradientPropertyEditorChanged)));

        /// <summary>
        /// Identifies the GradientBrushDisplayModeProperty
        /// </summary>
        public static readonly DependencyProperty GradientBrushDisplayModeProperty =
            DependencyProperty.Register("GradientBrushDisplayMode", typeof(GradientBrushDisplayMode), typeof(ColorPicker), new FrameworkPropertyMetadata(GradientBrushDisplayMode.Default, new PropertyChangedCallback(OnGradientBrushDisplayModeChanged)));

        /// <summary>
        /// Dependency property which has is gradient property enabled
        /// </summary>
        public static readonly DependencyProperty IsGradientPropertyEnabledProperty =
           DependencyProperty.Register("IsGradientPropertyEnabled", typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsGradientPropertyEnabledChanged)));

        //public static void SetEnableSolidToGradientSwitch(UIElement element, Boolean value)
        //{
        //    element.SetValue(EnableSolidToGradientSwitchProperty, value);
        //}
        //public static bool GetEnableSolidToGradientSwitch(UIElement element)
        //{
        //    return (Boolean)element.GetValue(EnableSolidToGradientSwitchProperty);
        //}
        /// <summary>
        /// Identifies ColorPicker.ColorEditBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty ColorEditBackgroundProperty =
            DependencyProperty.Register("ColorEditBackground", typeof(Brush), typeof(ColorPicker), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorEditBackgroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public bool EnableToolTip
        {
            get { return (bool)GetValue(EnableToolTipProperty); }
            set { SetValue(EnableToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnabletoolTip.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty EnableToolTipProperty =
            DependencyProperty.Register("EnableToolTip", typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnEnableToolTipChanged)));

        /// <summary>
        /// Identifies ColorPicker. IsAlphaVisible dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty IsAlphaVisibleProperty = DependencyProperty.Register("IsAlphaVisible", typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsAlphaVisiblePropertyChanged)));

        #endregion Dependency Properties

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ColorPicker"/> class.
        /// </summary>
        static ColorPicker()
        {
            // EnvironmentTest.ValidateLicense(typeof(ColorPicker));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
            M_displayPopup = new RoutedCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPicker"/> class..
        /// Creates ColorPicker object.
        /// </summary>
        public ColorPicker()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ColorPicker));
            }
            CommandBinding displayPopup = new CommandBinding(M_displayPopup);
            displayPopup.Executed += new ExecutedRoutedEventHandler(DisplayPopup);
            CommandBindings.Add(displayPopup);
            m_colorEditor = new ColorEdit();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

            Keyboard.AddKeyDownHandler(this, OnKeyDown);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
            Mouse.AddPreviewMouseDownHandler(this, OnMouseDown);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event that is raised when Color property is changed.
        /// </summary>
        public event PropertyChangedCallback ColorChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback EnableToolTipChanged;

        /// <summary>
        /// Occurs when [selected brush changed].
        /// </summary>
        public event PropertyChangedCallback SelectedBrushChanged;

        /// <summary>
        /// Occurs when [selected brush mode changed].
        /// </summary>
        public event PropertyChangedCallback SelectedBrushModeChanged;

        /// <summary>
        /// Occurs when [gradient property editor mode changed].
        /// </summary>
        public event PropertyChangedCallback GradientPropertyEditorModeChanged;

        /// <summary>
        /// Occurs when [gradient Brush Display Mode changed].
        /// </summary>
        public event PropertyChangedCallback GradientBrushDisplayModeChanged;

        /// <summary>
        /// Occurs when [is open gradient property editor changed].
        /// </summary>
        public event PropertyChangedCallback IsOpenGradientPropertyEditorChanged;

        /// <summary>
        /// Occurs when [is gradient property enabled changed].
        /// </summary>
        public event PropertyChangedCallback IsGradientPropertyEnabledChanged;

        #endregion Events

        #region Static Methods

        /// <summary>
        /// Calls OnColorChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;
            instance.OnColorChanged(e);
        }

        /// <summary>
        /// Called when [selected brush  Mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedBrushModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;
            instance.OnSelectedBrushModeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedBrushModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectedBrushModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_colorEditor != null)
            {
                if (BrushMode == BrushModes.Solid)
                {
                    m_colorEditor.BrushMode = BrushModes.Solid;
                    if (m_colorEditor.gradientGrid != null)
                        m_colorEditor.gradientGrid.Visibility = Visibility.Collapsed;
                }
                else if (BrushMode == BrushModes.Gradient)
                {
                    m_colorEditor.BrushMode = BrushModes.Gradient;
                    if (m_colorEditor.gradientGrid != null)
                        m_colorEditor.gradientGrid.Visibility = Visibility.Visible;
                }
            }
            if (SelectedBrushModeChanged != null)
            {
                SelectedBrushModeChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [selected brush changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;

            if (e.NewValue is SolidColorBrush)
            {
                instance.BrushMode = BrushModes.Solid;
            }
            else if (e.NewValue is LinearGradientBrush || e.NewValue is RadialGradientBrush)
            {
                instance.BrushMode = BrushModes.Gradient;
            }
            instance.OnSelectedBrushChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedBrushChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary rs = new ResourceDictionary { Source = new Uri(@"Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/generic.xaml", UriKind.RelativeOrAbsolute) };
            if (e.NewValue == null)
            {
                if (m_selectedColorRect != null)
                    m_selectedColorRect.Background = rs["NullBrush"] as DrawingBrush;
                if (m_selectedColorText != null)
                    m_selectedColorText.Text = "";
            }
            if (e.NewValue is SolidColorBrush)
            {
                if (m_colorEditor != null)
                {
                    m_colorEditor.BrushMode = BrushModes.Solid;
                }

                if (m_colorEditor.SelectedColor == null)
                {
                    Color = ((SolidColorBrush)e.NewValue).Color;
                }
                if (m_colorEditor.SelectedColor != null && m_colorEditor.CurrentColor != null)
                {
                    if (m_colorEditor.count < 1)
                    {
                        m_colorEditor.CurrentColor.Fill = m_colorEditor.previousSelectedBrush;
                        m_colorEditor.SelectedColor.Fill = this.Brush;
                        m_colorEditor.count++;
                    }
                }

                if (m_selectedColorRect != null && m_selectedColorText != null)
                {
                    //m_selectedColorRect.Background = new SolidColorBrush(Color);
                    m_selectedColorRect.Background = this.Brush;
                    m_selectedColorText.Text = ColorEdit.SuchColor(((SolidColorBrush)Brush).Color)[0];
                    if (m_selectedColorText.Text == "Transparent")
                    {
                        m_selectedColorRect.Background = rs["TransparentBrush"] as DrawingBrush;
                    }
                    if (m_colorEditor.CurrentColor != null)
                    {
                        //m_colorEditor.CurrentColor.Fill = new SolidColorBrush(Color);
                    }
                }
            }
            else if (e.NewValue is GradientBrush)
            {
                if (m_colorEditor != null)
                {
                    m_colorEditor.BrushMode = BrushModes.Gradient;
                    m_tempBrush = e.NewValue as Brush;
                    m_gradient = e.NewValue as LinearGradientBrush;
                    if (!m_colorEditor.flag)
                    {
                        if (m_colorEditor.gradientItemCollection != null)
                        {
                            m_colorEditor.gradientItemCollection.Items.Clear();
                        }
                        m_colorEditor.SetBrush(e.NewValue as GradientBrush);
                        m_colorEditor.bindedmanually = true;
                        m_colorEditor.Brush = this.Brush;
                        this.flag = true;
                        if (m_colorEditor.canvasBar != null)
                        {
                            for (int i = 0; i < m_colorEditor.canvasBar.Children.Count; i++)
                            {
                                UIElement item = m_colorEditor.canvasBar.Children[i];
                                if (item is Canvas)
                                {
                                    m_colorEditor.canvasBar.Children.Remove(item);
                                    i--;
                                }
                            }

                            foreach (GradientStopItem i in m_colorEditor.gradientItemCollection.Items)
                            {
                                try
                                {
                                    m_colorEditor.canvasBar.Children.Add(i.gradientitem);
                                }

                                catch { }
                            }
                        }

                        if (m_gradient != null)
                        {
                            m_colorEditor.Startpoint = m_gradient.StartPoint;
                            m_colorEditor.Endpoint = m_gradient.EndPoint;
                        }

                        m_colorEditor.fillGradient(m_colorEditor.gradientItemCollection.gradientItem);

                        if (((GradientBrush)e.NewValue).GradientStops.Count > 0)
                            this.m_colorEditor.Color = (((GradientBrush)e.NewValue).GradientStops[((GradientBrush)e.NewValue).GradientStops.Count - 1]).Color;
                        if (m_selectedColorRect != null && m_selectedColorText != null)
                        {
                            //m_selectedColorRect.Background = new SolidColorBrush(m_colorEditor.gradientItemCollection.gradientItem.color);
                            m_selectedColorRect.Background = this.Brush;
                            if (GradientBrushDisplayMode.Default == this.GradientBrushDisplayMode)
                                m_selectedColorText.Text = this.Brush.GetType().ToString();
                            else
                                m_selectedColorText.Text = ColorEdit.SuchColor(Color)[0];
                            //m_selectedColorText.Text = ColorEdit.SuchColor(m_colorEditor.gradientItemCollection.gradientItem.color)[0];
                            if (m_colorEditor.CurrentColor != null)
                            {
                                m_colorEditor.CurrentColor.Fill = new SolidColorBrush(m_colorEditor.gradientItemCollection.gradientItem.color);
                            }
                        }
                        if (!flag)
                        {
                            m_colorEditor.setnocolor = true;
                            m_colorEditor.Color = (((GradientBrush)m_tempBrush).GradientStops[((GradientBrush)m_tempBrush).GradientStops.Count - 1]).Color;
                        }
                    }
                    else
                    {
                        m_colorEditor.flag = false;

                        if (m_selectedColorRect != null && m_selectedColorText != null && m_colorEditor.gradientItemCollection != null)
                        {
                            //m_selectedColorRect.Background = new SolidColorBrush(m_colorEditor.gradientItemCollection.gradientItem.color);
                            m_selectedColorRect.Background = this.Brush;
                            if (GradientBrushDisplayMode.Default == this.GradientBrushDisplayMode)
                                m_selectedColorText.Text = this.Brush.GetType().ToString();
                            else
                                m_selectedColorText.Text = ColorEdit.SuchColor(Color)[0];
                            //m_selectedColorText.Text = ColorEdit.SuchColor(m_colorEditor.gradientItemCollection.gradientItem.color)[0];
                        }
                        if (m_colorEditor.CurrentColor != null && m_colorEditor.SelectedColor != null)
                        {
                            //m_colorEditor.CurrentColor.Fill = new SolidColorBrush(m_colorEditor.gradientItemCollection.gradientItem.color);
                            m_colorEditor.SelectedColor.Fill = this.Brush;
                            m_colorEditor.CurrentColor.Fill = m_colorEditor.previousSelectedBrush;
                        }
                    }
                }
            }
            if (this.m_colorEditor != null)
            {
                if (this.m_colorEditor.SelectedColor != null)
                {
                    this.m_colorEditor.SelectedColor.Fill = this.Brush;
                }
            }
            if (SelectedBrushChanged != null)
            {
                SelectedBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [gradient property editor mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGradientPropertyEditorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;
            instance.OnGradientPropertyEditorModeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:GradientPropertyEditorModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnGradientPropertyEditorModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GradientPropertyEditorModeChanged != null)
            {
                GradientPropertyEditorModeChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [is open gradient property editor changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsOpenGradientPropertyEditorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;

            instance.OnIsOpenGradientPropertyEditorChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsOpenGradientPropertyEditorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnIsOpenGradientPropertyEditorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsOpenGradientPropertyEditorChanged != null)
            {
                IsOpenGradientPropertyEditorChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [is gradient property enabled changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsGradientPropertyEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;

            instance.OnIsGradientPropertyEnabledChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsGradientPropertyEnabledChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsGradientPropertyEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsGradientPropertyEnabledChanged != null)
            {
                IsGradientPropertyEnabledChanged(this, e);
            }
            if ((bool)e.NewValue == true)
            {
                LinearGradientBrush gb = new LinearGradientBrush();
                this.Brush = gb;
            }
            else
            {
                SolidColorBrush sb = new SolidColorBrush();
                this.Brush = sb;
            }
        }

        /// <summary>
        /// Called when [GradientBrushDisplayMode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGradientBrushDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;
            instance.OnGradientBrushDisplayModeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:GradientBrushDisplayModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnGradientBrushDisplayModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_selectedColorText != null && m_colorEditor.BrushMode == BrushModes.Gradient)
            {
                if (GradientBrushDisplayMode.Default == this.GradientBrushDisplayMode)
                    m_selectedColorText.Text = this.Brush.GetType().ToString();
                else
                    m_selectedColorText.Text = ColorEdit.SuchColor(Color)[0];
            }
            if (GradientBrushDisplayModeChanged != null)
            {
                GradientBrushDisplayModeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnColorEditBackgroundChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnColorEditBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;
            instance.OnColorEditBackgroundChanged(e);
        }

        private static void OnEnableToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;
            instance.OnEnableToolTipChanged(e);
        }

        /// <summary>
        /// Called when [enable switch changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableSwitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker)
            {
                ColorPicker instance = (ColorPicker)d;
                instance.OnEnableSwitchChanged(e);
            }
            else
            {
                ColorEdit instance = (ColorEdit)d;
                instance.Loaded += new RoutedEventHandler(instance_Loaded);
            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the instance control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void instance_LayoutUpdated(object sender, EventArgs e)
        {
            // (sender as ColorEdit).enableSwitch.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Loaded event of the instance control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private static void instance_Loaded(object sender, RoutedEventArgs e)
        {
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
            if (this.m_colorEditor.enableSwitch != null)
            {
                if ((bool)e.NewValue == true)
                {
                    this.m_colorEditor.enableSwitch.Visibility = Visibility.Visible;
                }
                else
                {
                    this.m_colorEditor.enableSwitch.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Calls IsAlphaVisiblePropertyChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void IsAlphaVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker instance = (ColorPicker)d;
            instance.m_colorEditor.IsAlphaVisible = instance.IsAlphaVisible;
        }

        #endregion Static Methods

        #region Internal

        /// <summary>
        /// When implemented in a derived class, will be invoked whenever
        /// application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //ColorPicker colorpicker = new ColorPicker();
            m_colorToggleButton = GetTemplateChild(C_colorToggleButton) as ToggleButton;
            m_colorEditorPopup = GetTemplateChild(C_colorEditPopup) as Popup;
            m_colorEditor = GetTemplateChild(C_colorEdit) as ColorEdit;
            m_colorEditor.IsAlphaVisible = this.IsAlphaVisible;
            if (m_gradient != null)
            {
                m_colorEditor.Startpoint = m_gradient.StartPoint;
                m_colorEditor.Endpoint = m_gradient.EndPoint;
            }
            m_colorEditor.m_colorPicker = this;
            m_systemColors = Template.FindName(C_systemColors, this) as ComboBox;
            m_selectedColorRect = GetTemplateChild("selectedColorRect") as Border;
            m_selectedColorText = GetTemplateChild("SelectedColorText") as TextBlock;
            if (m_selectedColorText != null && this.BrushMode == BrushModes.Gradient)
            {
                if (GradientBrushDisplayMode.Default == this.GradientBrushDisplayMode)
                    m_selectedColorText.Text = this.Brush.GetType().ToString();
                else
                    m_selectedColorText.Text = ColorEdit.SuchColor(Color)[0];
            }
            //this.ColorEdit = m_colorEditor;
            if (m_colorEditor != null)
            {
                Binding binding = new Binding(C_color);
                binding.Source = this;
                binding.Mode = BindingMode.TwoWay;
                m_colorEditor.SetBinding(ColorEdit.ColorProperty, binding);
                //m_colorEditor.Background = this.Background;
                if (m_colorEditorPopup != null)
                {
                    m_colorEditorPopup.Placement = PlacementMode.Bottom;
                    m_colorEditorPopup.Opened += new EventHandler(ColorEditorPopup_Opened);
                }
                SelectPaletteColor();
                if (this.IsGradientPropertyEnabled == false)
                {
                    SolidColorBrush sb = new SolidColorBrush();
                    //this.Brush = sb;
                }
                else
                {
                    LinearGradientBrush gb = new LinearGradientBrush();
                    //this.Brush = gb;
                }
                ResourceDictionary rs1 = new ResourceDictionary { Source = new Uri(@"Syncfusion.Shared.WPF;component/Controls/ColorPicker/Themes/generic.xaml", UriKind.RelativeOrAbsolute) };

                if (m_selectedColorText.Text == "Transparent")
                {
                    if (m_selectedColorRect != null)
                        m_selectedColorRect.Background = rs1["TransparentBrush"] as DrawingBrush;
                }
                if (Brush == null)
                {
                    if (m_selectedColorRect != null)
                        m_selectedColorRect.Background = rs1["NullBrush"] as DrawingBrush;
                    if (m_selectedColorText != null)
                        m_selectedColorText.Text = "";
                }
                if (flag == true)
                {
                    if (this.Brush is System.Windows.Media.SolidColorBrush)
                    {
                        if (m_colorEditor.gradientItemCollection != null)
                        {
                            m_colorEditor.gradientItemCollection.Items.Clear();
                        }

                        m_colorEditor.SetBrush(m_tempBrush as LinearGradientBrush);
                        m_colorEditor.bindedmanually = true;
                        m_colorEditor.fillGradient(m_colorEditor.gradientItemCollection.gradientItem);
                        flag = false;
                        m_colorEditor.flag = false;
                    }
                    else
                    {
                        if (m_colorEditor.gradientItemCollection != null)
                        {
                            m_colorEditor.gradientItemCollection.Items.Clear();
                        }

                        m_colorEditor.SetBrush(m_tempBrush as GradientBrush);
                        m_colorEditor.bindedmanually = true;
                        m_colorEditor.fillGradient(m_colorEditor.gradientItemCollection.gradientItem);
                        if (m_colorEditor.gradientItemCollection.gradientItem != null)
                        {
                            if (m_selectedColorRect != null)
                            {
                                //m_selectedColorRect.Background = new SolidColorBrush(m_colorEditor.gradientItemCollection.gradientItem.color);
                                m_selectedColorRect.Background = this.Brush;
                            }
                            if (m_colorEditor.CurrentColor != null)
                            {
                                m_colorEditor.CurrentColor.Fill = new SolidColorBrush(m_colorEditor.gradientItemCollection.gradientItem.color);
                            }
                            Color = m_colorEditor.gradientItemCollection.gradientItem.color;
                            if (m_selectedColorText != null)
                            {
                                m_selectedColorText.Text = this.Brush.GetType().ToString();

                                //m_selectedColorText.Text =
                                //ColorEdit.SuchColor(m_colorEditor.gradientItemCollection.gradientItem.color)[0];
                            }
                        }
                        flag = false;
                        m_colorEditor.flag = false;
                    }
                }
                else
                {
                    //Color = m_colorEditor.Color;
                    //Brush = new SolidColorBrush(Color);
                }
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            //if (this.IsLoaded && e.Property == SkinStorage.VisualStyleProperty)
            //{
            //    string style = SkinStorage.GetVisualStyle(this);
            //    Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            //    if (list1 != null)
            //    {
            //        Shared.DictionaryList list2 = list1[style] as Shared.DictionaryList;

            //        if (SkinStorage.GetVisualStyle(this) != C_defaultSkinName)
            //        {
            //            if (list2 != null && list2.ContainsKey(C_colorEditContainerBrush))
            //            {
            //                ColorEditBackground = list2[C_colorEditContainerBrush] as Brush;
            //            }
            //        }
            //        else
            //        {
            //            if (m_defaultBackground == null && list2 != null && list2.ContainsKey(C_colorEditContainerBrush))
            //            {
            //                ColorEditBackground = list2[C_colorEditContainerBrush] as Brush;
            //            }

            //            ColorEditBackground = m_defaultBackground;
            //        }
            //    }
            //}
            //else if (e.Property == ColorPicker.ColorEditBackgroundProperty)
            //{
            //    if (SkinStorage.GetVisualStyle(this) == C_defaultSkinName)
            //    {
            //        m_defaultBackground = ColorEditBackground;
            //    }
            //}
        }

        /// <summary>
        /// Sets color selected by user.
        /// </summary>
        internal void SelectPaletteColor()
        {
            if (Template != null && IsColorPaletteVisible == true)
            {
                ComboBox obj = Template.FindName(C_systemColors, this) as ComboBox;
                obj.DropDownOpened += new EventHandler(obj_DropDownOpened);
                if (obj != null)
                {
                    //if (m_colorEditor != null)
                    //    m_colorEditor.changeHSVBackground = true;
                    IList list = obj.ItemsSource as IList;
                    int index = -1;
                    if (list != null)
                    {
                        for (int i = 0, cnt = list.Count; i < cnt; i++)
                        {
                            ColorItem item = list[i] as ColorItem;

                            if (item.Name == ColorEdit.SuchColor(Color)[0])
                            {
                                index = i;
                                break;
                            }
                        }

                        if (index != -1)
                        {
                            obj.SelectedIndex = index;
                        }
                    }
                }
            }
        }

        private void obj_DropDownOpened(object sender, EventArgs e)
        {
            if (this.m_colorEditor != null)
            {
                this.m_colorEditor.changeColor = true;
            }
        }

        /// <summary>
        /// Raises ColorChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            //Color = (Color)e.NewValue;

            if (ColorChanged != null)
            {
                if (this.m_colorEditor != null)
                {
                    //if (this.m_colorEditor.A == ((Color)e.NewValue).ScA)
                    ColorChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Raises when ColorEditBackground property changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnColorEditBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_colorEditor != null)
            {
                m_colorEditor.Background = (Brush)e.NewValue;
            }
        }

        private void OnEnableToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableToolTipChanged != null)
            {
                EnableToolTipChanged(this, e);
            }
        }

        /// <summary>
        /// Changes the color.
        /// </summary>
        /// <param name="sender">Dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        public void ColorChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Color = (sender as ColorEdit).Color;
        }

        /// <summary>
        /// Displays the popup.
        /// </summary>
        /// <param name="sender">Sender Object.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" />
        /// instance containing the event data.</param>
        private void DisplayPopup(object sender, RoutedEventArgs e)
        {
            if (m_colorToggleButton.IsChecked == true)
            {
                m_colorEditorPopup.Width = ActualWidth;
                if (m_colorEditor.m_wordKnownColorPopup != null)
                    m_colorEditor.m_wordKnownColorPopup.IsOpen = false;
                if (m_systemColors != null)
                {
                    ComboBox obj = m_systemColors as ComboBox;
                    if (obj != null)
                    {
                        IList<ColorItem> list = obj.ItemsSource as IList<ColorItem>;
                        int index = -1;
                        if (list != null)
                        {
                            for (int i = 0, cnt = list.Count; i < cnt; i++)
                            {
                                ColorItem item = list[i] as ColorItem;

                                if (item.Name == ColorEdit.SuchColor(this.Color)[0])
                                {
                                    index = i;
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
                Keyboard.Focus(m_colorEditor);
            }
        }

        /// <summary>
        /// Executes when some key on keyboard is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" />
        /// instance containing the event data.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    m_colorToggleButton.IsChecked = false;
                    if (this.IsMouseCaptured)
                    {
                        Mouse.Capture(null);
                    }

                    break;
            }
        }

        /// <summary>
        /// Executes when mouse button is clicked outside the captured element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" />
        /// instance containing the event data.</param>
        private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            m_colorToggleButton.IsChecked = !m_colorToggleButton.IsChecked;
            if (this.IsMouseCaptured)
            {
                Mouse.Capture(null);
            }
        }

        /// <summary>
        /// Executes when the ColorEditor popup is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" />
        /// instance containing the event data.</param>
        private void ColorEditorPopup_Opened(object sender, EventArgs e)
        {
            //if ((bool)this.GetValue(EnableSolidToGradientSwitchProperty) == true)
            //{
            //    m_colorEditor.enableSwitch.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    m_colorEditor.enableSwitch.Visibility = Visibility.Collapsed;
            //}
            m_colorEditor.EnableGradientToSolidSwitch = this.EnableSolidToGradientSwitch;
            m_colorEditor.BrushMode = this.BrushMode;
            m_colorEditor.Background = this.ColorEditBackground;
            m_colorEditor.GradientPropertyEditorMode = this.GradientPropertyEditorMode;
            m_colorEditor.IsOpenGradientPropertyEditor = this.IsOpenGradientPropertyEditor;
            m_colorEditor.IsGradientPropertyEnabled = this.IsGradientPropertyEnabled;
            m_colorEditor.Background = this.ColorEditBackground;
            m_colorEditor.Hcount = 0;
            if (this.m_colorEditor.SelectedColor != null)
                this.m_colorEditor.SelectedColor.Fill = this.Brush;
            if (VisualizationStyle == ColorSelectionMode.RGB)
            {
                this.m_colorEditor.CalculateHSVSelectorPosition();
                this.m_colorEditor.CalculateHSVBackground();
                this.m_colorEditor.CalculateBackground();
            }

            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        /// Executes when mouse button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" />
        /// instance containing the event data.</param>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured && e.RightButton == MouseButtonState.Pressed)
            {
                Mouse.Capture(null);
                m_colorToggleButton.IsChecked = !m_colorToggleButton.IsChecked;
            }
        }

        #endregion Internal
    }
}