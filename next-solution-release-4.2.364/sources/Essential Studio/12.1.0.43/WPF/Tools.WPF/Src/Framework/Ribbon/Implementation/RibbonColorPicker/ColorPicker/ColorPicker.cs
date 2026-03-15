// <copyright file="ColorPicker.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
    internal class xColorPicker : Control
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

        /// <summary>
        /// Contains system colors.
        /// </summary>
        private const string C_systemColors = "systemColors";
        #endregion

        #region Private Members
        /// <summary>
        /// Color editor for this control.
        /// </summary>
        private xColorEdit m_colorEditor;

        /// <summary>
        /// Popup for color editor.
        /// </summary>
        private Popup m_colorEditorPopup;

        /// <summary>
        /// Toggle Button for open popup.
        /// </summary>
        private ToggleButton m_colorToggleButton;

        /// <summary>
        /// Chosen color.
        /// </summary>
        private Color m_color = Colors.Black;

        /// <summary>
        /// Command for open popup.
        /// </summary>
        public static RoutedCommand M_displayPopup;

        /// <summary>
        /// Default background
        /// </summary>
        private Brush m_defaultBackground = null;
        #endregion

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
                return m_color;
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
        public xColorSelectionMode VisualizationStyle
        {
            get
            {
                return m_colorEditor.VisualizationStyle;
            }

            set
            {
                m_colorEditor.VisualizationStyle = value;
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
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Identifies ColorPicker.Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        internal static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(xColorPicker), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnColorChanged)));

        /// <summary>
        /// Identifies ColorPicker.IsColorPaletteVisible dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        internal static readonly DependencyProperty IsColorPaletteVisibleProperty =
            DependencyProperty.Register("IsColorPaletteVisible", typeof(bool), typeof(xColorPicker), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies ColorPicker.ColorEditBackground dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public static readonly DependencyProperty ColorEditBackgroundProperty =
            DependencyProperty.Register("ColorEditBackground", typeof(Brush), typeof(xColorPicker), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorEditBackgroundChanged)));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ColorPicker"/> class.
        /// </summary>
        static xColorPicker()
        {
            EnvironmentTest.ValidateLicense(typeof(xColorPicker));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(xColorPicker), new FrameworkPropertyMetadata(typeof(xColorPicker)));
            M_displayPopup = new RoutedCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPicker"/> class.. 
        /// Creates ColorPicker object.
        /// </summary>
        public xColorPicker()
        {
            CommandBinding displayPopup = new CommandBinding(M_displayPopup);
            displayPopup.Executed += new ExecutedRoutedEventHandler(DisplayPopup);
            CommandBindings.Add(displayPopup);
            m_colorEditor = new xColorEdit();
            Keyboard.AddKeyDownHandler(this, OnKeyDown);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
            Mouse.AddPreviewMouseDownHandler(this, OnMouseDown);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Color property is changed.
        /// </summary>
        public event PropertyChangedCallback ColorChanged;
        #endregion

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
            xColorPicker instance = (xColorPicker)d;
            instance.OnColorChanged(e);
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
            xColorPicker instance = (xColorPicker)d;
            instance.OnColorEditBackgroundChanged(e);
        }

        #endregion

        #region Internal
        /// <summary>
        /// When implemented in a derived class, will be invoked whenever
        /// application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_colorToggleButton = GetTemplateChild(C_colorToggleButton) as ToggleButton;
            m_colorEditorPopup = GetTemplateChild(C_colorEditPopup) as Popup;

            m_colorEditor = GetTemplateChild(C_colorEdit) as xColorEdit;

            if (m_colorEditor != null)
            {
                Binding binding = new Binding(C_color);
                binding.Source = this;
                binding.Mode = BindingMode.TwoWay;
                m_colorEditor.SetBinding(xColorEdit.ColorProperty, binding);
            }

            m_colorEditorPopup.Placement = PlacementMode.Bottom;

            SelectPaletteColor();

            m_colorEditorPopup.Opened += new EventHandler(ColorEditorPopup_Opened);
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (this.IsLoaded && e.Property == SkinStorage.VisualStyleProperty)
            {
                string style = SkinStorage.GetVisualStyle(this);
                Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
                if (list1 != null)
                {
                    Shared.DictionaryList list2 = list1[style] as Shared.DictionaryList;

                    if (SkinStorage.GetVisualStyle(this) != C_defaultSkinName)
                    {
                        if (list2 != null && list2.ContainsKey(C_colorEditContainerBrush))
                        {
                            ColorEditBackground = list2[C_colorEditContainerBrush] as Brush;
                        }
                    }
                    else
                    {
                        if (m_defaultBackground == null && list2 != null && list2.ContainsKey(C_colorEditContainerBrush))
                        {
                            ColorEditBackground = list2[C_colorEditContainerBrush] as Brush;
                        }

                        ColorEditBackground = m_defaultBackground;
                    }
                }
            }
            else if (e.Property == xColorPicker.ColorEditBackgroundProperty)
            {
                if (SkinStorage.GetVisualStyle(this) == C_defaultSkinName)
                {
                    m_defaultBackground = ColorEditBackground;
                }
            }
        }

        /// <summary>
        /// Sets color selected by user.
        /// </summary>
        private void SelectPaletteColor()
        {
            if (Template != null && IsColorPaletteVisible == true)
            {
                ComboBox obj = Template.FindName(C_systemColors, this) as ComboBox;
                if (obj != null)
                {
                    IList list = obj.ItemsSource as IList;
                    int index = -1;
                    for (int i = 0, cnt = list.Count; i < cnt; i++)
                    {
                        xColorItem item = list[i] as xColorItem;

                        if (item.Name == xColorEdit.SuchColor(Color)[0])
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

        /// <summary>
        /// Raises ColorChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            m_color = (Color)e.NewValue;

            if (ColorChanged != null)
            {
                ColorChanged(this, e);
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

        /// <summary>
        /// Changes the color.
        /// </summary>
        /// <param name="sender">Dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        public void ColorChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Color = (sender as xColorEdit).Color;
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

        #endregion
    }
}
 