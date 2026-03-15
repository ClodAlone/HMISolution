// <copyright file="ColorBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
 

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the class for the color bar
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/SyncOrange.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/ShinyRed.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/ShinyBlue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ColorBar), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ColorBar/Themes/Generic.xaml")]
    public class ColorBar : Control
    {
        #region Constants
        /// <summary>
        /// Contains name of the slider.
        /// </summary>
        private const string ColorBarSlider = "ColorBarSlider";
        #endregion

        #region Private members
        /// <summary>
        /// The slider value for HSV model.
        /// </summary>
        private float m_sliderValue;

        /// <summary>
        /// Contains true if mouse down or mouse capture done.
        /// </summary>
        private bool m_mouseClicked = false;
        #endregion 

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="ColorBar"/> class.
        /// </summary>
        static ColorBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorBar), new FrameworkPropertyMetadata(typeof(ColorBar)));
            //EnvironmentTest.ValidateLicense(typeof(ColorBar));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBar"/> class.
        /// </summary>
        public ColorBar()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ColorBar));
            }
            // SizeChanged += new SizeChangedEventHandler( ColorEdit_SizeChanged );
            // OnColorChanged( new DependencyPropertyChangedEventArgs( ColorProperty, Color, Colors.White ) );       
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when SliderValueHSV property is changed.
        /// </summary>
        public event PropertyChangedCallback SliderValueChanged;

        /// <summary>
        /// Event that is raised when Color property is changed.
        /// </summary>
        public event PropertyChangedCallback ColorChanged;
        #endregion

        #region Implementation

        /// <summary>
        /// Called when an internal process or application calls ApplyTemplate, which is used to build the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Called when slider value HSV is changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private static void OnSliderValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorBar instance = (ColorBar)d;
            instance.OnSliderValueChanged(e);
        }

        /// <summary>
        /// Invoked when an unhandled GotMouseCapture attached event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            m_mouseClicked = true;
            base.OnGotMouseCapture(e);
        }

        /// <summary>
        /// Invoked when an unhandled LostMouseCapture attached event reaches an element in its route that is derived from this class. 
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            m_mouseClicked = false;
            base.OnLostMouseCapture(e);
        }

        /// <summary>
        /// Invoked when PreviewMouseLeftButtonDown event is raised.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            m_mouseClicked = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when MouseLeftButtonUp event is raised.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            m_mouseClicked = false;
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Called when slider value HSV is changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// instance containing the event data.</param>
        private void OnSliderValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_sliderValue != (float)e.NewValue)
            {
                m_sliderValue = (float)e.NewValue;

                if (SliderValueChanged != null)
                {
                    SliderValueChanged(this, e);
                }

                if (m_mouseClicked)
                {
                    Color = HsvColor.ConvertHsvToRgb(m_sliderValue, 1, 1);
                }
            }
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
            ColorBar instance = (ColorBar)d;
            instance.OnColorChanged(e);
        }

        /// <summary>
        /// Raises ColorChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, e);
            }
        }

        #endregion

        #region Properties

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
        /// Gets or sets the slider value for HSV model.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// <para/>
        /// The slider value.
        /// </value>
        internal float SliderValue
        {
            get
            {
                return m_sliderValue;
            }

            set
            {
                SetValue(SliderValueProperty, value);
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
        private float SliderMaxValue
        {
            get
            {
                return (float)GetValue(SliderMaxValueProperty);
            }

            set
            {
                SetValue(SliderMaxValueProperty, value);
            }
        }

        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies ColorBar. Color dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Color"/>
        /// </value>
        public static readonly DependencyProperty ColorProperty =
          DependencyProperty.Register("Color", typeof(Color), typeof(ColorBar), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnColorChanged)));
       
        /// <summary>
        /// Identifies ColorBar. SliderValueHSV dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SliderValueProperty =
          DependencyProperty.Register("SliderValue", typeof(float), typeof(ColorBar), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnSliderValueChanged)));
        
        /// <summary>
        /// Identifies ColorBar. SliderMaxValueHSV dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="float"/>
        /// </value>
        internal static readonly DependencyProperty SliderMaxValueProperty =
          DependencyProperty.Register("SliderMaxValue", typeof(float), typeof(ColorBar), new FrameworkPropertyMetadata(360f));

        #endregion
    }
}
