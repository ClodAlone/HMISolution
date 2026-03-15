#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Enum values for ChartStyles
    /// </summary>
    public enum ChartStyles
    {
        /// <summary>
        /// Enum value for GrayScale style
        /// </summary>
        GrayScale = 1,
        /// <summary>
        /// Enum value for MixedFantacy style
        /// </summary>
        MixedFantacy = 2,
        /// <summary>
        /// Enum value for BlueScale Style
        /// </summary>
        BlueScale = 3,
        /// <summary>
        /// Enum value for maroonRed Style
        /// </summary>
        MaroonRed = 4,
        /// <summary>
        /// Enum value for GreebScale Style
        /// </summary>
        GreenScale = 5,
        /// <summary>
        /// Enum value for MixedViolet Style
        /// </summary>
        MixedViolet = 6,
        /// <summary>
        /// Enum value for CoolBlueScale style
        /// </summary>
        CoolBlueScale = 7,
        /// <summary>
        /// Enum value for ChocolateOrange Style
        /// </summary>
        ChocolateOrange = 8,
        /// <summary>
        /// Enum value for GrayWithBorder Style
        /// </summary>
        GrayWithBorder = 9,
        /// <summary>
        /// Enum value for MixedWithBorder Style
        /// </summary>
        MixedWithBorder = 10,
        /// <summary>
        /// Enum value for BlueWithBorder Style
        /// </summary>
        BlueWithBorder = 11,
        /// <summary>
        /// Enum value for RedWithBorder Style
        /// </summary>
        RedWithBorder = 12,
        /// <summary>
        /// Enum value for GreenWithBorder style
        /// </summary>
        GreenWithBorder = 13,
        /// <summary>
        /// Enum value for VioletWithBorder style
        /// </summary>
        VioletWithBorder = 14,
        /// <summary>
        /// Enum value for CoolBlueWithBorder Style
        /// </summary>
        CoolBlueWithBorder = 15,
        /// <summary>
        /// Enum value for ChocolateWithBorder style
        /// </summary>
        ChocolateWithBorder = 16,
        /// <summary>
        /// Enum value for AlphaGray Style
        /// </summary>
        AlphaGray = 17,
        /// <summary>
        /// enum value for AlphaFantacy style
        /// </summary>
        AlphaFantacy = 18,
        /// <summary>
        /// Enum value for alphaBlue style
        /// </summary>
        AlphaBlue = 19,
        /// <summary>
        /// Enum value for Alphared Style
        /// </summary>
        AlphaRed = 20,
        /// <summary>
        /// Enum value for Alphagreen Style
        /// </summary>
        AlphaGreen = 21,
        /// <summary>
        /// enum value for AlphaViolet Style
        /// </summary>
        AlphaViolet = 22,
        /// <summary>
        /// Enum value for AlphaCoolBlue style
        /// </summary>
        AlphaCoolBlue = 23,
        /// <summary>
        /// Enum value for AlphaOrannge Style
        /// </summary>
        AlphaOrange = 24,
        /// <summary>
        /// Enum value for EnabledGray Style
        /// </summary>
        EnabledGray = 25,
        /// <summary>
        /// Enum value for EnabledMixed Style
        /// </summary>
        EnabledMixed = 26,
        /// <summary>
        /// Enum value for EnabledBlue Style
        /// </summary>
        EnabledBlue = 27,
        /// <summary>
        /// Enum value for EnabledRed Style
        /// </summary>
        EnabledRed = 28,
        /// <summary>
        /// Enum value for Enabled Green Style
        /// </summary>
        EnabledGreen = 29,
        /// <summary>
        /// Enum value for EnabledViolet Style
        /// </summary>
        EnabledViolet = 30,
        /// <summary>
        /// Enum value for EnabledCoolBlue style
        /// </summary>
        EnabledCoolBlue = 31,
        /// <summary>
        /// Enum  value for EnabledChocolate Style
        /// </summary>
        EnabledChocolate = 32,
        /// <summary>
        /// Enum value for GrayScreen style
        /// </summary>
        GrayScreen = 33,
        /// <summary>
        /// Enum value for MixedScreen Style
        /// </summary>
        MixedScreen = 34,
        /// <summary>
        /// Enum value for BlueScreen style
        /// </summary>
        BlueScreen = 35,
        /// <summary>
        /// Enum value for RedScreen Style
        /// </summary>
        RedScreen = 36,
        /// <summary>
        /// Enum value for GreenScree style
        /// </summary>
        GreenScreen = 37,
        /// <summary>
        /// Enum  value for VioletScreen Style
        /// </summary>
        VioletScreen = 38,
        /// <summary>
        /// Enum value for CoolBlueScreen Style
        /// </summary>
        CoolBlueScreen = 39,
        /// <summary>
        /// Enum value for ChocolateScreen style
        /// </summary>
        ChocolateScreen = 40,
        /// <summary>
        /// Enum value for BlenGray style
        /// </summary>
        BlendGray = 41,
        /// <summary>
        /// Enum value for MixedBlend Style
        /// </summary>
        MixedBlend = 42,
        /// <summary>
        /// enum value for BlueBlend style
        /// </summary>
        BlueBlend = 43,
        /// <summary>
        /// Enum value for RedBlend Style
        /// </summary>
        RedBlend = 44,
        /// <summary>
        /// Enum value for GreenBlend Style
        /// </summary>
        GreenBlend = 45,
        /// <summary>
        /// Enum value for VioletBlend style
        /// </summary>
        VioletBlend = 46,
        /// <summary>
        /// Enum value for CoolBlueBlend Style
        /// </summary>
        CoolBlueBlend = 47,
        /// <summary>
        /// Enum value for ChocolateBlend Style
        /// </summary>
        ChocolateBlend = 48,
        /// <summary>
        /// Enum value for Default style
        /// </summary>
        Default = 49,
        /// <summary>
        /// Enum value for Blend Style
        /// </summary>
        Blend = 50,
        /// <summary>
        /// Enum value for Office2003 style
        /// </summary>
        Office2003 = 51,
        /// <summary>
        /// Enum value for Office2007Blue style
        /// </summary>
        Office2007Blue = 52,
        /// <summary>
        /// enum value for Office2007Black style
        /// </summary>
        Office2007Black = 53,
        /// <summary>
        /// Enum value for Office2007Silver style
        /// </summary>
        Office2007Silver = 54,
        /// <summary>
        /// Enum value for VS2010 style
        /// </summary>
        VS2010=55,
        /// <summary>
        /// Enum value for Metro Style
        /// </summary>
        Metro=56,

    }


    /// <summary>
    /// Pre-defined palettes for use with the ChartControl. Palettes are simply a group of colors that
    /// can be used to provide a better visual appearance when displaying multiple chart series.
    /// </summary>    
    public enum ChartColorPalette
    {
        /// <summary>
        /// Default palette.
        /// </summary> 
        Default,

        /// <summary>
        /// Default palette with alpha blending.
        /// </summary>
        DefaultAlpha,

        /// <summary>
        /// Default dark palette.
        /// </summary> 
        DefaultDark,

        /// <summary>
        /// Palette containing earth tone colors.
        /// </summary>
        EarthTone,

        /// <summary>
        /// Palette containing analog colors.
        /// </summary>
        Analog,

        /// <summary>
        /// Colorful palette.
        /// </summary>
        Colorful,

        /// <summary>
        /// Palette containing the colors of nature.
        /// </summary>
        Nature,

        /// <summary>
        /// Palette containing pastel colors.
        /// </summary>
        Pastel,

        /// <summary>
        /// Palette containing triad colors.
        /// </summary>
        Triad,

        /// <summary>
        /// Palette that contains mixed warm and cold colors.
        /// </summary>
        WarmCold,

        /// <summary>
        /// GrayScale color palette which can be used for monochrome printing.
        /// </summary>
        Grayscale,

        /// <summary>
        /// Custom user assigned color palette.
        /// </summary>
        Custom,

        /// <summary>
        /// Enum value for Metro colorPalette
        /// </summary>
        Metro,
        /// <summary>
        /// Enum value for Palette1 colorPalette
        /// </summary>
        Palette1,
        /// <summary>
        /// Enum value for Palette2 colorPalette
        /// </summary>
        Palette2,
        /// <summary>
        /// Enum value for Palette3 colorPalette
        /// </summary>
        Palette3,
        /// <summary>
        /// Enum value for Palette4 colorPalette
        /// </summary>
        Palette4,
        /// <summary>
        /// Enum value for Palette5 colorPalette
        /// </summary>
        Palette5,
        /// <summary>
        /// Enum value for Palette6 colorPalette
        /// </summary>
        Palette6,
        /// <summary>
        /// Enum value for Palette7 colorPalette
        /// </summary>
        Palette7,
        /// <summary>
        /// Enum value for Palette8 colorPalette
        /// </summary>
        Palette8,
    }
    /// <summary>
    /// Class implentation for ChartStyleModel
    /// </summary>
    public class ChartStyleModel:DependencyObject,IDisposable
    {
        #region Members
        /// <summary>
        /// Initializes m_defaultPalette
        /// </summary>
        private static readonly Brush[] m_defaultPalette;

        /// <summary>
        /// Initializes m_defaultAlphaPalette
        /// </summary>
        private static readonly Brush[] m_defaultAlphaPalette;

        /// <summary>
        /// Initializes m_defaultDarkPalette
        /// </summary>
        private static readonly Brush[] m_defaultDarkPalette;

        /// <summary>
        /// Initializes m_earthTonePalette
        /// </summary>
        private static readonly Brush[] m_earthTonePalette;

        /// <summary>
        /// Initializes m_analogPalette
        /// </summary>
        private static readonly Brush[] m_analogPalette;

        /// <summary>
        /// Initializes m_colorfulPalette
        /// </summary>
        private static readonly Brush[] m_colorfulPalette;

        /// <summary>
        /// Initializes m_naturePalette
        /// </summary>
        private static readonly Brush[] m_naturePalette;

        /// <summary>
        /// Initializes m_pastelPalette
        /// </summary>
        private static readonly Brush[] m_pastelPalette;

        /// <summary>
        /// Initializes m_triadPalette
        /// </summary>
        private static readonly Brush[] m_triadPalette;

        /// <summary>
        /// Initializes m_warmColdPalette
        /// </summary>
        private static readonly Brush[] m_warmColdPalette;

        /// <summary>
        /// Initializes m_grayScalePalette
        /// </summary>
        private static readonly Brush[] m_grayScalePalette;

        private static readonly Brush[] m_metroThemePalette;

        private static readonly Brush[] m_Palette1;

        private static readonly Brush[] m_Palette2;

        private static readonly Brush[] m_Palette3;

        private static readonly Brush[] m_Palette4;

        private static readonly Brush[] m_Palette5;

        private static readonly Brush[] m_Palette6;

        private static readonly Brush[] m_Palette7;

        private static readonly Brush[] m_Palette8;

        /// <summary>
        /// Initializes m_area
        /// </summary>
        private ChartArea m_area;

       

        /// <summary>
        /// Initializes m_brushes
        /// </summary>
        private Brush[] m_brushes = m_defaultPalette;

        /// <summary>
        /// Initializes m_customPalette
        /// </summary>
        private  Brush[] m_customPalette;
        #endregion



        #region Dependency properties


        /// <summary>
        /// Sets the ColorPalette for the Chart Area.
        /// </summary>
        public static readonly DependencyProperty ColorPaletteProperty =
           DependencyProperty.Register("Palette", typeof(ChartColorPalette), typeof(ChartStyleModel), new PropertyMetadata(ChartColorPalette.Default,new PropertyChangedCallback(OnPaletteChanged)));
        /// <summary>
        /// Sets the CustomPalette for the Chart Area
        /// </summary>
        public static readonly DependencyProperty CustomPaletteProperty =
          DependencyProperty.Register("CustomPalette", typeof(Brush[]), typeof(ChartStyleModel), new PropertyMetadata(null,new PropertyChangedCallback(OnCustomePaletteChanged)));

        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        /// <value>The palette.</value>
        /// 

        public ChartColorPalette Palette
        {

            get { return (ChartColorPalette)GetValue(ColorPaletteProperty); }

            set { SetValue(ColorPaletteProperty, value); }

        }

        /// <summary>
        /// Called when Palette property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartStyleModel instance = d as ChartStyleModel;
            if (instance != null && instance.m_area != null)
                instance.m_area.LoadArea();
        }
        /// <summary>
        /// Gets or sets the custom palette.
        /// </summary>
        /// <value>The custom palette.</value>
        /// 

        public Brush[] CustomPalette
        {

            get { return (Brush[])GetValue(CustomPaletteProperty); }

            set { SetValue(CustomPaletteProperty, value);  }

        }

        private static void OnCustomePaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartStyleModel stm = (ChartStyleModel)d;
            stm.m_customPalette = (e.NewValue) as Brush[];
        }
        /// <summary>
        /// Gets the CurrentPalette Brushes
        /// </summary>
        public Brush[] CurrentPalette
        {
            get
            {
                return GetBrushes(this.Palette);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartStyleModel"/> class.
        /// </summary>
        static ChartStyleModel()
        {
            #region Default palette
            m_defaultPalette = new Brush[]
      {
        ////new SolidColorBrush(Color.FromArgb(255, 169, 185, 205)), 
        ////new SolidColorBrush(Color.FromArgb(255, 227, 145, 96)), 
        ////new SolidColorBrush(Color.FromArgb(255, 237, 196, 114)), 
        ////new SolidColorBrush(Color.FromArgb(255, 94, 158, 199)), 
        ////new SolidColorBrush(Color.FromArgb(255, 196, 99, 82)), 
        ////new SolidColorBrush(Color.FromArgb(255, 143, 167, 69)), 
        ////new SolidColorBrush(Color.FromArgb(255, 53, 83, 107)), 
        ////new SolidColorBrush(Color.FromArgb(255, 225, 216, 132)), 
        ////new SolidColorBrush(Color.FromArgb(255, 136, 157, 159)), 
        ////new SolidColorBrush(Color.FromArgb(255, 155, 135, 86))
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush1") as Brush,
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush2") as Brush,
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush3") as Brush,
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush4") as Brush,
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush5") as Brush,
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush6") as Brush,
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush7") as Brush,
        ResourceManager.GetSeriesLayer(typeof(ChartStyleModel),"DefaultColorBrush8") as Brush
      };
            #endregion

            #region DefaultAlpha palette
            m_defaultAlphaPalette = new Brush[]
      {
        new SolidColorBrush(Color.FromArgb(150, 169, 185, 205)), 
        new SolidColorBrush(Color.FromArgb(150, 227, 145, 96)), 
        new SolidColorBrush(Color.FromArgb(150, 237, 196, 114)), 
        new SolidColorBrush(Color.FromArgb(150, 94, 158, 199)), 
        new SolidColorBrush(Color.FromArgb(150, 196, 99, 82)), 
        new SolidColorBrush(Color.FromArgb(150, 143, 167, 69)), 
        new SolidColorBrush(Color.FromArgb(150, 53, 83, 107)), 
        new SolidColorBrush(Color.FromArgb(150, 225, 216, 132)), 
        new SolidColorBrush(Color.FromArgb(150, 136, 157, 159)), 
        new SolidColorBrush(Color.FromArgb(150, 155, 135, 86)),
        };
            #endregion

            #region DefaultDark palette
            m_defaultDarkPalette = new Brush[]
            {
                new SolidColorBrush(Color.FromArgb(100,128,158,79)),
                new SolidColorBrush(Color.FromArgb(100,69,38,79)),
                new SolidColorBrush(Color.FromArgb(100,214,128,46)),
                new SolidColorBrush(Color.FromArgb(100,51,162,192)),
                new SolidColorBrush(Color.FromArgb(100,168,58,54)),
                new SolidColorBrush(Color.FromArgb(100,43,94,171)),
                new SolidColorBrush(Color.FromArgb(100,128,158,79)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x6C, 0x15, 0x76))
            };
            #endregion

            #region EarthTone palette
            m_earthTonePalette = new Brush[] 
      {
        new SolidColorBrush(Color.FromArgb(142, 255, 0, 0)), 
        new SolidColorBrush(Color.FromArgb(142, 0, 255, 0)), 
        new SolidColorBrush(Color.FromArgb(142, 0, 0, 255)), 
        new SolidColorBrush(Color.FromArgb(142, 255, 255, 0)), 
        new SolidColorBrush(Color.FromArgb(142, 0, 255, 255)),
        new SolidColorBrush(Color.FromArgb(142, 255, 0, 255)), 
        new SolidColorBrush(Color.FromArgb(142, 170, 120, 20)), 
        new SolidColorBrush(Color.FromArgb(70, 255, 0, 0)), 
        new SolidColorBrush(Color.FromArgb(70, 0, 255, 0)), 
        new SolidColorBrush(Color.FromArgb(70, 0, 0, 255)), 
        new SolidColorBrush(Color.FromArgb(70, 255, 255, 0)), 
        new SolidColorBrush(Color.FromArgb(70, 0, 255, 255)), 
        new SolidColorBrush(Color.FromArgb(70, 255, 0, 255)), 
        new SolidColorBrush(Color.FromArgb(70, 170, 120, 20)), 
        new SolidColorBrush(Color.FromArgb(132, 100, 120, 50)), 
        new SolidColorBrush(Color.FromArgb(132, 40, 80, 150))
      };
            #endregion

            #region Analog palette
            m_analogPalette = new Brush[] 
      {
          new SolidColorBrush(Color.FromArgb(255, 0, 134, 137)),
          new SolidColorBrush(Color.FromArgb(255, 32, 55, 189)),
          new SolidColorBrush(Color.FromArgb(255, 47, 166, 208)),
          new SolidColorBrush(Color.FromArgb(255, 102, 0, 102)),
          new SolidColorBrush(Color.FromArgb(255, 204, 255, 255)),
          new SolidColorBrush(Color.FromArgb(255, 255, 128, 128)),
          new SolidColorBrush(Color.FromArgb(255, 0, 102, 204)),
          new SolidColorBrush(Color.FromArgb(255, 204, 204, 255)),
          new SolidColorBrush(Color.FromArgb(255, 0, 128, 128)),
          new SolidColorBrush(Color.FromArgb(255, 255, 117, 186)),
          new SolidColorBrush(Color.FromArgb(255, 255, 255, 153)),
          new SolidColorBrush(Color.FromArgb(255, 3, 198, 198)),
          new SolidColorBrush(Color.FromArgb(255, 128, 0, 128)),
          new SolidColorBrush(Color.FromArgb(255, 128, 0, 0)),
          new SolidColorBrush(Color.FromArgb(255, 0, 128, 255)),
          new SolidColorBrush(Color.FromArgb(255, 184, 2, 184))
        };
            #endregion

            #region Colorful palette
            m_colorfulPalette = new Brush[]
      {
new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)), 
new SolidColorBrush(Color.FromArgb(255, 251, 59, 153)), 
new SolidColorBrush(Color.FromArgb(255, 0, 255, 255)), 
new SolidColorBrush(Color.FromArgb(255, 0, 128, 255)),
new SolidColorBrush(Color.FromArgb(255, 255, 0, 128)),
new SolidColorBrush(Color.FromArgb(255, 255, 255, 122)),
new SolidColorBrush(Color.FromArgb(255, 128, 0, 255)),
new SolidColorBrush(Color.FromArgb(255, 0, 255, 128)),
new SolidColorBrush(Color.FromArgb(255, 218, 2, 2)),
new SolidColorBrush(Color.FromArgb(255, 255, 255, 61)),
new SolidColorBrush(Color.FromArgb(255, 122, 122, 255)),
new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),

