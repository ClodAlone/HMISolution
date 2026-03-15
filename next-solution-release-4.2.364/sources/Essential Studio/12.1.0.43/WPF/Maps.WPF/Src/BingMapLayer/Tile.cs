#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
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
    using System.Windows.Media.Imaging;
    using Syncfusion.Maps.Imagery;
    using Syncfusion.Maps.Imagery.Common;
    using System.ComponentModel;
    using System.Diagnostics;


    /// <summary>
    ///  Tile is a UIElement that contains shape file layer and map Images
    /// </summary>
    public class Tile : Control
    {
        #region Internal Properties

        internal ImageryLayer imageryLayer;

        #region TileString



        /// <summary>
        /// Gets or sets Text value of the tile .
        /// </summary>
        /// <value>
        /// String
        /// </value>
        public string TileString
        {
            get { return (string)GetValue(TileStringProperty); }
            set { SetValue(TileStringProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TileString.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TileStringProperty =
            DependencyProperty.Register("TileString", typeof(string), typeof(Tile), new PropertyMetadata(string.Empty));



        #endregion

        #endregion

        #region Private Fields

        private ImageryLayer tempImgLayer;
#if WPF
        internal CustomBackgroundWorker worker;
#endif

        #endregion

        #region Properties

        #region ImageSource - Dependency Property.

        /// <summary>
        /// Gets or sets TileImageSource for the Tile.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ImageSource TileImageSource
        {
            get { return (ImageSource)GetValue(TileImageSourceProperty); }
            set { SetValue(TileImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileImageSource.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies TileImageSource Property
        /// </summary>
        public static readonly DependencyProperty TileImageSourceProperty =
     DependencyProperty.Register("TileImageSource", typeof(ImageSource), typeof(Tile), new PropertyMetadata(null));


        #endregion

        #region X - Dependency Property

        /// <summary>
        /// Gets or sets X Co-Ordinate For the Tile.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int X
        {
            get { return (int)GetValue(XProperty); }
            internal set { SetValue(XProperty, value); }
        }

        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies X Property of the Tiles
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(int), typeof(Tile), new PropertyMetadata(0));


        #endregion

        #region Y -Dependency Property



        /// <summary>
        /// Gets or sets Y Co-Ordinate for the Tile.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Y
        {
            get { return (int)GetValue(YProperty); }
            internal set { SetValue(YProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies Y Property of the Tile.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(int), typeof(Tile), new PropertyMetadata(0));



        #endregion




        /// <summary>
        /// Gets or sets Image for Tile.
        /// </summary>
        /// <value>
        /// DependencyObject
        /// </value>
        public DependencyObject TileImage
        {
            get { return (DependencyObject)GetValue(TileImageProperty); }
            set { SetValue(TileImageProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TileImage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TileImageProperty =
            DependencyProperty.Register("TileImage", typeof(DependencyObject), typeof(Tile), new PropertyMetadata(null));


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BingMapApp.Tile">Tile</see> class. 
        /// </summary>
        public Tile()
        {
            this.DefaultStyleKey = typeof(Tile);
#if SILVERLIGHT
            if (this.Style == null)
            {
                var resourceDic = new ResourceDictionary();
                resourceDic.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                this.Style = resourceDic["PART_TileStyle"] as Style;
            }
#endif

        }

        #endregion

        #region Override Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.imageryLayer = MapControl.FindParent<ImageryLayer>(this) as ImageryLayer;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised
        /// on this element. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.tempImgLayer.ReleaseMouseCapture();
            this.tempImgLayer.isMouseDown = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an
        /// element in its route that is derived from this class. Implement this method to
        /// add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.tempImgLayer.ReleaseMouseCapture();
            this.tempImgLayer.isMouseDown = false;
            this.tempImgLayer.temposition = new Point(0, 0);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/> attached event is raised on this element. Implement this method to add class
        /// handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

        }
        #endregion

        #region HelperMethod

        internal void SetTileImage(int ZoomLevel, ImageryLayer imageryLayer)
        {
            this.tempImgLayer = imageryLayer;
#if WPF
            worker = new CustomBackgroundWorker(this, this.X, this.Y, ZoomLevel, this.tempImgLayer.MapStyle, this.tempImgLayer.BingMapKey);
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
#endif
#if SILVERLIGHT

            ImageryMetadataRequest mapMetaDataRequest = new ImageryMetadataRequest();
            ImageryMetadataOptions mapUriOptions = new ImageryMetadataOptions();

            mapMetaDataRequest.Credentials = new Credentials();
            mapMetaDataRequest.Credentials.ApplicationId = this.tempImgLayer.BingMapKey; ;
            var latlong = TileXYToLatLong(this.X, this.Y, (int)this.tempImgLayer.ZoomLevel);

            Location centerLocation = new Location();
            centerLocation.Latitude = latlong.X;
            centerLocation.Longitude = latlong.Y;
            mapUriOptions.Location = centerLocation;

            // Set the map style and zoom level
            mapUriOptions.ZoomLevel = (int)this.tempImgLayer.ZoomLevel;
            mapMetaDataRequest.Style = this.tempImgLayer.MapStyle;

            mapMetaDataRequest.Options = mapUriOptions;

            ImageryServiceClient imageryService;
            System.ServiceModel.BasicHttpBinding httpBinding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None) { MaxBufferSize = 2147483647, MaxReceivedMessageSize = 2147483647 };
            imageryService = new ImageryServiceClient(httpBinding, new System.ServiceModel.EndpointAddress("http://dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc"));
            //ImageryMetadataResponse metadataResponse = imageryService.GetImageryMetadata(mapMetaDataRequest);
            //ImageryMetadataResult result = metadataResponse.Results[0];
            imageryLayer.mapUriRequest.Options = imageryLayer.mapUriOptions;
            imageryLayer.imageryService = new ImageryServiceClient(imageryLayer.httpBinding, new System.ServiceModel.EndpointAddress("http://dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc"));
            imageryLayer.imageryService.GetImageryMetadataCompleted  += (s, e) =>
                {
                    ImageryMetadataResult mapUriResponse = e.Result.Results[0];
                    BitmapImage bmpImg = new BitmapImage(new Uri(mapUriResponse.ImageUri));
                    this.TileImageSource = bmpImg;
                };
            imageryLayer.imageryService.GetImageryMetadataAsync(mapMetaDataRequest);
            // imageryLayer.imageryService.CloseAsync();

#endif
#if WPF
            worker.RunWorkerAsync();
#endif
        }
#if WPF

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled != true)
            {
                if ((sender as CustomBackgroundWorker).TileImageUri != null)
                {
                    (sender as CustomBackgroundWorker).Tile.TileImageSource = new BitmapImage(new Uri((sender as CustomBackgroundWorker).TileImageUri, UriKind.RelativeOrAbsolute));
                }
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((sender as CustomBackgroundWorker).CancellationPending)
            {
                e.Cancel = true;

            }
            else
            {
                if ((sender as CustomBackgroundWorker).ZoomLevel > 0)
                {
                    ImageryMetadataRequest mapMetaDataRequest = new ImageryMetadataRequest();
                    ImageryMetadataOptions mapUriOptions = new ImageryMetadataOptions();

                    mapMetaDataRequest.Credentials = new Credentials();
                    mapMetaDataRequest.Credentials.ApplicationId = (sender as CustomBackgroundWorker).BingMapKey; ;
                    var latlong = TileXYToLatLong((sender as CustomBackgroundWorker).X, (sender as CustomBackgroundWorker).Y, (sender as CustomBackgroundWorker).ZoomLevel);

                    Location centerLocation = new Location();
                    centerLocation.Latitude = latlong.X;
                    centerLocation.Longitude = latlong.Y;
                    mapUriOptions.Location = centerLocation;

                    // Set the map style and zoom level
                    mapUriOptions.ZoomLevel = (sender as CustomBackgroundWorker).ZoomLevel;
                    mapMetaDataRequest.Style = (sender as CustomBackgroundWorker).MapStyle;

                    mapMetaDataRequest.Options = mapUriOptions;

                    ImageryServiceClient imageryService;
                    System.ServiceModel.BasicHttpBinding httpBinding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None) { MaxBufferSize = 2147483647, MaxReceivedMessageSize = 2147483647 };
                    imageryService = new ImageryServiceClient(httpBinding, new System.ServiceModel.EndpointAddress("http://dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc"));
                    ImageryMetadataResponse metadataResponse = imageryService.GetImageryMetadata(mapMetaDataRequest);
                    ImageryMetadataResult result = metadataResponse.Results[0];
                    (sender as CustomBackgroundWorker).TileImageUri = result.ImageUri.ToString();
                }

            }

        }
#endif
#if SILVERLIGHT
        void imageryService_GetMapUriCompleted(object sender, GetMapUriCompletedEventArgs e)
        {
            // The result is an MapUriResponse Object

            MapUriResponse mapUriResponse = e.Result;
            BitmapImage bmpImg = new BitmapImage(new Uri(mapUriResponse.Uri));
            this.TileImageSource = bmpImg;
        }
#endif
        internal static double MinMax(double n, double minValue, double maxValue)
        {
            var max = Math.Max(n, minValue);
            var min = Math.Min(max, maxValue);
            return min;
        }
        internal static Point TileXYToLatLong(int X, int Y, int zoomlevel)
        {
            var pixX = ((X) * 256) + 128;
            var pixY = ((Y) * 256) + 128;
            return PointToLatLon(pixX, pixY, zoomlevel);
        }
        internal static Point PointToLatLon(double pixX, double pixY, int zoomlevel)
        {
            double mapSize = 256 << zoomlevel;
            double x = (MinMax(pixX, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (MinMax(pixY, 0, mapSize - 1) / mapSize);
            var latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            var longitude = 360 * x;

            return new Point(latitude, longitude);
        }
        /// <summary>
        ///  This method converts latitude longitude to point value
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="zoomlevel"></param>
        public static Point LatLongToPoint(double latitude, double longitude, int zoomlevel)
        {
            latitude = Tile.MinMax(latitude, -85, 85);
            longitude = Tile.MinMax(longitude, -180, 180);

            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            uint mapSize = (uint)256 << zoomlevel;
            var x1 = (int)Tile.MinMax(x * mapSize + 0.5, 0, mapSize - 1);
            var y1 = (int)Tile.MinMax(y * mapSize + 0.5, 0, mapSize - 1);
            return new Point(x1, y1);
        }
        #endregion
    }

}
