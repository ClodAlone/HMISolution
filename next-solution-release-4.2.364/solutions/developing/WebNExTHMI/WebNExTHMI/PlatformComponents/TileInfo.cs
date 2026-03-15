using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace WebNExTHMI.PlatformComponents
{
    public class TileInfo
    {
        public String Section { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public String ImageUrl { get; set; }
        public Color ColorValue { get; set; }
        public String Color { get; set; }
        public bool IsExtraSmall { get; set; }
        public bool IsExtraLarge { get; set; }
        public bool IsLarge { get; set; }
        public bool HasGeoCoordinates { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public String UsersVisibility { get; set; }
        public String RolesVisibility { get; set; }
        public double MinZoomLevel { get; set; }
        public double MaxZoomLevel { get; set; }
        public String LatTag { get; set; }
        public String LonTag { get; set; }

        public List<TileInfo> childs = new List<TileInfo>();
    }
}
