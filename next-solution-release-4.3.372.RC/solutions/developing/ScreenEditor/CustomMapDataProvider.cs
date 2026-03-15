using DevExpress.Xpf.Map;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenManager
{
    public class CustomMapDataProvider : MapDataProviderBase
    {
        readonly SphericalMercatorProjection projection = new SphericalMercatorProjection();

        public override ProjectionBase Projection { get { return projection; } }

        public CustomMapDataProvider(String url = @"http://{subdomain}.tile.openstreetmap.org/{tileLevel}/{tileX}/{tileY}.png")
        {
            SetTileSource(new CustomTileSource(url));
        }
        protected override MapDependencyObject CreateObject()
        {
            return new CustomMapDataProvider();
        }
        public override Size GetMapSizeInPixels(double zoomLevel)
        {
            return new Size(Math.Pow(2.0, zoomLevel) * OpenStreetMapTileSource.tileSize,
                Math.Pow(2.0, zoomLevel) * OpenStreetMapTileSource.tileSize);
        }
    }

    public class CustomTileSource : MapTileSourceBase
    {
        string roadUrlTemplate;
        public const int maxZoomLevel = 20;
        public const int tileSize = 256;

        static int imageWidth = (int)Math.Pow(2.0, maxZoomLevel) * tileSize;
        static int imageHeight = (int)Math.Pow(2.0, maxZoomLevel) * tileSize;
        static string[] subdomains = new string[] { "a", "b", "c" };

        public CustomTileSource(String url)
            : base(imageWidth, imageHeight, tileSize, tileSize)
        {
            roadUrlTemplate = url;
        }
        public override Uri GetTileByZoomLevel(int zoomLevel, long tilePositionX, long tilePositionY)
        {
            string url = roadUrlTemplate;
            url = url.Replace("{tileX}", tilePositionX.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{tileY}", tilePositionY.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{tileLevel}", zoomLevel.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{subdomain}", subdomains[GetSubdomainIndex(subdomains.Length)]);
            return new Uri(url);
        }
    }

}
