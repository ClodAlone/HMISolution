#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
#if WINRT
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
    using System.Threading.Tasks;
#else
    using System.Windows.Media;
    using System.Windows;
#endif
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the ShapeSetting of the SfMap. Inherited from the <see cref="DependencyObject"/>
    /// </summary>
    /// <remarks>
    /// ShapeSetting class contains the member for Customizing the appearance of the MapShapes and determine the under bound values. It was inherited from the <see cref="DependencyObject"/> class.
    /// </remarks>
    /// <example>
    /// <para>Refer the following code to know how to set the ShapeSettings</para>
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
    ///             ShapeSetting shapeSetting = new ShapeSetting();
    ///             shapeSetting.ShapeValuePath = "CurrentTemperature";
    ///             shapeSetting.ShapeFill = new SolidColorBrush(Colors.Salmon);
    ///             shapeSetting.ShapeStroke = new SolidColorBrush(Colors.Black);
    ///             shapeSetting.ShapeStrokeThickness = 2d;
    ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);
    ///             ShapeFillSetting shapeFillSetting = new ShapeFillSetting();
    ///             shapeFillSetting.AutoFillColors = true;
    ///             shapeSetting.FillSetting = shapeFillSetting;
    ///             layer.ShapeSettings = shapeSetting;
    ///             syncMap.Layers.Add(layer);
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
    /// </code>
    /// </example>
    //[ClassReference(IsReviewed = false)]
    public class ShapeSetting : DependencyObject
    {

        #region InternalFields

        internal ShapeFileLayer layer;

        #endregion

        #region Constructor


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ShapeSetting">ShapeSetting</see> class. 
        /// </summary>
        //[ClassReference(IsReviewed = false)]
        public ShapeSetting()
        {
            if (this.CustomColors == null)
            {
                this.CustomColors = new ObservableCollection<MapColorPalette>();
            }
            if (this.FillSetting == null)
            {
                this.FillSetting = new ShapeFillSetting();
            }

        }

        #endregion

        #region Properties

        #region FillSetting


        /// <summary>
        /// Gets or sets the settings for filling color of the shapes.
        /// </summary>
        /// <remarks>
        /// FillSetting properties contains the members to provide the tree map like support for the map shapes.
        /// </remarks>
        /// <value>
        /// Type :<see cref="ShapeFillSetting"/>
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
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeValuePath = "CurrentTemperature";        
        ///             ShapeFillSetting shapeFillSetting = new ShapeFillSetting();
        ///             shapeFillSetting.AutoFillColors = false;
        ///             shapeFillSetting.ColorMappings.Add(new RangeColorMapping { Range = 0, Color = Colors.White });
        ///             shapeFillSetting.ColorMappings.Add(new RangeColorMapping { Range = 50, Color = Colors.Red });
        ///             shapeFillSetting.ColorMappings.Add(new RangeColorMapping { Range = 100, Color = Colors.Purple });           
        ///             shapeSetting.FillSetting = shapeFillSetting;
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
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
        //[ClassReference(IsReviewed = false)]
        public ShapeFillSetting FillSetting
        {
            get { return (ShapeFillSetting)GetValue(FillSettingProperty); }
            set { SetValue(FillSettingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillSetting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillSettingProperty =
            DependencyProperty.Register("FillSetting", typeof(ShapeFillSetting), typeof(ShapeSetting), new PropertyMetadata(null));




        #endregion

        #region CustomColors


        /// <summary>
        /// Gets or sets the list of colors for CustomPalette.
        /// </summary>
        /// <remarks>
        /// CustomColors property used to set the colors for CustomPalette. These colors applied on the shapes when ColorPalette set as CustomPalette.        
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
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeValuePath = "CurrentTemperature";           
        ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);
        ///             shapeSetting.ColorPalette = ColorPalettes.CustomPalette;
        ///             shapeSetting.CustomColors.Add(new MapColorPalette { FillBrush=new SolidColorBrush(Colors.Blue) });
        ///             shapeSetting.CustomColors.Add(new MapColorPalette { FillBrush = new SolidColorBrush(Colors.Gray) });
        ///             shapeSetting.CustomColors.Add(new MapColorPalette { FillBrush = new SolidColorBrush(Colors.RoyalBlue });
        ///             shapeSetting.CustomColors.Add(new MapColorPalette { FillBrush = new SolidColorBrush(Colors.Orchid) });
        ///             ShapeFillSetting shapeFillSetting = new ShapeFillSetting();
        ///             shapeFillSetting.AutoFillColors = true;
        ///             shapeSetting.FillSetting = shapeFillSetting;
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
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
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<MapColorPalette> CustomColors
        {
            get { return (ObservableCollection<MapColorPalette>)GetValue(CustomColorsProperty); }
            set { SetValue(CustomColorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomColors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomColorsProperty =
            DependencyProperty.Register("CustomColors", typeof(ObservableCollection<MapColorPalette>), typeof(ShapeSetting), new PropertyMetadata(null));



        #endregion

        #region ShapeFill


        /// <summary>
        /// Gets or sets the fill color for the map shape.
        /// </summary>
        /// <remarks>
        /// ShapeFill property is used to set the fill color for the map shapes.
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
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeValuePath = "CurrentTemperature";
        ///             shapeSetting.ShapeFill = new SolidColorBrush(Colors.Salmon);
        ///             shapeSetting.ShapeStroke = new SolidColorBrush(Colors.Black);
        ///             shapeSetting.ShapeStrokeThickness = 2d;
        ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);
        ///             ShapeFillSetting shapeFillSetting = new ShapeFillSetting();
        ///             shapeFillSetting.AutoFillColors = false;
        ///             shapeSetting.FillSetting = shapeFillSetting;
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
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
        //[ClassReference(IsReviewed = false)]
        public Brush ShapeFill
        {
            get { return (Brush)GetValue(ShapeFillProperty); }
            set { SetValue(ShapeFillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeFill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeFillProperty =
            DependencyProperty.Register("ShapeFill", typeof(Brush), typeof(ShapeSetting), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(229, 229, 229, 229)), new PropertyChangedCallback(OnShapeFillChanged)));

        private static void OnShapeFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeSetting setting = d as ShapeSetting;
            if (setting.layer != null)
            {
                foreach (MapShape shp in setting.layer.MapShapes)
                {
                    if (!shp.isSelected)
                    {
                        setting.layer.FillColors(shp.Shape, shp.ColorValue, -1);
                    }
                }
            }
        }




        #endregion

        #region ShapeStroke


        /// <summary>
        /// Gets or sets the border color for the MapShape.
        /// </summary>
        /// <remarks>
        /// ShapeStroke property is set the border color for the map shape.
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
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeValuePath = "CurrentTemperature";
        ///             shapeSetting.ShapeFill = new SolidColorBrush(Colors.Salmon);
        ///             shapeSetting.ShapeStroke = new SolidColorBrush(Colors.Black);
        ///             shapeSetting.ShapeStrokeThickness = 2d;
        ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);          
        ///             ShapeFillSetting shapeFillSetting = new ShapeFillSetting();
        ///             shapeFillSetting.AutoFillColors = false;                
        ///             shapeSetting.FillSetting = shapeFillSetting;
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
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
        //[ClassReference(IsReviewed = false)]
        public Brush ShapeStroke
        {
            get { return (Brush)GetValue(ShapeStrokeProperty); }
            set { SetValue(ShapeStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeStrokeProperty =
            DependencyProperty.Register("ShapeStroke", typeof(Brush), typeof(ShapeSetting), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(193, 193, 193, 193))));



        #endregion

        #region ShapeStrokeThickness


        /// <summary>
        /// Gets or sets border thickness of the map shapes.
        /// </summary>
        /// <remarks>
        /// ShapeStrokeThickness sets the border thickness of the map shapes.
        /// </remarks>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <example>
        /// <code languge="C#">
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
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeValuePath = "CurrentTemperature";
        ///             shapeSetting.ShapeFill = new SolidColorBrush(Colors.Salmon);
        ///             shapeSetting.ShapeStroke = new SolidColorBrush(Colors.Black);
        ///             shapeSetting.ShapeStrokeThickness = 2d;
        ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);
        ///             shapeSetting.ColorPalette = ColorPalettes.CustomPalette;
        ///             shapeFillSetting.AutoFillColors = false;
        ///               shapeSetting.FillSetting = shapeFillSetting;
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
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
        //[ClassReference(IsReviewed = false)]
        public double ShapeStrokeThickness
        {
            get { return (double)GetValue(ShapeStrokeThicknessProperty); }
            set { SetValue(ShapeStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeStrokeThicknessProperty =
            DependencyProperty.Register("ShapeStrokeThickness", typeof(double), typeof(ShapeSetting), new PropertyMetadata(0.5d, new PropertyChangedCallback(OnShapeStrokeThicknessChanged)));

        private static void OnShapeStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settings = d as ShapeSetting;
            if (e.NewValue != null)
            {

            }
        }



        #endregion

        #region ShapeValuePath
        /// <summary>
        /// Gets or sets the property name to set the value for the map shape from the under bound object.
        /// </summary>
        /// <remarks>
        /// ShapeValuePath sets the name of the property of the under bound object to set the value for the map shapes.
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
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeValuePath = "CurrentTemperature";
        ///             shapeSetting.ShapeFill = new SolidColorBrush(Colors.Salmon);
        ///             shapeSetting.ShapeStroke = new SolidColorBrush(Colors.Black);
        ///             shapeSetting.ShapeStrokeThickness = 2d;
        ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
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
        //[ClassReference(IsReviewed = false)]
        public string ShapeValuePath
        {
            get { return (string)GetValue(ShapeValuePathProperty); }
            set { SetValue(ShapeValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeValuePathProperty =
            DependencyProperty.Register("ShapeValuePath", typeof(string), typeof(ShapeSetting), new PropertyMetadata(string.Empty));

        #endregion

        #region SelectedShapeColor

        /// <summary>
        /// Gets or sets the color for selected map shapes in the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// SelectedShapeColor sets the color of the SelectedMapShapes of the <see cref="ShapeFileLayer"/>
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
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";           
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeFill = new SolidColorBrush(Colors.Salmon);
        ///             shapeSetting.ShapeStroke = new SolidColorBrush(Colors.Black);
        ///             shapeSetting.ShapeStrokeThickness = 2d;
        ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);           
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
        /// 
        ///         }
        /// 
        ///     }
        ///     
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public Brush SelectedShapeColor
        {
            get { return (Brush)GetValue(SelectedShapeColorProperty); }
            set { SetValue(SelectedShapeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedShapeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedShapeColorProperty =
            DependencyProperty.Register("SelectedShapeColor", typeof(Brush), typeof(ShapeSetting), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        #endregion

        #region ColorPalette


        /// <summary>
        /// Gets or set ColorPalette for the map shapes.
        /// </summary>
        /// <remarks>
        /// ColorPalette is the set of colors that are applied on the map shapes.There are two build in color palette available.
        /// <para>Metro and CoolBlue</para>
        /// </remarks>
        /// <value>
        /// Type :<see cref="ColorPalettes"/>
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
        ///            
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";           
        ///             ShapeSetting shapeSetting = new ShapeSetting();            
        ///             shapeSetting.ShapeFill = new SolidColorBrush(Colors.Salmon);
        ///             shapeSetting.ShapeStroke = new SolidColorBrush(Colors.Black);
        ///             shapeSetting.ShapeStrokeThickness = 2d;
        ///             shapeSetting.SelectedShapeColor = new SolidColorBrush(Colors.Blue);
        ///             shapeSetting.ColorPalette = ColorPalettes.Metro;
        ///             ShapeFillSetting shapeFillSetting = new ShapeFillSetting();
        ///             shapeFillSetting.AutoFillColors = true;
        ///             hapeSetting.FillSetting = shapeFillSetting;
        ///             layer.ShapeSettings = shapeSetting;
        ///             syncMap.Layers.Add(layer);
        /// 
        ///         }
        /// 
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public ColorPalettes ColorPalette
        {
            get { return (ColorPalettes)GetValue(ColorPaletteProperty); }
            set { SetValue(ColorPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorPalette.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorPaletteProperty =
            DependencyProperty.Register("ColorPalette", typeof(ColorPalettes), typeof(ShapeSetting), new PropertyMetadata(ColorPalettes.Metro, new PropertyChangedCallback(OnColorPaletteChanged)));

        private static void OnColorPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeSetting settings = d as ShapeSetting;
            if (settings.layer != null)
            {
                if (e.NewValue != null)
                {
                    settings.layer.shapeCount = 0;
                    settings.layer.SetColorPalette((ColorPalettes)e.NewValue);
                    if ((ColorPalettes)e.NewValue == ColorPalettes.CustomPalette || (e.OldValue != null && (ColorPalettes)e.OldValue == ColorPalettes.CustomPalette))
                    {
                        foreach (MapShape shp in settings.layer.MapShapes)
                        {
                            settings.layer.FillColors(shp.Shape, null, -1);
                        }
                    }
                }
            }
        }




        #endregion

        #region ShapeColorValuePath

        public string ShapeColorValuePath
        {
            get { return (string)GetValue(ShapeColorValuePathProperty); }
            set { SetValue(ShapeColorValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeColorValuePathProperty =
            DependencyProperty.Register("ShapeColorValuePath", typeof(string), typeof(ShapeSetting), new PropertyMetadata(string.Empty));

        #endregion

        #endregion
    }
}
