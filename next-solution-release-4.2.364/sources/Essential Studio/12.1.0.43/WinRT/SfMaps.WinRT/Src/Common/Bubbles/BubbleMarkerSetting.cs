#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Reflection;
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
#else
#if WPF
using System.Data;
#endif
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Maps
{

    /// <summary>
    /// Represent the BubbleMarkerSettings in the SfMap.
    /// </summary>
    /// <remarks>
    /// BubbleMarkerSetting class is used to set setting for the bubbles in the map.This class is used to set MaxSize, MinSize, Fill, Stoke, StokeThickness and the ValuePath for the Bubble.
    /// </remarks>
    /// <example>
    /// <code language="C#">
    /// using Syncfusion.UI.Xaml.Maps;
    /// using System;
    /// using System.Collections.Generic;
    /// using System.IO;
    /// using System.Linq;
    /// using Windows.Foundation;
    /// using Windows.Foundation.Collections;
    /// using Windows.UI;
    /// using Windows.UI.Xaml;
    /// using Windows.UI.Xaml.Controls;
    /// using Windows.UI.Xaml.Controls.Primitives;
    /// using Windows.UI.Xaml.Data;
    /// using Windows.UI.Xaml.Input;
    /// using Windows.UI.Xaml.Media;
    /// using Windows.UI.Xaml.Navigation;
    /// 
    /// 
    /// namespace MapApp
    /// {
    ///     public sealed partial class MainPage : Page
    ///     {
    ///         public MainPage()
    ///         {
    ///             this.InitializeComponent();
    ///             SfMap syncMap = new SfMap();
    ///             ViewModel viewModel = new ViewModel();
    ///             ShapeFileLayer layer = new ShapeFileLayer();
    ///             layer.Uri = "MapApp.world1.shp";
    ///             layer.ItemsSource = viewModel.Models;
    ///             layer.ShapeIDPath = "Country";
    ///             layer.ShapeIDTableField = "NAME";
    ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
    ///             bubbleSetting.AutoFillColor = true;
    ///             bubbleSetting.Fill = new SolidColorBrush(Colors.Red);
    ///             bubbleSetting.ValuePath = "AverageHighTemperature";
    ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
    ///             bubbleSetting.StrokeThickness = 5;
    ///             bubbleSetting.MaxSize = 500;
    ///             bubbleSetting.MinSize = 100;
    ///             layer.BubbleMarkerSetting = bubbleSetting;
    ///             syncMap.Layers.Add(layer);
    /// 
    /// 
    ///         }
    /// 
    ///     }
    ///     public class Weather
    ///     {
    /// 
    ///         public int CurrentTemperature { get; set; }
    ///         public int AverageHighTemperature { get; set; }
    ///         public int AverageLowTemperature { get; set; }
    ///         public string Country { get; set; }
    ///         public string Continent { get; set; }
    ///         public string City { get; set; }
    ///         public string WeatherDescription { get; set; }
    ///         public int Humidity { get; set; }
    ///         public string Longitude { get; set; }
    ///         public string Latitude { get; set; }
    /// 
    ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
    ///         {
    ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
    ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
    ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
    ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
    ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
    ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
    ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
    ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
    ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
    ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
    ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
    ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
    ///             return weatherCollection;
    ///         }
    ///     }
    ///     public class ViewModel
    ///     {
    ///         public List<![CDATA[<Weather>]]> Models
    ///         {
    ///             get;
    ///             set;
    ///         }
    /// 
    ///         public ViewModel()
    ///         {
    /// 
    ///             this.Models = new List<![CDATA[<Weather>]]>();
    ///             this.Models = Weather.GetWeatherData();
    /// 
    ///         }
    ///     }
    /// 
    /// }
    /// 
    /// </code>
    /// </example>
    [ClassReference(IsReviewed = false)]
    public class BubbleMarkerSetting : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.BubbleMarkerSetting">BubbleMarkerSetting</see> class. 
        /// </summary>
        /// <remarks>
        /// Initialize the object value for the BubbleMarkerSettings and it's property values.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public BubbleMarkerSetting()
        {
            ColorMappings = new ObservableCollection<ColorMapping>();
#if WINRT
            resouceDictionary = new ResourceDictionary { Source = new Uri("ms-appx:///Syncfusion.SfMaps.WinRT/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
#elif WPF
        	resouceDictionary = new ResourceDictionary { Source = new Uri(@"/Syncfusion.SfMaps.WPF;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
#elif SILVERLIGHT || SILVERLIGHT_5
        	resouceDictionary = new ResourceDictionary { Source = new Uri(@"/Syncfusion.SfMaps.Silverlight;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
#elif WINDOWSPHONE_8
        	resouceDictionary = new ResourceDictionary { Source = new Uri("/Syncfusion.SfMaps.WP8;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
#else
        	resouceDictionary = new ResourceDictionary { Source = new Uri("/Syncfusion.SfMaps.WP7;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
#endif
        }

        #region Internal Properties

        internal ShapeFileLayer shapeFileLayer;
        internal SfMap mapControl;
        internal readonly ResourceDictionary resouceDictionary;

        #endregion

        #region Properties

        #region BubbleType
        public BubbleType BubbleType
        {
            get { return (BubbleType)GetValue(BubbleTypeProperty); }
            set { SetValue(BubbleTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BubbleType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubbleTypeProperty =
            DependencyProperty.Register("BubbleType", typeof(BubbleType), typeof(BubbleMarkerSetting), new PropertyMetadata(BubbleType.Circle, OnBubbleTypeChanged));

        private static void OnBubbleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMarkerSetting)
            {
                var setting = d as BubbleMarkerSetting;
                if (setting.shapeFileLayer != null)
                {
                    setting.UpdateBubbleItems();
                    setting.UpdateLegendItems();
                }
            }
        }
        #endregion

        #region CustomTemplate
        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(BubbleMarkerSetting), new PropertyMetadata(null));
        #endregion

        #region SizeRatio
        /// <summary>
        /// Gets the Size ratio for the bubbles based on the <see cref="MinSize"/> and <see cref="MaxSize"/>
        /// </summary>
        /// <remarks>
        /// This read only property get the ratio for the bubbles size. Ratio will be determined based on MinSize and MaxSize of the <see cref="BubbleMarkerSetting"/>. 
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public double SizeRatio
        {
            get { return (double)GetValue(SizeRatioProperty); }
            internal set { SetValue(SizeRatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueRatio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeRatioProperty =
            DependencyProperty.Register("SizeRatio", typeof(double), typeof(BubbleMarkerSetting), new PropertyMetadata(0d));
        #endregion

        #region LegendSizeRatio
        internal double LegendSizeRatio
        {
            get { return (double)GetValue(LegendSizeRatioProperty); }
            set { SetValue(LegendSizeRatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendSizeRatio.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LegendSizeRatioProperty =
            DependencyProperty.Register("LegendSizeRatio", typeof(double), typeof(BubbleMarkerSetting), new PropertyMetadata(0d));
        #endregion

        #region ColorMappings

        /// <summary>
        /// Gets or sets the list of <see cref="RangeColorMapping"/> to provide tree map like support for bubbles.
        /// </summary>
        /// <remarks>
        /// This property contain collection of <see cref="RangeColorMapping"/> which contains Range and the Corresponding color of the Range. This property is used to provide the Tree map like support for bubbles.
        /// </remarks>
        /// <value>
        /// ObservableCollection of <see cref="RangeColorMapping"/>.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = false;
        ///             bubbleSetting.ColorMappings.Add(new RangeColorMapping { Range=0, Color=Colors.White });
        ///             bubbleSetting.ColorMappings.Add(new RangeColorMapping { Range = 50, Color = Colors.Thistle });
        ///             bubbleSetting.ColorMappings.Add(new RangeColorMapping { Range = 100, Color = Colors.Violet });
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public ObservableCollection<ColorMapping> ColorMappings
        {
            get { return (ObservableCollection<ColorMapping>)GetValue(ColorMappingsProperty); }
            set { SetValue(ColorMappingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorMappings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorMappingsProperty =
            DependencyProperty.Register("ColorMappings", typeof(ObservableCollection<ColorMapping>), typeof(BubbleMarkerSetting), new PropertyMetadata(null));

        #endregion

        #region AutoFillColor



        /// <summary>
        /// Gets or sets a value indicating whether fill color of bubbles from "Fill" property or "ColorMapping".
        /// </summary>      
        /// <remarks>
        /// This property is used to determine fill of the bubble. Bubbles fill color will be determined by "Fill" property if "True" or determined by ColorMappings if "False".
        /// </remarks>
        /// <value>
        /// Type :<see cref="bool"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = true;          
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public bool AutoFillColor
        {
            get { return (bool)GetValue(AutoFillColorProperty); }
            set { SetValue(AutoFillColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoFillColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoFillColorProperty =
            DependencyProperty.Register("AutoFillColor", typeof(bool), typeof(BubbleMarkerSetting), new PropertyMetadata(false));





        #endregion

        #region MaxSize
        /// <summary>
        /// Gets or sets the Maximum size of the bubble in the map.
        /// </summary>
        /// <remarks>
        /// MaxSize property set the Maximum height and width for the bubble which a bubble has maximum "BubbleValue" in the map.
        /// </remarks>
        /// Type :<see cref="double"/>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = true;          
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// 
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double MaxSize
        {
            get { return (double)GetValue(MaxSizeProperty); }
            set { SetValue(MaxSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxSizeProperty =
            DependencyProperty.Register("MaxSize", typeof(double), typeof(BubbleMarkerSetting), new PropertyMetadata(50d, OnMaxSizeChanged));

        private static void OnMaxSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMarkerSetting && e.NewValue != null)
            {
                var setting = d as BubbleMarkerSetting;
                setting.SizeRatio = setting.MinSize / (double)e.NewValue;
            }
        }
        #endregion

        #region MinSize
        /// <summary>
        /// Gets or sets the Minimum size of the bubble in the map.
        /// </summary>
        /// <remarks>
        /// MaxSize property set the Minimum height and width for the bubble which a bubble has minimum "BubbleValue" in the map.
        /// </remarks>
        /// Type :<see cref="double"/>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = true;          
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// 
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double MinSize
        {
            get { return (double)GetValue(MinSizeProperty); }
            set { SetValue(MinSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinSizeProperty =
            DependencyProperty.Register("MinSize", typeof(double), typeof(BubbleMarkerSetting), new PropertyMetadata(20d, OnMinSizeChanged));

        private static void OnMinSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMarkerSetting && e.NewValue != null)
            {
                var setting = d as BubbleMarkerSetting;
                setting.SizeRatio = (double)e.NewValue / setting.MaxSize;
            }
        }
        #endregion

        #region Fill


        /// <summary>
        /// Gets or sets the fill color of the bubbles when "AutoFillColor" of the <see cref="BubbleMarkerSetting"/> is True.
        /// </summary>
        /// <remarks>
        /// Use this property to set the fill color for the bubble when need not Tree map like support to the bubble fill. "AutoFillColor" of the <see cref="BubbleMarkerSetting"/> must set as true.
        /// </remarks>
        /// Type :<see cref="Brush"/>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = true;          
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// 
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(BubbleMarkerSetting), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));






        #endregion

        #region ValuePath


        /// <summary>
        /// Gets or sets the property name from the Item of ItemsSource in <see cref="ShapeFileLayer"/> for bubble value.
        /// </summary>
        /// <remarks>
        /// Use this ValuePath to determine the under bound value for the bubbles. The property value with the name of "ValuePath" will be fetched from Item of ItemsSource in <see cref="ShapeFileLayer"/>.
        /// </remarks>
        /// <value>
        /// Type :<see cref="string"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = true;          
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register("ValuePath", typeof(string), typeof(BubbleMarkerSetting), new PropertyMetadata(string.Empty));




        #endregion

        #region ColorValuePath

        public string ColorValuePath
        {
            get { return (string)GetValue(ColorValuePathProperty); }
            set { SetValue(ColorValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorValuePathProperty =
            DependencyProperty.Register("ColorValuePath", typeof(string), typeof(BubbleMarkerSetting), new PropertyMetadata(string.Empty));

        #endregion

        #region Stroke


        /// <summary>
        /// Gets or set the border color of the bubbles.
        /// </summary>
        /// <remarks>
        /// Use this property to set the border color of the bubbles in the map.
        /// </remarks>
        /// <value>
        /// Type :<see cref="Brush"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = true;          
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(BubbleMarkerSetting), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        #endregion


        #region StrokeThickness


        /// <summary>
        /// Gets or set the border thickness of the bubbles.
        /// </summary>
        /// <remarks>
        /// Use this property to set the border thickness of the bubbles in the map.
        /// </remarks>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ViewModel viewModel = new ViewModel();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ItemsSource = viewModel.Models;
        ///             layer.ShapeIDPath = "Country";
        ///             layer.ShapeIDTableField = "NAME";
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.AutoFillColor = true;          
        ///             bubbleSetting.ValuePath = "AverageHighTemperature";
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
        ///             bubbleSetting.StrokeThickness = 5;
        ///             bubbleSetting.MaxSize = 500;
        ///             bubbleSetting.MinSize = 100;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             syncMap.Layers.Add(layer);
        ///            
        /// 
        ///         }       
        ///        
        ///     }
        ///     public class Weather
        ///     {
        /// 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[<Weather>]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[<Weather>]]> weatherCollection = new List<![CDATA[<Weather>]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 63, CurrentTemperature = 59, AverageHighTemperature = 66, AverageLowTemperature = 45, City = "Beiging", Continent = "Asia", Country = "China", WeatherDescription = "Partly Cloudy", Longitude = "39.9100N", Latitude = "116.4000E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public List<![CDATA[<Weather>]]> Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new List<![CDATA[<Weather>]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// 
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(BubbleMarkerSetting), new PropertyMetadata(1d));



        #endregion

        #endregion

        #region Implementation

        internal double GetBubbleSize(double bubbleValue)
        {
            return (SizeRatio * bubbleValue) + MinSize;
        }

        private void UpdateBubbleItems()
        {
            foreach (Bubble bubble in shapeFileLayer.Bubbles)
            {
                double bubbleSize = GetBubbleSize(Convert.ToDouble(bubble.BubbleValue));
                bubble.BubbleItem = GetBubbleItem(bubbleSize, Convert.ToDouble(bubble.BubbleColorValue));
                bubble.Margin = new Thickness(bubble.midpoint.X - bubbleSize / 2, bubble.midpoint.Y - bubbleSize / (BubbleType == BubbleType.Rectangle ? 4 : 2), 0, 0);
            }
        }

        private void UpdateLegendItems()
        {
            if (shapeFileLayer.Legends.Count.Equals(ColorMappings.Count))
            {
                for (int i = 0; i < ColorMappings.Count; i++)
                {
                    double colorValue = 0d;
                    if (ColorMappings[i] is RangeColorMapping)
                    {
                        colorValue = (ColorMappings[i] as RangeColorMapping).Range;
                    }
                    else if (ColorMappings[i] is EqualsColorMapping)
                    {
                        colorValue = Convert.ToDouble((ColorMappings[i] as EqualsColorMapping).Value);
                    }
                    double bubbleSize = GetBubbleSize(Convert.ToDouble(colorValue));
                    shapeFileLayer.Legends[i].LegendIcon = GetBubbleItem(bubbleSize, colorValue);
                }
            }
        }

        internal FrameworkElement GetBubbleItem(double bubbleSize, double BubbleColorValue)
        {
            FrameworkElement element;
            DataTemplate dataTemplate;
            switch (BubbleType)
            {
                case BubbleType.Diamond:
                    dataTemplate = resouceDictionary["DiamondBubbleType"] as DataTemplate;
                    element = (dataTemplate != null) ? (Shape)dataTemplate.LoadContent() : null;
                    break;
                case BubbleType.Triangle:
                    dataTemplate = resouceDictionary["TriangleBubbleType"] as DataTemplate;
                    element = (dataTemplate != null) ? (Shape)dataTemplate.LoadContent() : null;
                    break;
                case BubbleType.Trapezoid:
                    dataTemplate = resouceDictionary["TrapezoidBubbleType"] as DataTemplate;
                    element = (dataTemplate != null) ? (Shape)dataTemplate.LoadContent() : null;
                    break;
                case BubbleType.Star:
                    dataTemplate = resouceDictionary["StarBubbleType"] as DataTemplate;
                    element = (dataTemplate != null) ? (Shape)dataTemplate.LoadContent() : null;
                    break;
                case BubbleType.Pushpin:
                    dataTemplate = resouceDictionary["PinBubbleType"] as DataTemplate;
                    element = (dataTemplate != null) ? (Shape)dataTemplate.LoadContent() : null;
                    break;
                case BubbleType.Pentagon:
                    dataTemplate = resouceDictionary["PentagonBubbleType"] as DataTemplate;
                    element = (dataTemplate != null) ? (Shape)dataTemplate.LoadContent() : null;
                    break;
                case BubbleType.Custom:
                    element = CustomTemplate != null ? (FrameworkElement)CustomTemplate.LoadContent() : null;
                    break;
                case BubbleType.Rectangle:
                    element = new Rectangle { Stretch = Stretch.Fill };
                    break;
                default:
                    element = new Ellipse { Stretch = Stretch.Fill };
                    break;
            }
            if (element != null)
            {
                if (BubbleType != BubbleType.Custom)
                {
                    SetShapeColor(element, BubbleColorValue);
                }
                element.Width = bubbleSize;
                element.Height = BubbleType.Equals(BubbleType.Rectangle) ? bubbleSize / 2 : bubbleSize;
            }
            return element;
        }

        private void SetShapeColor(FrameworkElement element, double BubbleColorValue)
        {
            if (element is Shape)
            {
                if (!AutoFillColor && ColorMappings.Count > 0)
                {
                    if (ColorMappings[0] is RangeColorMapping)
                    {
                        (element as Shape).Fill = new SolidColorBrush(mapControl.GetColor(BubbleColorValue, ColorMappings, Colors.Blue));
                    }
                    else
                    {
                        (element as Shape).Fill = new SolidColorBrush(mapControl.GetValueColor(BubbleColorValue, ColorMappings, Colors.Blue));
                    }
                }
                else
                {
                    (element as Shape).SetBinding(Shape.FillProperty, new Binding { Source = this, Path = new PropertyPath("Fill") });
                }
                (element as Shape).SetBinding(Shape.StrokeProperty, new Binding { Source = this, Path = new PropertyPath("Stroke") });
                (element as Shape).SetBinding(Shape.StrokeThicknessProperty, new Binding { Source = this, Path = new PropertyPath("StrokeThickness") });
            }
        }

        internal void FindSizeRatio(IEnumerable objs)
        {
            var max = 0d;
            foreach (object obj in objs)
            {
                object shapeValue;
                double parseVal;
#if WPF
                PropertyInfo shapeVal = null;

                if (obj is DataRow && shapeFileLayer != null && shapeFileLayer.ItemsSource is DataTable)
                {
                    if ((shapeFileLayer.ItemsSource as DataTable).Columns.Contains(ValuePath))
                    {
                        shapeValue = (obj as DataRow)[ValuePath];
                        if (double.TryParse(shapeValue.ToString(), out parseVal))
                        {
                            if (max < parseVal)
                            {
                                max = parseVal;
                            }
                        }
                        else
                        {
                            max = 0;
                            SizeRatio = 1;

                            break;
                        }
                    }
                }
                else
                {
                    shapeVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(ValuePath);
                }
#else
                PropertyInfo shapeVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(ValuePath);
#endif

                if (shapeVal != null)
                {
                    shapeValue = shapeVal.GetValue(obj, null);
                    if (double.TryParse(shapeValue.ToString(), out parseVal))
                    {
                        if (max < parseVal)
                        {
                            max = parseVal;
                        }
                    }
                    else
                    {
                        max = 20;
                        SizeRatio = 1;
                        break;
                    }
                }
            }
            SizeRatio = (max > 0) ? (MaxSize - MinSize) / max : 1;
        }

        internal void FindLegendSizeRatio()
        {
            double max;
            ObservableCollection<ColorMapping> tempColorMapping = ColorMappings;
            for (int i = 0; i < tempColorMapping.Count; i++)
            {
                for (int j = i + 1; j <= tempColorMapping.Count - 1; j++)
                {
                    if (tempColorMapping[i] is RangeColorMapping)
                    {
                        var tempColor1 = tempColorMapping[i] as RangeColorMapping;
                        var tempColor2 = tempColorMapping[j] as RangeColorMapping;
                        if (tempColor2 != null && tempColor1.Range <= tempColor2.Range)
                        {
                            double temp = tempColor1.Range;
                            tempColor1.Range = tempColor2.Range;
                            tempColor2.Range = temp;
                        }
                    }
                }
            }
            if (tempColorMapping.Count > 0 && tempColorMapping[0] is RangeColorMapping)
            {
                max = (tempColorMapping[0] as RangeColorMapping).Range;
            }
            else
            {
                max = 20;
            }
            if (max > 0)
            {
                LegendSizeRatio = (MaxSize - MinSize) / max;
            }
            else
            {
                LegendSizeRatio = 1;
            }
        }

        #endregion
    }
}