new SolidColorBrush(Color.FromArgb(255, 255, 255, 61)),
new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
new SolidColorBrush(Color.FromArgb(255, 0, 224, 224)),
new SolidColorBrush(Color.FromArgb(255, 1, 70, 175))
        };
            #endregion

            #region Nature palette
            m_naturePalette = new Brush[]
{
new SolidColorBrush(Color.FromArgb(255, 119, 149, 17)),
new SolidColorBrush(Color.FromArgb(255, 119, 17, 119)),
new SolidColorBrush(Color.FromArgb(255, 17, 99, 180)),
new SolidColorBrush(Color.FromArgb(255, 241, 129, 17)),

new SolidColorBrush(Color.FromArgb(255, 241, 223, 17)),
new SolidColorBrush(Color.FromArgb(255, 66, 153, 42)),
new SolidColorBrush(Color.FromArgb(255, 17, 68, 119)),
new SolidColorBrush(Color.FromArgb(255, 119, 17, 17)),

new SolidColorBrush(Color.FromArgb(255, 68, 119, 17)),
new SolidColorBrush(Color.FromArgb(255, 17, 17, 119)),
new SolidColorBrush(Color.FromArgb(255, 119, 17, 68)),
new SolidColorBrush(Color.FromArgb(255, 224, 86, 19)),

new SolidColorBrush(Color.FromArgb(255, 236, 191, 12)),
new SolidColorBrush(Color.FromArgb(255, 95, 172, 18)),
new SolidColorBrush(Color.FromArgb(255, 55, 130, 205)),
new SolidColorBrush(Color.FromArgb(255, 1, 1, 105))
};
            #endregion

            #region Pastel palette
            m_pastelPalette = new Brush[]
{
new SolidColorBrush(Color.FromArgb(255, 163, 163, 245)),
new SolidColorBrush(Color.FromArgb(255, 163, 245, 163)),
new SolidColorBrush(Color.FromArgb(255, 239, 173, 108)),
new SolidColorBrush(Color.FromArgb(255, 53, 142, 232)),

new SolidColorBrush(Color.FromArgb(255, 53, 67, 232)),
new SolidColorBrush(Color.FromArgb(255, 158, 142, 198)),
new SolidColorBrush(Color.FromArgb(255, 245, 204, 163)),
new SolidColorBrush(Color.FromArgb(255, 163, 245, 245)),

new SolidColorBrush(Color.FromArgb(255, 204, 163, 245)),
new SolidColorBrush(Color.FromArgb(255, 245, 245, 163)),
new SolidColorBrush(Color.FromArgb(255, 163, 245, 204)),
new SolidColorBrush(Color.FromArgb(255, 68, 178, 232)),

new SolidColorBrush(Color.FromArgb(255, 53, 97, 232)),
new SolidColorBrush(Color.FromArgb(255, 245, 245, 163)),
new SolidColorBrush(Color.FromArgb(255, 201, 151, 118)),
new SolidColorBrush(Color.FromArgb(255, 176, 115, 238))
};
            #endregion

            #region Triad palette
            m_triadPalette = new Brush[]
{
new SolidColorBrush(Color.FromArgb(255, 190, 0, 255)),
new SolidColorBrush(Color.FromArgb(255, 0, 255, 190)),
new SolidColorBrush(Color.FromArgb(255, 255, 190, 0)),
new SolidColorBrush(Color.FromArgb(255, 206, 255, 61)),

new SolidColorBrush(Color.FromArgb(255, 155, 122, 255)),
new SolidColorBrush(Color.FromArgb(255, 88, 4, 116)),
new SolidColorBrush(Color.FromArgb(255, 0, 255, 127)),
new SolidColorBrush(Color.FromArgb(255, 255, 127, 0)),

new SolidColorBrush(Color.FromArgb(255, 254, 255, 61)),
new SolidColorBrush(Color.FromArgb(255, 122, 122, 255)),
new SolidColorBrush(Color.FromArgb(255, 0, 0, 102)),
new SolidColorBrush(Color.FromArgb(255, 168, 168, 252)),

new SolidColorBrush(Color.FromArgb(255, 0, 255, 204)),
new SolidColorBrush(Color.FromArgb(255, 255, 204, 0)),
new SolidColorBrush(Color.FromArgb(255, 254, 255, 120)),
new SolidColorBrush(Color.FromArgb(255, 199, 199, 255))
};
            #endregion

            #region WarmCold palette
            m_warmColdPalette = new Brush[]
{
new SolidColorBrush(Color.FromArgb(255, 7, 89, 166)),
new SolidColorBrush(Color.FromArgb(255, 255, 225, 0)),
new SolidColorBrush(Color.FromArgb(255, 255, 72, 0)),
new SolidColorBrush(Color.FromArgb(255, 255, 213, 61)),

new SolidColorBrush(Color.FromArgb(255, 122, 151, 255)),
new SolidColorBrush(Color.FromArgb(255, 35, 4, 116)),
new SolidColorBrush(Color.FromArgb(255, 255, 141, 0)),
new SolidColorBrush(Color.FromArgb(255, 255, 9, 0)),

new SolidColorBrush(Color.FromArgb(255, 255, 163, 61)),
new SolidColorBrush(Color.FromArgb(255, 122, 184, 255)),
new SolidColorBrush(Color.FromArgb(255, 0, 47, 102)),
new SolidColorBrush(Color.FromArgb(255, 168, 207, 252)),

new SolidColorBrush(Color.FromArgb(255, 255, 234, 0)),
new SolidColorBrush(Color.FromArgb(255, 255, 86, 0)),
new SolidColorBrush(Color.FromArgb(255, 255, 192, 120)),
new SolidColorBrush(Color.FromArgb(255, 199, 225, 255))
};
            #endregion

            #region GrayScale palette
            m_grayScalePalette = new Brush[]
{
new SolidColorBrush(Color.FromArgb(255, 204, 204, 204)),
new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)),
new SolidColorBrush(Color.FromArgb(255, 238, 238, 238)),
new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),

