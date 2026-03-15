// <copyright file="ChartStyleModel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Resources;

    
    /// <summary>
    /// The class creates an extension method on the actual enum "ChartColorPalette" which then allows
    /// you to call a ToFriendlyString() on the instance of all enums of that type
    /// Which could be used for localization
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class LocalizeEnumChartColorPalette
    {
        /// <summary>
        /// Returns a friendly enum name, used for localization
        /// </summary>
        /// <param name="chartColorPaletteEnum">The chart color palette enum.</param>
        /// <returns>Returns string from ResourceDictionary</returns>
        public static string ToFriendlyString(this ChartColorPalette chartColorPaletteEnum)
        {
            //ResourceDictionary resourceDictionaryItems = new ResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/LangDictionary.xaml", UriKind.RelativeOrAbsolute)
            //};

            ChartResourceWrapper wrapper = new ChartResourceWrapper();

            //string str = resourceDictionaryItems[chartColorPaletteEnum.ToString()] as string;
            object str = ChartDataUtils.GetPropertyDescriptor(wrapper, chartColorPaletteEnum.ToString());

            if (str != null)
            {
                return str.ToString();
            }
            else
            {
                return chartColorPaletteEnum.ToString();
            }
        }
    }

    /// <summary>
    /// Represents ChartStyleModel
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartStyleModel : INotifyPropertyChanged, IDisposable
    {
        #region Helper classes
        /// <summary>
        /// Represents StyleBindingObject
        /// </summary>
        private class StyleBindingObject : DependencyObject, IDisposable
        {
            #region DependencyProperties
            /// <summary>
            /// Identifies the <see cref="Interior"/> property.
            /// </summary>
            public static readonly DependencyProperty InteriorProperty = DependencyProperty.Register("Interior", typeof(Brush), typeof(StyleBindingObject));
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the Interior property
            /// </summary>
            public Brush Interior
            {
                get
                {
                    return GetValue(StyleBindingObject.InteriorProperty) as Brush;
                }

                set
                {
                    SetValue(StyleBindingObject.InteriorProperty, value);
                }
            }

            /// <summary>
            /// Gets the InteriorBinding
            /// </summary>
            public Binding InteriorBinding
            {
                get
                {
                    return m_binding;
                }
            }
            #endregion

            #region Members
            /// <summary>
            /// Declares m_binding
            /// </summary>
            private Binding m_binding = new Binding();
            #endregion

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ChartStyleModel.StyleBindingObject">StyleBindingObject</see> class. 
            /// </summary>
            /// <param name="brush">The brush value</param>
            /// <remarks></remarks>
            public StyleBindingObject(Brush brush)
            {
                m_binding.Source = this;
                m_binding.Path = new PropertyPath(StyleBindingObject.InteriorProperty);

                Interior = brush;
            }
            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this.ClearValue(StyleBindingObject.InteriorProperty);
                this.Interior = null;
                m_binding = null;
                BindingOperations.ClearAllBindings(this);
            }

            #endregion
        }
        #endregion

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
        /// Initializes m_colorfulPalette
        /// </summary>
        private static readonly Brush[] m_GradientPalette;

        /// <summary>
        /// Initializes m_naturePalette
        /// </summary>
        private static readonly Brush[] m_naturePalette;

        private static readonly Brush[] m_MixedGray;

        private static readonly Brush[] m_BlueScale;

        private static readonly Brush[] m_MaroonRed;

        private static readonly Brush[] m_GreenScale;

        private static readonly Brush[] m_MixedViolet;

        private static readonly Brush[] m_CoolBlueScale;

        private static readonly Brush[] m_ChocolateOrange;

        private static readonly Brush[] m_MixedFantasy;

        private static readonly Brush[] m_metroTheme;

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

        /// <summary>
        /// Initializes m_Office2007BluePalette
        /// </summary>
        private static readonly Brush[] m_Office2007BluePalette;

        /// <summary>
        /// Initializes m_Office2007BlackPalette
        /// </summary>
        private static readonly Brush[] m_Office2007BlackPalette;

        /// <summary>
        /// Initializes m_Office2007SilverPalette
        /// </summary>
        private static readonly Brush[] m_Office2007SilverPalette;

        /// <summary>
        /// Initializes m_area
        /// </summary>
        internal ChartArea m_area;

        /// <summary>
        /// Initializes m_brushObjects
        /// </summary>
        private StyleBindingObject[] m_brushObjects;

        /// <summary>
        /// Initializes m_palette
        /// </summary>
        private ChartColorPalette m_palette = ChartColorPalette.Default;

        /// <summary>
        /// Initializes m_brushes
        /// </summary>
        internal Brush[] m_brushes = m_defaultPalette;


        /// <summary>
        /// Initializes m_customPalette
        /// </summary>
        private Brush[] m_customPalette;



        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        /// <value>The palette.</value>
        public ChartColorPalette Palette
        {
            get
            {
                return m_palette;
            }

            set
            {
                m_palette = value;
                if (value != ChartColorPalette.Custom || (value == ChartColorPalette.Custom && CustomPalette != null))
                {
                    ApplyPalette(m_palette);
                }
                paletteChanged = true;
            }
        }
        internal bool paletteChanged = false;
        /// <summary>
        /// Gets or sets the custom palette.
        /// </summary>
        /// <value>The custom palette.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Brush[] CustomPalette
        {
            get
            {
                return m_customPalette;
            }

            set
            {
                if (m_customPalette != value)
                {
                    m_customPalette = value;

                    if (m_palette == ChartColorPalette.Custom)
                    {
                        ApplyPalette(m_palette);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the CurrentPalette
        /// </summary>
        public Brush[] CurrentPalette
        {
            get
            {
                return m_brushes;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartStyleModel"/> class.
        /// </summary>
        static ChartStyleModel()
        {
            ResourceDictionary rd1 = ChartDictionaries.ChartPaletteBrushesDictionary;
            //ResourceDictionary   rd1 = new SharedResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
            //};
            #region Default palette
            GradientStopCollection coll = new GradientStopCollection();
            coll.Add(new GradientStop(Colors.White, 1));
            GradientStopCollection coll1 = new GradientStopCollection() { new GradientStop(Colors.White, 1), new GradientStop(Colors.Yellow, 0) };
            m_defaultPalette = new Brush[]
      {
    
        rd1["DefaultPaletteBrush1"] as LinearGradientBrush , 
       rd1["DefaultPaletteBrush2"] as LinearGradientBrush , 
       rd1["DefaultPaletteBrush3"] as LinearGradientBrush , 
      rd1["DefaultPaletteBrush4"] as LinearGradientBrush , 
      rd1["DefaultPaletteBrush5"] as LinearGradientBrush ,
         rd1["DefaultPaletteBrush6"] as LinearGradientBrush ,
        rd1["DefaultPaletteBrush7"] as LinearGradientBrush ,
         rd1["DefaultPaletteBrush8"] as LinearGradientBrush ,
          rd1["DefaultPaletteBrush9"] as LinearGradientBrush ,
         rd1["DefaultPaletteBrush10"] as LinearGradientBrush ,
      };
            #endregion

            #region DefaultAlpha palette
            m_defaultAlphaPalette = new Brush[]
      {
          //new SolidColorBrush(Color.FromRgb(169, 155, 189)),
          //new SolidColorBrush(Color.FromRgb(185, 205, 150)),
          //new SolidColorBrush(Color.FromRgb(209, 147, 146)),
          //new SolidColorBrush(Color.FromRgb(147, 169, 207)),
          //new SolidColorBrush(Color.FromRgb(219, 132, 61)),
          //new SolidColorBrush(Color.FromRgb(65, 152, 175)),
          //new SolidColorBrush(Color.FromRgb(113, 88, 143)),
          //new SolidColorBrush(Color.FromRgb(137, 165, 78)),
          //new SolidColorBrush(Color.FromRgb(170, 70, 67)),
          //new SolidColorBrush(Color.FromRgb(231, 231, 231))
          
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 169, 185, 205), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 169, 185, 205), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 227, 145, 96), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 227, 145, 96), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 237, 196, 114), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 237, 196, 114), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 94, 158, 199), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 94, 158, 199), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 196, 99, 82), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 196, 99, 82), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 143, 167, 69), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 143, 167, 69), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 53, 83, 107), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 53, 83, 107), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 225, 216, 132), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 225, 216, 132), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 136, 157, 159), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 136, 157, 159), 0)},new Point(1,0.5),new Point(0,0.5)),      
         //new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Color.FromArgb(150, 155, 135, 86), 1), new GradientStop(Colors.White, 0.5), new GradientStop(Color.FromArgb(150, 155, 135, 86), 0)},new Point(1,0.5),new Point(0,0.5)),      

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
                new SolidColorBrush(Color.FromRgb(128,158,79)),
                new SolidColorBrush(Color.FromRgb(69,38,79)),
                new SolidColorBrush(Color.FromRgb(214,128,46)),
                new SolidColorBrush(Color.FromRgb(51,162,192)),
                new SolidColorBrush(Color.FromRgb(168,58,54)),
                new SolidColorBrush(Color.FromRgb(43,94,171)),
                new SolidColorBrush(Color.FromRgb(128,158,79)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x83, 0x74, 0x15)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x12, 0x6A, 0x6E)),
                new SolidColorBrush(Color.FromArgb(0xFF, 0x6C, 0x15, 0x76))
            }; 
            #endregion

            #region EarthTone palette
            m_earthTonePalette = new Brush[] 
      {

          //new SolidColorBrush(Color.FromRgb(51, 86, 127)),
          //new SolidColorBrush(Color.FromRgb(58, 97, 144)),
          // new SolidColorBrush(Color.FromRgb(65, 108, 159)),
          // new SolidColorBrush(Color.FromRgb(71, 116, 171)),
          // new SolidColorBrush(Color.FromRgb(76, 125, 183)),
          // new SolidColorBrush(Color.FromRgb(102, 141, 194)),
          //   new SolidColorBrush(Color.FromRgb(138, 163, 204)),
          //   new SolidColorBrush(Color.FromRgb(163, 181, 212)),
          //   new SolidColorBrush(Color.FromRgb(185, 198, 221)),
          // new SolidColorBrush(Color.FromRgb(204, 213, 230))           
                  

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
          //new SolidColorBrush(Color.FromRgb(130, 51, 49)),
          //new SolidColorBrush(Color.FromRgb(147, 59, 57)),
          //new SolidColorBrush(Color.FromRgb(161, 66, 63)),
          //new SolidColorBrush(Color.FromRgb(174, 72, 69)),
          //new SolidColorBrush(Color.FromRgb(186,77, 74)),
          //new SolidColorBrush(Color.FromRgb(197, 103, 101)),
          //new SolidColorBrush(Color.FromRgb(206, 138, 137)),
          //new SolidColorBrush(Color.FromRgb(214, 163, 162)),
          //new SolidColorBrush(Color.FromRgb(223, 185, 184)),
          //new SolidColorBrush(Color.FromRgb(231, 204, 204))


           new SolidColorBrush(Color.FromRgb(184, 2, 184)),
           new SolidColorBrush(Color.FromRgb(0, 128, 255)),
           new SolidColorBrush(Color.FromRgb(128, 0, 0)),
           new SolidColorBrush(Color.FromRgb(128, 0, 128)),
           new SolidColorBrush(Color.FromRgb(3, 198, 198)),
           new SolidColorBrush(Color.FromRgb(255, 255, 153)),
           new SolidColorBrush(Color.FromRgb(255, 117, 186)),
           new SolidColorBrush(Color.FromRgb(0, 128, 128)),
           new SolidColorBrush(Color.FromRgb(204, 204, 255)),
           new SolidColorBrush(Color.FromRgb(0, 102, 204)),
           new SolidColorBrush(Color.FromRgb(255, 128, 128)),
           new SolidColorBrush(Color.FromRgb(204, 255, 255)),
           new SolidColorBrush(Color.FromRgb(102, 0, 102)),
           new SolidColorBrush(Color.FromRgb(47, 166, 208)),
           new SolidColorBrush(Color.FromRgb(32, 55, 189)),
           new SolidColorBrush(Color.FromRgb(0, 134, 137))         
         
        };
            #endregion

            #region Colorful palette
            m_colorfulPalette = new Brush[]
      {
          			
          //new SolidColorBrush(Color.FromRgb(104, 126, 58)),
          //new SolidColorBrush(Color.FromRgb(118, 143, 66)),
          //new SolidColorBrush(Color.FromRgb(130, 157, 74)),
          //new SolidColorBrush(Color.FromRgb(140, 169, 80)),
          //new SolidColorBrush(Color.FromRgb(150,181, 86)),
          //new SolidColorBrush(Color.FromRgb(163, 192, 109)),
          //new SolidColorBrush(Color.FromRgb(180, 202,142)),
          //new SolidColorBrush(Color.FromRgb(193, 211, 166)),
          //new SolidColorBrush(Color.FromRgb(207, 220, 187)),
          //new SolidColorBrush(Color.FromRgb(219, 229, 205))

            new SolidColorBrush(Color.FromRgb(0, 0, 255)), 
            new SolidColorBrush(Color.FromRgb(251, 59, 153)), 
            new SolidColorBrush(Color.FromRgb(0, 255, 255)), 
            new SolidColorBrush(Color.FromRgb(0, 128, 255)),
            new SolidColorBrush(Color.FromRgb(255, 0, 128)),
            new SolidColorBrush(Color.FromRgb(255, 255, 122)),
            new SolidColorBrush(Color.FromRgb(128, 0, 255)),
            new SolidColorBrush(Color.FromRgb(0, 255, 128)),
            new SolidColorBrush(Color.FromRgb(218, 2, 2)),
            new SolidColorBrush(Color.FromRgb(255, 255, 61)),
            new SolidColorBrush(Color.FromRgb(122, 122, 255)),
            new SolidColorBrush(Color.FromRgb(0, 255, 0)),

            new SolidColorBrush(Color.FromRgb(255, 255, 61)),
            new SolidColorBrush(Color.FromRgb(255, 0, 0)),
            new SolidColorBrush(Color.FromRgb(0, 224, 224)),
            new SolidColorBrush(Color.FromRgb(1, 70, 175))
        };
            #endregion

            #region Nature palette
            m_naturePalette = new Brush[]
{

          //new SolidColorBrush(Color.FromRgb(85, 65, 109)),
          //new SolidColorBrush(Color.FromRgb(97, 75, 123)),
          //new SolidColorBrush(Color.FromRgb(107, 83, 136)),
          //new SolidColorBrush(Color.FromRgb(115, 90, 146)),
          //new SolidColorBrush(Color.FromRgb(124,97, 157)),
          //new SolidColorBrush(Color.FromRgb(140, 118, 170)),
          //new SolidColorBrush(Color.FromRgb(163, 147,185)),
          //new SolidColorBrush(Color.FromRgb(180, 169, 197)),
          //new SolidColorBrush(Color.FromRgb(197, 189, 210)),
          //new SolidColorBrush(Color.FromRgb(213, 207, 221))

            new SolidColorBrush(Color.FromRgb(119, 149, 17)),
            new SolidColorBrush(Color.FromRgb(119, 17, 119)),
            new SolidColorBrush(Color.FromRgb(17, 99, 180)),
            new SolidColorBrush(Color.FromRgb(241, 129, 17)),

            new SolidColorBrush(Color.FromRgb(241, 223, 17)),
            new SolidColorBrush(Color.FromRgb(66, 153, 42)),
            new SolidColorBrush(Color.FromRgb(17, 68, 119)),
            new SolidColorBrush(Color.FromRgb(119, 17, 17)),

            new SolidColorBrush(Color.FromRgb(68, 119, 17)),
            new SolidColorBrush(Color.FromRgb(17, 17, 119)),
            new SolidColorBrush(Color.FromRgb(119, 17, 68)),
            new SolidColorBrush(Color.FromRgb(224, 86, 19)),

            new SolidColorBrush(Color.FromRgb(236, 191, 12)),
            new SolidColorBrush(Color.FromRgb(95, 172, 18)),
            new SolidColorBrush(Color.FromRgb(55, 130, 205)),
            new SolidColorBrush(Color.FromRgb(1, 1, 105))
};
            #endregion

            #region Pastel palette
            m_pastelPalette = new Brush[]
{
          //new SolidColorBrush(Color.FromRgb(48, 116, 134)),
          //new SolidColorBrush(Color.FromRgb(55, 131, 151)),
          //new SolidColorBrush(Color.FromRgb(62, 144, 167)),
          //new SolidColorBrush(Color.FromRgb(67, 155, 179)),
          //new SolidColorBrush(Color.FromRgb(72,166, 192)),
          //new SolidColorBrush(Color.FromRgb(100, 178, 202)),
          //new SolidColorBrush(Color.FromRgb(136, 192,210)),
          //new SolidColorBrush(Color.FromRgb(161, 203, 218)),
          //new SolidColorBrush(Color.FromRgb(184, 214, 225)),
          //new SolidColorBrush(Color.FromRgb(203, 224, 233))


            new SolidColorBrush(Color.FromRgb(163, 163, 245)),
            new SolidColorBrush(Color.FromRgb(163, 245, 163)),
            new SolidColorBrush(Color.FromRgb(239, 173, 108)),
            new SolidColorBrush(Color.FromRgb(53, 142, 232)),

            new SolidColorBrush(Color.FromRgb(53, 67, 232)),
            new SolidColorBrush(Color.FromRgb(158, 142, 198)),
            new SolidColorBrush(Color.FromRgb(245, 204, 163)),
            new SolidColorBrush(Color.FromRgb(163, 245, 245)),

            new SolidColorBrush(Color.FromRgb(204, 163, 245)),
            new SolidColorBrush(Color.FromRgb(245, 245, 163)),
            new SolidColorBrush(Color.FromRgb(163, 245, 204)),
            new SolidColorBrush(Color.FromRgb(68, 178, 232)),

            new SolidColorBrush(Color.FromRgb(53, 97, 232)),
            new SolidColorBrush(Color.FromRgb(245, 245, 163)),
            new SolidColorBrush(Color.FromRgb(201, 151, 118)),
            new SolidColorBrush(Color.FromRgb(176, 115, 238))
};
            #endregion

            #region Triad palette
            m_triadPalette = new Brush[]
{
          //new SolidColorBrush(Color.FromRgb(168, 100, 45)),
          //new SolidColorBrush(Color.FromRgb(189, 114, 51)),
          //new SolidColorBrush(Color.FromRgb(208, 126, 58)),
          //new SolidColorBrush(Color.FromRgb(224, 135, 63)),
          //new SolidColorBrush(Color.FromRgb(239,145, 67)),
          //new SolidColorBrush(Color.FromRgb(247, 159, 96)),
          //new SolidColorBrush(Color.FromRgb(249, 177,134)),
          //new SolidColorBrush(Color.FromRgb(250, 191, 160)),
          //new SolidColorBrush(Color.FromRgb(251, 205, 183)),
          //new SolidColorBrush(Color.FromRgb(252, 218, 203))

            new SolidColorBrush(Color.FromRgb(190, 0, 255)),
            new SolidColorBrush(Color.FromRgb(0, 255, 190)),
            new SolidColorBrush(Color.FromRgb(255, 190, 0)),
            new SolidColorBrush(Color.FromRgb(206, 255, 61)),

            new SolidColorBrush(Color.FromRgb(155, 122, 255)),
            new SolidColorBrush(Color.FromRgb(88, 4, 116)),
            new SolidColorBrush(Color.FromRgb(0, 255, 127)),
            new SolidColorBrush(Color.FromRgb(255, 127, 0)),

            new SolidColorBrush(Color.FromRgb(254, 255, 61)),
            new SolidColorBrush(Color.FromRgb(122, 122, 255)),
            new SolidColorBrush(Color.FromRgb(0, 0, 102)),
            new SolidColorBrush(Color.FromRgb(168, 168, 252)),

            new SolidColorBrush(Color.FromRgb(0, 255, 204)),
            new SolidColorBrush(Color.FromRgb(255, 204, 0)),
            new SolidColorBrush(Color.FromRgb(254, 255, 120)),
            new SolidColorBrush(Color.FromRgb(199, 199, 255))
};
            #endregion

            #region WarmCold palette
            m_warmColdPalette = new Brush[]
{
new SolidColorBrush(Color.FromRgb(7, 89, 166)),
new SolidColorBrush(Color.FromRgb(255, 225, 0)),
new SolidColorBrush(Color.FromRgb(255, 72, 0)),
new SolidColorBrush(Color.FromRgb(255, 213, 61)),

new SolidColorBrush(Color.FromRgb(122, 151, 255)),
new SolidColorBrush(Color.FromRgb(35, 4, 116)),
new SolidColorBrush(Color.FromRgb(255, 141, 0)),
new SolidColorBrush(Color.FromRgb(255, 9, 0)),

new SolidColorBrush(Color.FromRgb(255, 163, 61)),
new SolidColorBrush(Color.FromRgb(122, 184, 255)),
new SolidColorBrush(Color.FromRgb(0, 47, 102)),
new SolidColorBrush(Color.FromRgb(168, 207, 252)),

new SolidColorBrush(Color.FromRgb(255, 234, 0)),
new SolidColorBrush(Color.FromRgb(255, 86, 0)),
new SolidColorBrush(Color.FromRgb(255, 192, 120)),
new SolidColorBrush(Color.FromRgb(199, 225, 255))
};
            #endregion

            #region GrayScale palette
            m_grayScalePalette = new Brush[]
{
new SolidColorBrush(Color.FromRgb(204, 204, 204)),
new SolidColorBrush(Color.FromRgb(221, 221, 221)),
new SolidColorBrush(Color.FromRgb(238, 238, 238)),
new SolidColorBrush(Color.FromRgb(255, 255, 255)),

new SolidColorBrush(Color.FromRgb(68, 68, 68)),
new SolidColorBrush(Color.FromRgb(85, 85, 85)),
new SolidColorBrush(Color.FromRgb(102, 102, 102)),

new SolidColorBrush(Color.FromRgb(119, 119, 119)),
new SolidColorBrush(Color.FromRgb(136, 136, 136)),
new SolidColorBrush(Color.FromRgb(153, 153, 153)),

new SolidColorBrush(Color.FromRgb(170, 170, 170)),
new SolidColorBrush(Color.FromRgb(187, 187, 187)),
new SolidColorBrush(Color.FromRgb(0, 0, 0)),
new SolidColorBrush(Color.FromRgb(17, 17, 17)),
new SolidColorBrush(Color.FromRgb(34, 34, 34)),
new SolidColorBrush(Color.FromRgb(51, 51, 51))
};
            #endregion

            #region Office2007Blue palette

            m_Office2007BluePalette = new Brush[]
{
new LinearGradientBrush(Color.FromArgb(255, 9, 97, 155), Color.FromArgb(255, 28, 132, 201), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 85, 140, 177), Color.FromArgb(255, 121, 178, 215), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 44, 107, 149), Color.FromArgb(255, 108, 174, 218), new Point(0, 0), new Point(1, 1)), 
new LinearGradientBrush(Color.FromArgb(255, 146, 183, 232), Color.FromArgb(255, 191, 219, 255), new Point(0, 0), new Point(1, 1)), 
new LinearGradientBrush(Color.FromArgb(255, 99, 139, 198), Color.FromArgb(255, 148, 180, 225), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 90, 139, 202), Color.FromArgb(255, 140, 184, 240), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 83, 114, 151), Color.FromArgb(255, 132, 161, 195), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 84, 24, 151), Color.FromArgb(255, 74, 150, 238), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 0, 69, 147), Color.FromArgb(255, 0, 112, 255), new Point(0, 0), new Point(1, 1))
};

            #endregion

            #region Office2007Black palette

            m_Office2007BlackPalette = new Brush[]
{
new LinearGradientBrush(Color.FromArgb(255, 46, 48, 49), Color.FromArgb(255, 129, 133, 139), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 16, 16, 16), Color.FromArgb(255, 66, 66, 68), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 54, 54, 54), Color.FromArgb(255, 100, 100, 101), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 15, 12, 12), Color.FromArgb(255, 47, 46, 46), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 52, 55, 68), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 150, 148, 148), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 105, 105, 105), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 31, 31, 34), Color.FromArgb(255, 211, 211, 211), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 22, 22, 22), Color.FromArgb(255, 68, 68, 68), new Point(0, 0), new Point(1, 1))
};

            #endregion

            #region Office2007Silver palette

            m_Office2007SilverPalette = new Brush[]
{
new LinearGradientBrush(Color.FromArgb(255, 208, 212, 223), Color.FromArgb(255, 230, 235, 238), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 174, 178, 189), Color.FromArgb(255, 205, 209, 218), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 109, 113, 122), Color.FromArgb(255, 187, 191, 200), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 15, 12, 12), Color.FromArgb(255, 47, 46, 46), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 177, 177, 177), Color.FromArgb(255, 229, 228, 228), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 130, 131, 135), Color.FromArgb(255, 202, 203, 208), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 75, 78, 82), Color.FromArgb(255, 129, 133, 139), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 121, 125, 136), Color.FromArgb(255, 211, 211, 211), new Point(0, 0), new Point(1, 1)),
new LinearGradientBrush(Color.FromArgb(255, 99, 99, 99), Color.FromArgb(255, 197, 197, 197), new Point(0, 0), new Point(1, 1))
};

            #endregion

            #region Gradient Palette

            ResourceDictionary rd = ChartDictionaries.ChartPaletteBrushesDictionary;
            //  ResourceDictionary rd = new SharedResourceDictionary()
            //{
            //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Base.xaml", UriKind.RelativeOrAbsolute)
            //};

             LinearGradientBrush brush1 = rd["SeriesAInterior"] as LinearGradientBrush;
            LinearGradientBrush brush2 =  rd["SeriesBInterior"] as LinearGradientBrush;
            LinearGradientBrush brush3 =  rd["SeriesCInterior"] as LinearGradientBrush;
            LinearGradientBrush brush4 =  rd["Series4Interior"] as LinearGradientBrush;
            LinearGradientBrush brush5 =  rd["Series5Interior"] as LinearGradientBrush;
            LinearGradientBrush brush6 =  rd["Series6Interior"] as LinearGradientBrush;
            LinearGradientBrush brush7 =  rd["Series7Interior"] as LinearGradientBrush;
            LinearGradientBrush brush8 =  rd["Series1Interior"] as LinearGradientBrush;
            LinearGradientBrush brush9 =  rd["Series2Interior"] as LinearGradientBrush;
            LinearGradientBrush brush10 =  rd["Series3Interior"] as LinearGradientBrush;

            m_GradientPalette = new Brush[]{
                brush1,
                brush2,
                brush3,
                brush4,
                brush5,
                brush6,
                brush7,
                brush8,
                brush9,
                brush10
            };
            #endregion

            #region MixedGray
            m_MixedGray = new Brush[]
{
        new SolidColorBrush(Color.FromRgb(147, 147, 147)),
         new SolidColorBrush(Color.FromRgb(170, 170, 170)),
          new SolidColorBrush(Color.FromRgb(200, 200, 200)),
          new SolidColorBrush(Color.FromRgb(154, 154, 154)),
          new SolidColorBrush(Color.FromRgb(193, 193, 193)),
          new SolidColorBrush(Color.FromRgb(131, 131, 131)),
          new SolidColorBrush(Color.FromRgb(70, 70, 70)),
          new SolidColorBrush(Color.FromRgb(114, 114, 114)),
          new SolidColorBrush(Color.FromRgb(158, 158, 158)),
          new SolidColorBrush(Color.FromRgb(129, 129, 129))

         //     new SolidColorBrush(Color.FromRgb(79, 79, 79)),
         //new SolidColorBrush(Color.FromRgb(170, 170, 170)),
         // new SolidColorBrush(Color.FromRgb(147, 147, 147)),
         // new SolidColorBrush(Color.FromRgb(106, 106, 106)),
         // new SolidColorBrush(Color.FromRgb(122, 122, 122)),
         // new SolidColorBrush(Color.FromRgb(180, 180, 180)),
         // new SolidColorBrush(Color.FromRgb(97, 97, 97)),
         // new SolidColorBrush(Color.FromRgb(179, 179, 179)),
         // new SolidColorBrush(Color.FromRgb(129, 129, 129)),
         // new SolidColorBrush(Color.FromRgb(80, 80, 80))
        
};
            #endregion

            #region BlueScale
            m_BlueScale = new Brush[]
{  

                new SolidColorBrush(Color.FromRgb(51, 86, 127)),
              new SolidColorBrush(Color.FromRgb(58, 97, 144)),
               new SolidColorBrush(Color.FromRgb(65, 108, 159)),
               new SolidColorBrush(Color.FromRgb(71, 116, 171)),
               new SolidColorBrush(Color.FromRgb(76, 125, 183)),
               new SolidColorBrush(Color.FromRgb(102, 141, 194)),
                 new SolidColorBrush(Color.FromRgb(138, 163, 204)),
                 new SolidColorBrush(Color.FromRgb(163, 181, 212)),
                 new SolidColorBrush(Color.FromRgb(185, 198, 221)),
               new SolidColorBrush(Color.FromRgb(204, 213, 230)) 
            
};
            #endregion

            #region MaroonRed
            m_MaroonRed = new Brush[]
{
       new SolidColorBrush(Color.FromRgb(130, 51, 49)),
          new SolidColorBrush(Color.FromRgb(147, 59, 57)),
          new SolidColorBrush(Color.FromRgb(161, 66, 63)),
          new SolidColorBrush(Color.FromRgb(174, 72, 69)),
          new SolidColorBrush(Color.FromRgb(186,77, 74)),
          new SolidColorBrush(Color.FromRgb(197, 103, 101)),
          new SolidColorBrush(Color.FromRgb(206, 138, 137)),
          new SolidColorBrush(Color.FromRgb(214, 163, 162)),
          new SolidColorBrush(Color.FromRgb(223, 185, 184)),
          new SolidColorBrush(Color.FromRgb(231, 204, 204))


        
};
            #endregion

            #region GreenScale
            m_GreenScale = new Brush[]
{
      new SolidColorBrush(Color.FromRgb(104, 126, 58)),
          new SolidColorBrush(Color.FromRgb(118, 143, 66)),
          new SolidColorBrush(Color.FromRgb(130, 157, 74)),
          new SolidColorBrush(Color.FromRgb(140, 169, 80)),
          new SolidColorBrush(Color.FromRgb(150,181, 86)),
          new SolidColorBrush(Color.FromRgb(163, 192, 109)),
          new SolidColorBrush(Color.FromRgb(180, 202,142)),
          new SolidColorBrush(Color.FromRgb(193, 211, 166)),
          new SolidColorBrush(Color.FromRgb(207, 220, 187)),
          new SolidColorBrush(Color.FromRgb(219, 229, 205))
        
};
            #endregion

            #region MixedViolet
            m_MixedViolet = new Brush[]
{
       new SolidColorBrush(Color.FromRgb(85, 65, 109)),
          new SolidColorBrush(Color.FromRgb(97, 75, 123)),
          new SolidColorBrush(Color.FromRgb(107, 83, 136)),
          new SolidColorBrush(Color.FromRgb(115, 90, 146)),
          new SolidColorBrush(Color.FromRgb(124,97, 157)),
          new SolidColorBrush(Color.FromRgb(140, 118, 170)),
          new SolidColorBrush(Color.FromRgb(163, 147,185)),
          new SolidColorBrush(Color.FromRgb(180, 169, 197)),
          new SolidColorBrush(Color.FromRgb(197, 189, 210)),
          new SolidColorBrush(Color.FromRgb(213, 207, 221))
        
};
            #endregion

            #region CoolBlueScale
            m_CoolBlueScale = new Brush[]
{
         new SolidColorBrush(Color.FromRgb(48, 116, 134)),
          new SolidColorBrush(Color.FromRgb(55, 131, 151)),
          new SolidColorBrush(Color.FromRgb(62, 144, 167)),
          new SolidColorBrush(Color.FromRgb(67, 155, 179)),
          new SolidColorBrush(Color.FromRgb(72,166, 192)),
          new SolidColorBrush(Color.FromRgb(100, 178, 202)),
          new SolidColorBrush(Color.FromRgb(136, 192,210)),
          new SolidColorBrush(Color.FromRgb(161, 203, 218)),
          new SolidColorBrush(Color.FromRgb(184, 214, 225)),
          new SolidColorBrush(Color.FromRgb(203, 224, 233))
        
};
            #endregion

            #region ChocolateOrange
            m_ChocolateOrange = new Brush[]
{
       new SolidColorBrush(Color.FromRgb(168, 100, 45)),
          new SolidColorBrush(Color.FromRgb(189, 114, 51)),
          new SolidColorBrush(Color.FromRgb(208, 126, 58)),
          new SolidColorBrush(Color.FromRgb(224, 135, 63)),
          new SolidColorBrush(Color.FromRgb(239,145, 67)),
          new SolidColorBrush(Color.FromRgb(247, 159, 96)),
          new SolidColorBrush(Color.FromRgb(249, 177,134)),
          new SolidColorBrush(Color.FromRgb(250, 191, 160)),
          new SolidColorBrush(Color.FromRgb(251, 205, 183)),
          new SolidColorBrush(Color.FromRgb(252, 218, 203))

        
};
            #endregion

            #region MixedFantasy
            m_MixedFantasy = new Brush[]
{
     new SolidColorBrush(Color.FromRgb(79, 129, 189)),
     new SolidColorBrush(Color.FromRgb(170, 70, 67)),
     new SolidColorBrush(Color.FromRgb(137, 165, 78)),
     new SolidColorBrush(Color.FromRgb(113, 88, 143)),
     new SolidColorBrush(Color.FromRgb(65, 152, 175)),
     new SolidColorBrush(Color.FromRgb(219, 132, 61)),
        new SolidColorBrush(Color.FromRgb(147, 169, 207)),
     new SolidColorBrush(Color.FromRgb(209, 147, 146)),
     new SolidColorBrush(Color.FromRgb(185, 205, 150)),     

     new SolidColorBrush(Color.FromRgb(169, 155, 189)),
        
         
         
};
            #endregion

            #region Metro
            m_metroTheme = new Brush[]
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
        /// Initializes a new instance of the <see cref="ChartStyleModel"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        public ChartStyleModel(int count)
        {
            m_brushObjects = new StyleBindingObject[count];

            for (int i = 0; i < count; i++)
            {
                m_brushObjects[i] = new StyleBindingObject(m_brushes[i % m_defaultPalette.Length]);
            }
        }

        /// <summary>
        /// Called when instance created for ChartStyleModel
        /// </summary>
        public ChartStyleModel()
        {
            m_brushObjects = new StyleBindingObject[m_defaultPalette.Length];
            for (int i = 0; i < m_defaultPalette.Length; i++)
            {
                m_brushObjects[i] = new StyleBindingObject(m_brushes[i]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleModel"/> class.
        /// </summary>
        /// <param name="area">The area value.</param>
        /// <param name="count">The count value.</param>
        public ChartStyleModel(ChartArea area, int count)
        {
            if (area == null)
            {
                throw new ArgumentNullException("area");
            }

            m_area = area;
            m_area.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSeriesChanged);

            m_brushObjects = new StyleBindingObject[count];

            for (int i = 0; i < count; i++)
            {
                m_brushObjects[i] = new StyleBindingObject(m_brushes[i % m_defaultPalette.Length]);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the binding.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        /// <param name="colorIndex">Index of the color.</param>
        public void SetBinding(DependencyObject target, DependencyProperty property, int colorIndex)
        {
            BindingOperations.SetBinding(target, property, m_brushObjects[colorIndex % m_brushObjects.Length].InteriorBinding);
        }

        /// <summary>
        /// Gets the icon by specified palette.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="palette">The palette.</param>
        /// <returns>Returns the image</returns>
        public Image GetIcon(int width, int height, ChartColorPalette palette)
        {
            Image resImg = new Image();
            DrawingVisual dv = new DrawingVisual();
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 0, 0, PixelFormats.Default);
            DrawingContext dc = dv.RenderOpen();
            Brush[] brushes = GetBrushes(palette);

            if (brushes != null)
            {
                for (int i = 0, c = brushes.Length; i < c; i++)
                {
                    dc.DrawRectangle(brushes[i], null, new Rect(i * width / c, 0, (i + 1) * width / c, height));
                }
            }

            dc.DrawRectangle(null, new Pen(Brushes.Black, 1), new Rect(0, 0, width, height));
            dc.Close();
            rtb.Render(dv);
            resImg.Source = rtb;

            return resImg;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Applies the palette.
        /// </summary>
        /// <param name="palette">The palette.</param>
        internal void ApplyPalette(ChartColorPalette palette)
        {
            m_brushes = GetBrushes(palette);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentPalette"));
            }

            for (int i = 0; i < m_brushObjects.Length; i++)
            {
                m_brushObjects[i].Interior = m_brushes == null ? null : m_brushes[i % m_brushes.Length];
            }
        }

        internal void ApplyPalette(Brush[] palette)
        {
            m_brushes = palette;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentPalette"));
            }

            for (int i = 0; i < m_brushObjects.Length; i++)
            {
                m_brushObjects[i].Interior = m_brushes == null ? null : m_brushes[i % m_brushes.Length];
            }
        }

        /// <summary>
        /// Gets the brushes.
        /// </summary>
        /// <param name="palette">The palette.</param>
        /// <returns>Returns the brushes</returns>
        private Brush[] GetBrushes(ChartColorPalette palette)
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

                case ChartColorPalette.Office2007Blue:
                    brushes = m_Office2007BluePalette;
                    break;

                case ChartColorPalette.Office2007Black:
                    brushes = m_Office2007BlackPalette;
                    break;

                case ChartColorPalette.Office2007Silver:
                    brushes = m_Office2007SilverPalette;
                    break;

                case ChartColorPalette.Custom:
                    brushes = m_customPalette;
                    break;

                case ChartColorPalette.Gradient:
                    brushes = m_GradientPalette;
                    break;


                case ChartColorPalette.MixedGray:
                    brushes = m_MixedGray;
                    break;

                case ChartColorPalette.BlueScale:
                    brushes = m_BlueScale;
                    break;

                case ChartColorPalette.MaroonRed:
                    brushes = m_MaroonRed;
                    break;

                case ChartColorPalette.GreenScale:
                    brushes = m_GreenScale;
                    break;

                case ChartColorPalette.MixedViolet:
                    brushes = m_MixedViolet;
                    break;

                case ChartColorPalette.CoolBlueScale:
                    brushes = m_CoolBlueScale;
                    break;

                case ChartColorPalette.ChocolateOrange:
                    brushes = m_ChocolateOrange;
                    break;

                case ChartColorPalette.MixedFantasy:
                    brushes = m_MixedFantasy;
                    break;

                case ChartColorPalette.Metro:
                    brushes = m_metroTheme;
                    break;
            }

            return brushes;
        }

        /// <summary>
        /// Called when series collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSeriesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ////if (args.NewItems != null)
            ////{
            ////  for (int i = 0; i < args.NewItems.Count; i++)
            ////  {
            ////    int index = (args.NewStartingIndex + i) % m_brushObjects.Length;
            ////    ChartSeries series = args.NewItems[i] as ChartSeries;

            ////    if (series.ReadLocalValue(ChartSeries.InteriorProperty) == DependencyProperty.UnsetValue)
            ////    {
            ////      BindingOperations.SetBinding(series, ChartSeries.InteriorProperty, m_brushObjects[index].InteriorBinding);
            ////    }
            ////  }
            ////}

            ////if (args.OldItems != null)
            ////{
            ////  foreach (ChartSeries series in args.OldItems)
            ////  {
            ////    Binding binding = BindingOperations.GetBinding(series, ChartSeries.InteriorProperty);

            ////    foreach (StyleBindingObject brushObject in m_brushObjects)
            ////    {
            ////      if (brushObject.InteriorBinding == binding)
            ////      {
            ////        BindingOperations.ClearBinding(series, ChartSeries.InteriorProperty);
            ////        break;
            ////      }
            ////    }
            ////  }
            ////}
        }
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //if(m_area!= null && m_area.Series!=null)
            //    m_area.Series.CollectionChanged -= OnSeriesChanged;
            //Array.Clear(m_GradientPalette, 0, m_GradientPalette.Length);
            //Array.Clear(m_brushes, 0, m_brushes.Length);
            if(m_brushObjects!=null)
            foreach (StyleBindingObject bindingObj in m_brushObjects)
            {
                if (bindingObj != null)
                {
                    bindingObj.Dispose();
                }
            }
            if (this.m_brushObjects != null)
            {
                foreach (StyleBindingObject sbo in this.m_brushObjects)
                {
                    sbo.Dispose();
                }
            }
            //if(m_brushObjects!=null)
            //Array.Clear(m_brushObjects, 0, m_brushObjects.Length);
            //Array.Clear(m_colorfulPalette, 0, m_colorfulPalette.Length);
            //Array.Clear(m_defaultAlphaPalette, 0, m_defaultAlphaPalette.Length);
            //Array.Clear(m_defaultPalette, 0, m_defaultPalette.Length);
            //Array.Clear(m_earthTonePalette, 0, m_earthTonePalette.Length);
            //Array.Clear(m_GradientPalette, 0, m_GradientPalette.Length);
            //Array.Clear(m_grayScalePalette, 0, m_grayScalePalette.Length);
            //Array.Clear(m_naturePalette, 0, m_naturePalette.Length);
            //Array.Clear(m_Office2007BlackPalette, 0, m_Office2007BlackPalette.Length);
            //Array.Clear(m_Office2007BluePalette, 0, m_Office2007BluePalette.Length);
            //Array.Clear(m_Office2007SilverPalette, 0, m_Office2007SilverPalette.Length);
            //Array.Clear(m_MixedGray, 0, m_MixedGray.Length);
            //Array.Clear(m_BlueScale, 0, m_BlueScale.Length);
            //Array.Clear(m_MaroonRed, 0, m_MaroonRed.Length);
            //Array.Clear(m_GreenScale, 0, m_GreenScale.Length);
            //Array.Clear(m_MixedViolet, 0, m_MixedViolet.Length);
            //Array.Clear(m_CoolBlueScale, 0, m_CoolBlueScale.Length);
            //Array.Clear(m_ChocolateOrange, 0, m_ChocolateOrange.Length);
            //Array.Clear(m_MixedFantasy, 0, m_MixedFantasy.Length);
            //Array.Clear(m_metroTheme,0,m_metroTheme.Length);
            //Array.Clear(m_pastelPalette, 0, m_pastelPalette.Length);
            //Array.Clear(m_triadPalette, 0, m_triadPalette.Length);
            //Array.Clear(m_warmColdPalette, 0, m_warmColdPalette.Length);
            m_area = null;            
            this.m_brushObjects = null;
            
        }

        #endregion
    }

   
}
