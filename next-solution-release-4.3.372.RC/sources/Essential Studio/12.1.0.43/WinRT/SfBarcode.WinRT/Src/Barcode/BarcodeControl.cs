#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if BARCODE_WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
#else
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
#endif

#if WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Barcode
#else
namespace Syncfusion.UI.Xaml.Controls.Barcode
#endif
{
    /// <summary>
    /// Control class used to encode the text as bar code and render in Windows Store Apps.
    /// </summary>
    [TemplatePart(Name = "TextElement", Type = typeof(TextBlock))]
    [TemplatePart(Name = "BarcodeElement", Type = typeof(Canvas))]
    public class SfBarcode : Control
    {
        # region Fields
        private TextBlock m_textElement;
        private Canvas m_barcodeElement;
        private const double m_marginFactor = 0.03; // 3%
        private BarcodeTextLocation m_textLocation = BarcodeTextLocation.Bottom;
        private BarcodeRotation m_rotation = BarcodeRotation.Angle0;
        private readonly double fontSize;
        private readonly FontWeight fontWeight;
        # endregion

        # region Constructor
        /// <summary>
        /// Initializes BarcodeControl class.
        /// </summary>
        public SfBarcode()
        {
            DefaultStyleKey = typeof(SfBarcode);
            this.IsTabStop = true;
            this.Loaded += SfBarcodeControl_Loaded;

            fontSize = FontSize;
            fontWeight = FontWeight;
        }
        #endregion

        # region Properties
        /// <summary>
        /// Gets or sets the TextBlock used to display the original text.
        /// </summary>
        private TextBlock TextElement
        {
            get { return m_textElement; }
            set { m_textElement = value; }
        }

        /// <summary>
        /// Gets or sets the text to encode.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SfBarcode), new PropertyMetadata(String.Empty, new PropertyChangedCallback(TextChangedCallback)));

        /// <summary>
        /// Gets or sets the bar code symbol type.
        /// </summary>
        /// <value>
        /// The default value is Code39.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Barcode.BarcodeSymbolType"/>
        public BarcodeSymbolType Symbology
        {
            get { return (BarcodeSymbolType)GetValue(SymbologyProperty); }
            set { SetValue(SymbologyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Symbology.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbologyProperty =
            DependencyProperty.Register("Symbology", typeof(BarcodeSymbolType), typeof(SfBarcode), new PropertyMetadata(BarcodeSymbolType.Code39, new PropertyChangedCallback(AppearanceChangedCallBack)));

        /// <summary>
        /// Gets or sets the canvas to draw bar code.
        /// </summary>
        private Canvas BarcodeElement
        {
            get { return m_barcodeElement; }
            set { m_barcodeElement = value; }
        }

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfBarcode barcode = (SfBarcode)d;
            if (barcode.TextElement != null)
            {
                barcode.TextElement.Text = e.NewValue.ToString();
                barcode.DrawBarcode();
            }
        }

        /// <summary>
        /// Gets or sets whether to display original text.
        /// </summary>
        /// <value>
        /// The default value is True.
        /// </value>
        public bool DisplayText
        {
            get { return (bool)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register("DisplayText", typeof(bool), typeof(SfBarcode), new PropertyMetadata(true, new PropertyChangedCallback(DisplayTextCallBack)));

        private static void DisplayTextCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfBarcode barcode = (SfBarcode)d;
            if (barcode.TextElement != null)
                barcode.RecalculateSize();
        }

        private static void AppearanceChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfBarcode barcode = (SfBarcode)d;
            if (barcode.BarcodeElement != null)
                barcode.DrawBarcode();
        }

        /// <summary>
        /// Gets or sets the Brush for the lighter area of bar code.
        /// </summary>
        /// <value>
        /// The default brush is White.
        /// </value>
        public Brush LightBarBrush
        {
            get { return (Brush)GetValue(LightBarBrushProperty); }
            set { SetValue(LightBarBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LightBarBrushProperty =
            DependencyProperty.Register("LightBarBrush", typeof(Brush), typeof(SfBarcode), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(AppearanceChangedCallBack)));

        /// <summary>
        /// Gets or sets the Brush for the darker area of bar code.
        /// </summary>
        /// <value>
        /// The default brush is Black.
        /// </value>
        public Brush DarkBarBrush
        {
            get { return (Brush)GetValue(DarkBarBrushProperty); }
            set { SetValue(DarkBarBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DarkBarBrushProperty =
            DependencyProperty.Register("DarkBarBrush", typeof(Brush), typeof(SfBarcode), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(AppearanceChangedCallBack)));

        /// <summary>
        /// Gets or sets the height between bar code and the text.
        /// </summary>
        /// <remarks>
        /// The value is in pixel.
        /// </remarks>
        /// <value>
        /// The default value is 0.
        /// </value>
        public double TextGapHeight
        {
            get { return (double)GetValue(TextGapHeightProperty); }
            set { SetValue(TextGapHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextGapHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextGapHeightProperty =
            DependencyProperty.Register("TextGapHeight", typeof(double), typeof(SfBarcode), new PropertyMetadata(0.0, new PropertyChangedCallback(AppearanceChangedCallBack)));

        /// <summary>
        /// Gets or sets the QuietZone that surrounds the bar code.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Barcode.BarcodeQuietZones"/>
        public BarcodeQuietZones QuietZone
        {
            get { return (BarcodeQuietZones)GetValue(QuietZoneProperty); }
            set { SetValue(QuietZoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuietZone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuietZoneProperty =
            DependencyProperty.Register("QuietZone", typeof(BarcodeQuietZones), typeof(SfBarcode), new PropertyMetadata(new BarcodeQuietZones(), new PropertyChangedCallback(AppearanceChangedCallBack)));

        /// <summary>
        /// Gets or sets the text brush.
        /// </summary>
        /// <value>
        /// The default brush is Black.
        /// </value>
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(SfBarcode), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(DisplayTextCallBack)));

        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        /// <value>
        /// The default value is Center.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Barcode.BarcodeTextAlignment"/>
        public BarcodeTextAlignment TextAlignment
        {
            get { return (BarcodeTextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(BarcodeTextAlignment), typeof(SfBarcode), new PropertyMetadata(BarcodeTextAlignment.Center));


        /// <summary>
        /// Gets or sets the Settings for the applied bar code type.
        /// </summary>
        public Settings SymbologySettings
        {
            get { return (Settings)GetValue(SymbologySettingsProperty); }
            set { SetValue(SymbologySettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbologySettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbologySettingsProperty =
            DependencyProperty.Register("SymbologySettings", typeof(Settings), typeof(SfBarcode), new PropertyMetadata(new Settings()));

        /// <summary>
        /// Gets of sets the vertical alignment of the text.
        /// </summary>
        /// <value>
        /// The default value is Bottom.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Barcode.BarcodeTextLocation"/>
        public BarcodeTextLocation TextLocation
        {
            get
            {
                return m_textLocation;
            }
            set
            {
                m_textLocation = value;
                if (m_textElement != null)
                {
                    if (m_textLocation == BarcodeTextLocation.Bottom)
                        Grid.SetRow(m_textElement, 2);
                    else if (m_textLocation == BarcodeTextLocation.Top)
                        Grid.SetRow(m_textElement, 0);
                    RecalculateSize();
                }
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        internal int Angle
        {
            get { return (int)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RotateAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(int), typeof(SfBarcode), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the center based X value for rotation.
        /// </summary>
        internal double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterX.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register("CenterX", typeof(double), typeof(SfBarcode), new PropertyMetadata(Double.NaN));

        /// <summary>
        /// Gets or sets the center based Y value for rotation.
        /// </summary>
        internal double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterY.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register("CenterY", typeof(double), typeof(SfBarcode), new PropertyMetadata(Double.NaN));


        /// <summary>
        /// Gets or sets the rotation of the control.
        /// </summary>
        /// <value>
        /// The default value is Angle0.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Barcode.BarcodeRotation"/>
        public BarcodeRotation Rotation
        {
            get
            {
                return m_rotation;
            }
            set
            {
                m_rotation = value;
                switch (m_rotation)
                {
                    case BarcodeRotation.Angle0:
                    case BarcodeRotation.Angle180:
                        Angle = 0;
                        CenterX = Double.NaN;
                        CenterY = Double.NaN;
                        break;
                    case BarcodeRotation.Angle90: //Rotate based on canvas center to avoid blur effect.
                        Angle = 90;
                        CenterX = m_barcodeElement.Width / 2;
                        CenterY = m_barcodeElement.Height / 2;
                        break;
                    case BarcodeRotation.Angle270: //Rotate based on canvas center to avoid blur effect.
                        Angle = -90;
                        CenterX = m_barcodeElement.Width / 2;
                        CenterY = m_barcodeElement.Height / 2;
                        break;
                }
            }
        }
        # endregion

        # region Implementation
        /// <summary>
        /// Handles loaded event of the control.
        /// </summary>
        void SfBarcodeControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrawBarcode();
        }

        /// <summary>
        /// Adjusts the size of the control based on value change.
        /// </summary>
#if !BARCODE_WINRT
        protected override Size MeasureOverride(Size availableSize)
#else
        protected override Size MeasureOverride(Size availableSize)
#endif
        {
            Size newSize = base.MeasureOverride(availableSize);
            m_textElement.Measure(newSize);
            DrawBarcode();
            Size size = new Size(double.IsNaN(Width) ? newSize.Width : Width, double.IsNaN(Height) ? newSize.Height : Height);
            return size;
        }

        /// <summary>
        /// Draws bar code.
        /// </summary>
        private void DrawBarcode()
        {
#if !BARCODE_WINRT
            this.Visibility = System.Windows.Visibility.Visible;
#else
            this.Visibility = Windows.UI.Xaml.Visibility.Visible;
#endif
            if (!String.IsNullOrEmpty(Text))
            {
                m_barcodeElement.Children.Clear();

                UnidimensionalBarcode m_barcode1D = null;
                DataMatrixBarcode m_dataMatrixBarcode = null;
                QRBarcode m_qrBarcode = null;
                Barcode1DSetting typeSetting = null;
                IBarcodeSetting setting = SymbologySettings as IBarcodeSetting;

                switch (Symbology)
                {
                    case BarcodeSymbolType.Code39:
                        {
                            m_barcode1D = new Code39Barcode();
                            typeSetting = setting as Code39Setting;
                        }
                        break;
                    case BarcodeSymbolType.Code39Extended:
                        {
                            m_barcode1D = new Code39ExtendedBarcode();
                            typeSetting = setting as Code39ExtendedSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code11:
                        {
                            m_barcode1D = new Code11Barcode();
                            typeSetting = setting as Code11Setting;
                        }
                        break;
                    case BarcodeSymbolType.Codabar:
                        {
                            m_barcode1D = new CodabarBarcode();
                            typeSetting = setting as CodabarSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code32:
                        {
                            m_barcode1D = new Code32Barcode();
                            typeSetting = setting as Code32Setting;
                        }
                        break;
                    case BarcodeSymbolType.Code93:
                        {
                            m_barcode1D = new Code93Barcode();
                            typeSetting = setting as Code93Setting;
                        }
                        break;
                    case BarcodeSymbolType.Code93Extended:
                        {
                            m_barcode1D = new Code93ExtendedBarcode();
                            typeSetting = setting as Code93ExtendedSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code128A:
                        {
                            m_barcode1D = new Code128ABarcode();
                            typeSetting = setting as Code128ASetting;
                        }
                        break;
                    case BarcodeSymbolType.Code128B:
                        {
                            m_barcode1D = new Code128BBarcode();
                            typeSetting = setting as Code128BSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code128C:
                        {
                            m_barcode1D = new Code128CBarcode();
                            typeSetting = setting as Code128CSetting;
                        }
                        break;
                    case BarcodeSymbolType.DataMatrix:
                        m_dataMatrixBarcode = new DataMatrixBarcode();
                        break;
                    case BarcodeSymbolType.QRBarcode:
                        m_qrBarcode = new QRBarcode();
                        break;
                    default:
                        break;
                }

                if (m_barcode1D != null)
                {
                    m_barcode1D.Text = m_textElement.Text;
                    if (LightBarBrush is SolidColorBrush)
                        m_barcode1D.BackColor = (LightBarBrush as SolidColorBrush).Color;
                    if (DarkBarBrush is SolidColorBrush)
                        m_barcode1D.BarColor = (DarkBarBrush as SolidColorBrush).Color;
                    if (QuietZone.All > 0.0)
                        m_barcode1D.QuietZone = QuietZone;

                    if (typeSetting != null)
                    {
                        m_barcode1D.BarHeight = (float)typeSetting.BarHeight;
                        m_barcode1D.EnableCheckDigit = typeSetting.EnableCheckDigit;
                        m_barcode1D.EncodeStartStopSymbols = typeSetting.EncodeStartStopSymbols;
                        m_barcode1D.ShowCheckDigit = typeSetting.ShowCheckDigit;
                        m_barcode1D.NarrowBarWidth = (float)typeSetting.NarrowBarWidth;
                    }

                    try
                    {
                        m_barcode1D.Draw(m_barcodeElement);

                        FontSize = fontSize;
                        FontWeight = fontWeight;
                    }
                    catch (BarcodeException ex) // Invalid data to encode.
                    {
#if !BARCODE_WINRT
                        this.Visibility = System.Windows.Visibility.Collapsed;
#else
                        this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
#endif
                        return;
                    }
                }
                else if (m_dataMatrixBarcode != null)
                {
                    m_dataMatrixBarcode.Text = m_textElement.Text;
                    if (QuietZone.All > 0.0)
                        m_dataMatrixBarcode.QuietZone = QuietZone;

                    DataMatrixSetting dmSetting = setting as DataMatrixSetting;

                    if (dmSetting != null)
                    {
                        m_dataMatrixBarcode.XDimension = (float)dmSetting.XDimension;
                        m_dataMatrixBarcode.Encoding = dmSetting.Encoding;
                        m_dataMatrixBarcode.Size = dmSetting.Size;
                    }

                    m_dataMatrixBarcode.Draw(m_barcodeElement);

                    if (FontSize <= fontSize)
                        FontSize += 2;
#if !BARCODE_WINRT
                    if (FontWeight == FontWeights.Normal || FontWeight == FontWeights.Light)
#else
                    if (FontWeight.Weight == FontWeights.Normal.Weight || FontWeight.Weight == FontWeights.Light.Weight)
#endif
                        FontWeight = FontWeights.Bold;
                }
                else if (m_qrBarcode != null)
                {
                    m_qrBarcode.Text = m_textElement.Text;

                    if (QuietZone.All > 0.0)
                        m_qrBarcode.QuietZone = QuietZone;

                    QRBarcodeSetting qrSetting = setting as QRBarcodeSetting;

                    if (qrSetting != null)
                    {
                        m_qrBarcode.XDimension = (float)qrSetting.XDimension;
                        m_qrBarcode.QRVersion = qrSetting.Version;
                        m_qrBarcode.ErrorCorrectionLevel = qrSetting.ErrorCorrectionLevel;
                        m_qrBarcode.InputMode = qrSetting.InputMode;
                    }

                    m_qrBarcode.Draw(m_barcodeElement);

                    if (FontSize <= fontSize)
                        FontSize += 2;
#if !BARCODE_WINRT
                    if (FontWeight == FontWeights.Normal || FontWeight == FontWeights.Light)
#else
                    if (FontWeight.Weight == FontWeights.Normal.Weight || FontWeight.Weight == FontWeights.Light.Weight)
#endif
                        FontWeight = FontWeights.Bold;
                }

                RecalculateSize();
            }
        }

        /// <summary>
        /// Applies control template.
        /// </summary>
#if !BARCODE_WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            BarcodeElement = GetTemplateChild("BarcodeElement") as Canvas;
            TextElement = GetTemplateChild("TextElement") as TextBlock;

            TextElement.SetBinding(TextBlock.TextAlignmentProperty, new Binding() { Path = new PropertyPath("TextAlignment"), Source = this, Converter = new TextAlignmentConverter() });
        }

        /// <summary>
        /// Calculates the size of the control.
        /// </summary>
        private void RecalculateSize()
        {
            if (m_barcodeElement != null && !double.IsNaN(m_barcodeElement.Height))
            {
                m_textElement.Measure(Size.Empty);
                double width, height, leftMargin;
                Thickness barcodeMargin = new Thickness(); // Margin around Canvas.
                Thickness textMargin = new Thickness(); // Margin around TextBlock.

                if (TextLocation == BarcodeTextLocation.Bottom)
                {
                    textMargin.Top = TextGapHeight;
                    textMargin.Bottom = m_textElement.ActualHeight * m_marginFactor;
                    barcodeMargin.Top = m_barcodeElement.Height * m_marginFactor;
                }
                else
                {
                    textMargin.Top = m_textElement.ActualHeight * m_marginFactor;
                    textMargin.Bottom = TextGapHeight;
                    barcodeMargin.Bottom = m_barcodeElement.Height * m_marginFactor;
                }

                if (DisplayText.ToString().ToLower() == "true")
                {
                    if (m_barcodeElement.Width > m_textElement.ActualWidth)
                    {
                        leftMargin = m_barcodeElement.Height * m_marginFactor;
                        width = m_barcodeElement.Width + 2 * leftMargin;
                        barcodeMargin.Left = leftMargin;
                        barcodeMargin.Right = leftMargin;
                    }
                    else
                    {
                        leftMargin = m_textElement.ActualWidth * m_marginFactor;
                        width = m_textElement.ActualWidth + 2 * leftMargin;
                        textMargin.Left = leftMargin;
                        textMargin.Right = leftMargin;
                    }

                    m_textElement.Margin = textMargin;
                    height = m_barcodeElement.Height + m_textElement.ActualHeight + barcodeMargin.Top + TextGapHeight + textMargin.Bottom;
#if !BARCODE_WINRT
                    m_textElement.Visibility = Visibility.Visible;
#else
                    m_textElement.Visibility = Windows.UI.Xaml.Visibility.Visible;
#endif
                }
                else
                {
                    leftMargin = m_barcodeElement.Height * m_marginFactor;
                    barcodeMargin.Bottom = leftMargin;

                    width = m_barcodeElement.Width + 2 * leftMargin;
                    height = m_barcodeElement.Height + barcodeMargin.Top + barcodeMargin.Bottom;

                    m_textElement.Margin = new Thickness(0);
#if !BARCODE_WINRT
                    m_textElement.Visibility = System.Windows.Visibility.Collapsed;
#else
                    m_textElement.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
#endif
                }

                m_barcodeElement.Margin = barcodeMargin;
                Width = width;
                Height = height;
            }
        }
        # endregion
    }

    # region Enumerations
    /// <summary>
    /// Specifies barcode symbol type
    /// </summary>
    public enum BarcodeSymbolType
    {
        Code39,
        Code39Extended,
        Code11,
        Codabar,
        Code32,
        Code93,
        Code93Extended,
        Code128A,
        Code128B,
        Code128C,
        DataMatrix,
        QRBarcode
    }

    /// <summary>
    /// Specifies barcode rotation.
    /// </summary>
    public enum BarcodeRotation
    {
        Angle0,
        Angle90,
        Angle180,
        Angle270,
    }
    #endregion

    # region ConverterClass
    /// <summary>
    /// Converts TextAlignment to BarcodeTextAlignment and viceversa.
    /// </summary>
    internal class TextAlignmentConverter : IValueConverter
    {
#if !BARCODE_WINRT
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo info)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            if (value is BarcodeTextAlignment)
            {
#if !BARCODE_WINRT
                TextAlignment align = (TextAlignment)Enum.Parse(typeof(TextAlignment), value.ToString(),true);
#else
                Windows.UI.Xaml.TextAlignment align = (Windows.UI.Xaml.TextAlignment)Enum.Parse(typeof(Windows.UI.Xaml.TextAlignment), value.ToString());
#endif
                return align;
            }
            return DependencyProperty.UnsetValue;
        }

#if !BARCODE_WINRT
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo info)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            if (value is TextAlignment)
            {
                BarcodeTextAlignment textAlignment = (BarcodeTextAlignment)Enum.Parse(typeof(BarcodeTextAlignment), value.ToString(),true);
                return textAlignment;
            }
            return DependencyProperty.UnsetValue;
        }
    }
    # endregion
}