new SolidColorBrush(Color.FromArgb(255, 68, 68, 68)),
new SolidColorBrush(Color.FromArgb(255, 85, 85, 85)),
new SolidColorBrush(Color.FromArgb(255, 102, 102, 102)),

new SolidColorBrush(Color.FromArgb(255, 119, 119, 119)),
new SolidColorBrush(Color.FromArgb(255, 136, 136, 136)),
new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)),

new SolidColorBrush(Color.FromArgb(255, 170, 170, 170)),
new SolidColorBrush(Color.FromArgb(255, 187, 187, 187)),
new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
new SolidColorBrush(Color.FromArgb(255, 17, 17, 17)),
new SolidColorBrush(Color.FromArgb(255, 34, 34, 34)),
new SolidColorBrush(Color.FromArgb(255, 51, 51, 51))
};
            #endregion

            #region Palette1
            m_Palette1 = new Brush[]
{
        new SolidColorBrush(Color.FromArgb(255, 147, 147, 147)),
         new SolidColorBrush(Color.FromArgb(255, 170, 170, 170)),
          new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
          new SolidColorBrush(Color.FromArgb(255, 154, 154, 154)),
          new SolidColorBrush(Color.FromArgb(255, 193, 193, 193)),
          new SolidColorBrush(Color.FromArgb(255, 131, 131, 131)),
          new SolidColorBrush(Color.FromArgb(255, 70, 70, 70)),
          new SolidColorBrush(Color.FromArgb(255, 114, 114, 114)),
          new SolidColorBrush(Color.FromArgb(255, 158, 158, 158)),
          new SolidColorBrush(Color.FromArgb(255, 129, 129, 129))
        
};
            #endregion

            #region Palette2
            m_Palette2 = new Brush[]
{  

                new SolidColorBrush(Color.FromArgb(255, 51, 86, 127)),
              new SolidColorBrush(Color.FromArgb(255, 58, 97, 144)),
               new SolidColorBrush(Color.FromArgb(255, 65, 108, 159)),
               new SolidColorBrush(Color.FromArgb(255, 71, 116, 171)),
               new SolidColorBrush(Color.FromArgb(255, 76, 125, 183)),
               new SolidColorBrush(Color.FromArgb(255, 102, 141, 194)),
                 new SolidColorBrush(Color.FromArgb(255, 138, 163, 204)),
                 new SolidColorBrush(Color.FromArgb(255, 163, 181, 212)),
                 new SolidColorBrush(Color.FromArgb(255, 185, 198, 221)),
               new SolidColorBrush(Color.FromArgb(255, 204, 213, 230)) 
            
};
            #endregion

            #region Palette3
            m_Palette3 = new Brush[]
{
       new SolidColorBrush(Color.FromArgb(255, 130, 51, 49)),
          new SolidColorBrush(Color.FromArgb(255, 147, 59, 57)),
          new SolidColorBrush(Color.FromArgb(255, 161, 66, 63)),
          new SolidColorBrush(Color.FromArgb(255, 174, 72, 69)),
          new SolidColorBrush(Color.FromArgb(255, 186,77, 74)),
          new SolidColorBrush(Color.FromArgb(255, 197, 103, 101)),
          new SolidColorBrush(Color.FromArgb(255, 206, 138, 137)),
          new SolidColorBrush(Color.FromArgb(255, 214, 163, 162)),
          new SolidColorBrush(Color.FromArgb(255, 223, 185, 184)),
          new SolidColorBrush(Color.FromArgb(255, 231, 204, 204))


        
};
            #endregion

            #region Palette4
            m_Palette4 = new Brush[]
{
      new SolidColorBrush(Color.FromArgb(255, 104, 126, 58)),
          new SolidColorBrush(Color.FromArgb(255, 118, 143, 66)),
          new SolidColorBrush(Color.FromArgb(255, 130, 157, 74)),
          new SolidColorBrush(Color.FromArgb(255, 140, 169, 80)),
          new SolidColorBrush(Color.FromArgb(255, 150,181, 86)),
          new SolidColorBrush(Color.FromArgb(255, 163, 192, 109)),
          new SolidColorBrush(Color.FromArgb(255, 180, 202,142)),
          new SolidColorBrush(Color.FromArgb(255, 193, 211, 166)),
          new SolidColorBrush(Color.FromArgb(255, 207, 220, 187)),
          new SolidColorBrush(Color.FromArgb(255, 219, 229, 205))
        
};
            #endregion

            #region Palette5
            m_Palette5 = new Brush[]
{
       new SolidColorBrush(Color.FromArgb(255, 85, 65, 109)),
          new SolidColorBrush(Color.FromArgb(255, 97, 75, 123)),
          new SolidColorBrush(Color.FromArgb(255, 107, 83, 136)),
          new SolidColorBrush(Color.FromArgb(255, 115, 90, 146)),
          new SolidColorBrush(Color.FromArgb(255, 124,97, 157)),
          new SolidColorBrush(Color.FromArgb(255, 140, 118, 170)),
          new SolidColorBrush(Color.FromArgb(255, 163, 147,185)),
          new SolidColorBrush(Color.FromArgb(255, 180, 169, 197)),
          new SolidColorBrush(Color.FromArgb(255, 197, 189, 210)),
          new SolidColorBrush(Color.FromArgb(255, 213, 207, 221))
        
};
            #endregion

            #region Palette6
            m_Palette6 = new Brush[]
{
         new SolidColorBrush(Color.FromArgb(255, 48, 116, 134)),
          new SolidColorBrush(Color.FromArgb(255, 55, 131, 151)),
          new SolidColorBrush(Color.FromArgb(255, 62, 144, 167)),
          new SolidColorBrush(Color.FromArgb(255, 67, 155, 179)),
          new SolidColorBrush(Color.FromArgb(255, 72,166, 192)),
          new SolidColorBrush(Color.FromArgb(255, 100, 178, 202)),
          new SolidColorBrush(Color.FromArgb(255, 136, 192,210)),
          new SolidColorBrush(Color.FromArgb(255, 161, 203, 218)),
          new SolidColorBrush(Color.FromArgb(255, 184, 214, 225)),
          new SolidColorBrush(Color.FromArgb(255, 203, 224, 233))
        
};
            #endregion

            #region Palette7
            m_Palette7 = new Brush[]
{
       new SolidColorBrush(Color.FromArgb(255, 168, 100, 45)),
          new SolidColorBrush(Color.FromArgb(255, 189, 114, 51)),
          new SolidColorBrush(Color.FromArgb(255, 208, 126, 58)),
          new SolidColorBrush(Color.FromArgb(255, 224, 135, 63)),
          new SolidColorBrush(Color.FromArgb(255, 239,145, 67)),
          new SolidColorBrush(Color.FromArgb(255, 247, 159, 96)),
          new SolidColorBrush(Color.FromArgb(255, 249, 177,134)),
          new SolidColorBrush(Color.FromArgb(255, 250, 191, 160)),
          new SolidColorBrush(Color.FromArgb(255, 251, 205, 183)),
          new SolidColorBrush(Color.FromArgb(255, 252, 218, 203))

        
};
            #endregion

            #region Palette8
            m_Palette8 = new Brush[]
{
     new SolidColorBrush(Color.FromArgb(255, 79, 129, 189)),
     new SolidColorBrush(Color.FromArgb(255, 170, 70, 67)),
     new SolidColorBrush(Color.FromArgb(255, 137, 165, 78)),
     new SolidColorBrush(Color.FromArgb(255, 113, 88, 143)),
     new SolidColorBrush(Color.FromArgb(255, 65, 152, 175)),
     new SolidColorBrush(Color.FromArgb(255, 219, 132, 61)),
        new SolidColorBrush(Color.FromArgb(255, 147, 169, 207)),
     new SolidColorBrush(Color.FromArgb(255, 209, 147, 146)),
     new SolidColorBrush(Color.FromArgb(255, 185, 205, 150)),     

     new SolidColorBrush(Color.FromArgb(255, 169, 155, 189)),
        
         
         
};
            #endregion

            #region MetroTheme
            m_metroThemePalette = new Brush[]
            {
                new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0x14, 0x00)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0xC1, 0x39)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0x00, 0x73)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x99, 0x33)),               
                new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0x96, 0x09)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0x71, 0xB8)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x00, 0xFF)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAB, 0xA9)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0xA0, 0x50, 0x00))

            };
            #endregion

        }

        /// <summary>
        /// Called when instance created for ChartstyleModel with one Arguments
        /// </summary>
        /// <param name="area"></param>
        public ChartStyleModel(ChartArea area)
        {
            m_area = area;
        }
        /// <summary>
        /// Empty constructor for ChartStyleModel
        /// </summary>
        public ChartStyleModel()
        {
        }
        #endregion

        /// <summary>
        /// Gets the brushes.
        /// </summary>
        /// <param name="palette">The palette.</param>
        /// <returns>Returns the brushes</returns>
        internal Brush[] GetBrushes(ChartColorPalette palette)
        {
            Brush[] brushes = null;

            switch (palette)
            {
                case ChartColorPalette.Default:
                    brushes = m_defaultPalette;
                    break;

                case ChartColorPalette.DefaultAlpha:
                    brushes = m_defaultAlphaPalette;
                    break;

                case ChartColorPalette.DefaultDark:
                    brushes = m_defaultDarkPalette;
                    break;

                case ChartColorPalette.EarthTone:
                    brushes = m_earthTonePalette;
                    break;

                case ChartColorPalette.Analog:
                    brushes = m_analogPalette;
                    break;

                case ChartColorPalette.Colorful:
                    brushes = m_colorfulPalette;
                    break;

                case ChartColorPalette.Nature:
                    brushes = m_naturePalette;
                    break;

                case ChartColorPalette.Pastel:
                    brushes = m_pastelPalette;
                    break;

                case ChartColorPalette.Triad:
                    brushes = m_triadPalette;
                    break;

                case ChartColorPalette.WarmCold:
                    brushes = m_warmColdPalette;
                    break;

                case ChartColorPalette.Grayscale:
                    brushes = m_grayScalePalette;
                    break;

                case ChartColorPalette.Metro:
                    brushes = m_metroThemePalette;
                    break;

                case ChartColorPalette.Custom:
                    brushes = m_customPalette;
                    break;

                case ChartColorPalette.Palette1:
                    brushes = m_Palette1;
                    break;

                case ChartColorPalette.Palette2:
                    brushes = m_Palette2;
                    break;

                case ChartColorPalette.Palette3:
                    brushes = m_Palette3;
                    break;

                case ChartColorPalette.Palette4:
                    brushes = m_Palette4;
                    break;

                case ChartColorPalette.Palette5:
                    brushes = m_Palette5;
                    break;

                case ChartColorPalette.Palette6:
                    brushes = m_Palette6;
                    break;

                case ChartColorPalette.Palette7:
                    brushes = m_Palette7;
                    break;

                case ChartColorPalette.Palette8:
                    brushes = m_Palette8;
                    break;
            }

            return brushes;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_area != null)
            {
                m_area = null;
            }

            if (m_brushes != null)
            {
                m_brushes = null;
            }
            CustomPalette = null;
            //m_analogPalette = null;
            m_brushes = null;
            //m_colorfulPalette = null;
            //m_customPalette = null;

            //m_defaultAlphaPalette = null;
            //m_defaultPalette = null;
            //m_earthTonePalette = null;
            //m_grayScalePalette = null;
            //m_naturePalette = null;
            //m_pastelPalette = null;
            //m_triadPalette = null;
            //m_warmColdPalette = null;   
        }

        #endregion
    }
}
