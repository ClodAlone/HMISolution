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
    using Windows.UI.Xaml;
    using Windows.UI;
    using System.Threading.Tasks;
#else
    using System.Windows;
#endif
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the ShapeFillSetting class in the SfMap.Inherited from <see cref="DependencyObject"/>
    /// </summary>
    /// <remarks>
    /// ShapeFillSetting class is used to set fill setting for the shapes in the map.
    /// </remarks>
    /// <example>
    /// <para>Refer the following code to know how to set the ShapeFillSetting,</para>
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
    public class ShapeFillSetting : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ShapeFillSetting">ShapeFillSetting</see> class. 
        /// </summary>
        //[ClassReference(IsReviewed = false)]
        public ShapeFillSetting()
        {
            if (this.ColorMappings == null)
            {
                this.ColorMappings = new ObservableCollection<ColorMapping>();
            }
        }

        #region Properties

        #region AutoFillColors


        /// <summary>
        /// Gets or sets a value indicating whether fill color of map shapes from "ShapeFill" property or "ShapeSetting".
        /// </summary>      
        /// <remarks>
        /// This property is used to determine fill of the map shape. SfMap shapes fill color will be determined by "ShapeFill" property if "True" or determined by ColorMappings if "False".
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
        ///             ShapeSetting shapeSetting = new ShapeSetting();
        ///             shapeSetting.ShapeValuePath = "CurrentTemperature";
        ///             ShapeFillSetting shapeFillSetting = new ShapeFillSetting();
        ///             shapeFillSetting.AutoFillColors = true;
        ///             shapeSetting.ShapeFill=new SolidColorBrush(Colors.Yellow);        
        ///             shapeSetting.FillSetting = shapeFillSetting;
        ///             layer.ShapeSettings = shapeSetting;
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
        //[ClassReference(IsReviewed = false)]
        public bool AutoFillColors
        {
            get { return (bool)GetValue(AutoFillColorsProperty); }
            set { SetValue(AutoFillColorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoFillColors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoFillColorsProperty =
            DependencyProperty.Register("AutoFillColors", typeof(bool), typeof(ShapeFillSetting), new PropertyMetadata(false));




        #endregion

        #region ColorMappings


        /// <summary>
        /// Gets or sets the list of <see cref="RangeColorMapping"/> to provide tree map like support for map shapes.
        /// </summary>
        /// <remarks>
        /// This property contain collection of <see cref="RangeColorMapping"/> which contains Range and the Corresponding color of the Range. This property is used to provide the Tree map like support for map shapes.
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
        public ObservableCollection<ColorMapping> ColorMappings
        {
            get { return (ObservableCollection<ColorMapping>)GetValue(ColorMappingsProperty); }
            set { SetValue(ColorMappingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorMappings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorMappingsProperty =
            DependencyProperty.Register("ColorMappings", typeof(ObservableCollection<ColorMapping>), typeof(ShapeFillSetting), new PropertyMetadata(null));




        #endregion

        #endregion

    }
}
