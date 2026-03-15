#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

#if WPF

using Syncfusion.Licensing;

#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// A control having a set of ColorGroupItems
    /// </summary>
    public class ColorGroup : Control
    {
        /// <summary>
        /// An instance of ColorPickerPalette class
        /// </summary>
        public ColorPickerPalette colorpicker;

        /// <summary>
        /// Identifies the HeaderName dependency property
        /// </summary>
        public static readonly DependencyProperty HeadNameProperty = DependencyProperty.Register("HeaderName", typeof(string), typeof(ColorGroup), new PropertyMetadata(IsHeaderChanged));

        /// <summary>
        /// Identifies the HeaderVisibilityProperty dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(ColorGroup), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the color property
        /// </summary>
        internal static readonly DependencyProperty ColorProperty = DependencyProperty.Register("color", typeof(Brush), typeof(ColorGroup), new PropertyMetadata(new PropertyChangedCallback(IsColorChanged)));

        /// <summary>
        /// Identifies the DataSource dependency property
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(ObservableCollection<ColorGroupItem>), typeof(ColorGroup), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the PanelVisibility dependency property
        /// </summary>
        public static readonly DependencyProperty PanelVisibilityproperty = DependencyProperty.Register("PanelVisibility", typeof(Visibility), typeof(ColorGroup), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ThemeHeaderForeGround"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeForeGroundProperty = DependencyProperty.Register("ThemeHeaderForeGround", typeof(Brush), typeof(ColorGroup), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 50, 21, 110))));

        /// <summary>
        /// Identifies the <see cref="ThemeHeaderBackGround"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemeBackGroundProperty = DependencyProperty.Register("ThemeHeaderBackGround", typeof(Brush), typeof(ColorGroup), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 221, 231, 238))));

        /// <summary>
        /// Identifies the ColorName dependency property
        /// </summary>
        public static new readonly DependencyProperty NameProperty = DependencyProperty.Register("ColorName", typeof(string), typeof(ColorGroup), new PropertyMetadata("hi"));

        /// <summary>
        /// Gets or sets the value of the PanelVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorPickerPalette  Name=&quot;ColorPickerPalette&quot;
        /// ThemePanelVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorGroup ColGroup=new ColorGroup();</para>
        /// <para>ColGroup.PanelVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility PanelVisibility
        {
            get
            {
                return (Visibility)GetValue(PanelVisibilityproperty);
            }

            set
            {
                SetValue(PanelVisibilityproperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header visibility.
        /// </summary>
        /// <value>The header visibility.</value>
        public Visibility HeaderVisibility
        {
            get
            {
                return (Visibility)GetValue(HeaderVisibilityProperty);
            }

            set
            {
                SetValue(HeaderVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Color dependency property
        /// </summary>
        public Brush color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ThemeForeGround dependency property.
        /// </summary>
        public Brush ThemeHeaderForeGround
        {
            get
            {
                return (Brush)GetValue(ThemeForeGroundProperty);
            }

            set
            {
                SetValue(ThemeForeGroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ThemeBackGround dependency property.
        /// </summary>
        public Brush ThemeHeaderBackGround
        {
            get
            {
                return (Brush)GetValue(ThemeBackGroundProperty);
            }

            set
            {
                SetValue(ThemeBackGroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the DataSource dependency property
        /// </summary>
        public ObservableCollection<ColorGroupItem> DataSource
        {
            get
            {
                return (ObservableCollection<ColorGroupItem>)GetValue(DataSourceProperty);
            }

            set
            {
                SetValue(DataSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ColorName dependency property
        /// </summary>
        public string ColorName
        {
            get
            {
                return (string)GetValue(NameProperty);
            }

            set
            {
                SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the HeaderName dependency property
        /// </summary>
        public string HeaderName
        {
            get
            {
                return (string)GetValue(HeadNameProperty);
            }

            set
            {
                SetValue(HeadNameProperty, value);
            }
        }

        /// <summary>
        /// Creates the instance of ColorGroup control
        /// </summary>
        public ColorGroup()
        {
            DefaultStyleKey = typeof(ColorGroup);
        }

#if WPF

        static ColorGroup()
        {
            EnvironmentTest.ValidateLicense(typeof(ColorGroup));
        }

#endif
        internal ItemsControl colorGroupItemsControl;
#if WPF
        public Rectangle cgHeaderName;
        public TextBlock cgHeaderTextBox;
#endif

        /// <summary>
        /// Applies the Template for the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            colorGroupItemsControl = GetTemplateChild("Ic") as ItemsControl;
            base.OnApplyTemplate();
#if WPF
            cgHeaderName = GetTemplateChild("CGHeaderName") as Rectangle;
            cgHeaderTextBox = GetTemplateChild("CGTextBox") as TextBlock;
            cgHeaderName.MouseLeftButtonDown += new MouseButtonEventHandler(cgHeaderName_MouseLeftButtonDown);
            cgHeaderTextBox.MouseLeftButtonDown += new MouseButtonEventHandler(cgHeaderName_MouseLeftButtonDown);
#endif
        }

#if WPF

        public void cgHeaderName_MouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (colorpicker != null && this.colorpicker.Popup != null)
            {
                if (!this.colorpicker.IsExpanded)
                    this.colorpicker.Popup.IsOpen = true;
                this.colorpicker.updownclick = true;
            }
        }

#endif

        /// <summary>
        /// Event raised when HeaderName is changed
        /// </summary>
        /// <param name="o">ColorGroup object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void IsHeaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorGroup g = (ColorGroup)o;
            g.IsHeaderChanged(e);
        }

        /// <summary>
        /// Method called when IsHeaderChanged event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void IsHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        ///  Event raised when Color is changed
        /// </summary>
        /// <param name="o">ColorGroup object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void IsColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorGroup g = (ColorGroup)o;
            g.IsColorChanged(e);
        }

        /// <summary>
        /// Method called when IsColorChanged event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void IsColorChanged(DependencyPropertyChangedEventArgs e)
        {
            this.colorpicker.ColorName = this.ColorName;
            this.colorpicker.Color = ((SolidColorBrush)this.color).Color;
        }
    }
}