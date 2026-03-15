using System;
using System.Runtime.Serialization;

namespace UFWebClient.HTML5
{
    [DataContract]
    public class TileInfo
    {
        [DataMember]
        public String Section { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public String Url { get; set; }
        [DataMember]
        public String ImageUrl { get; set; }
        [DataMember]
        public String Color { get; set; }
        [DataMember]
        public bool IsExtraSmall { get; set; }
        [DataMember]
        public bool IsExtraLarge { get; set; }
        [DataMember]
        public bool IsLarge { get; set; }
        [DataMember]
        public bool HasGeoCoordinates { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public String UsersVisibility { get; set; }
        [DataMember]
        public String RolesVisibility { get; set; }
    }
}
