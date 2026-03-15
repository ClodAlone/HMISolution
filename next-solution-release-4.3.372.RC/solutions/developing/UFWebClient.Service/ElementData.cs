using System;
using System.Runtime.Serialization;

namespace UFWebClient.Service
{
    [DataContract]
    public class ElementData
    {
        [DataMember]
        public String id { get; set; }
        [DataMember]
        public String storageid { get; set; }
        [DataMember]
        public double top { get; set; }
        [DataMember]
        public double left { get; set; }
        [DataMember]
        public double width { get; set; }
        [DataMember]
        public double height { get; set; }
        [DataMember]
        public String imageData { get; set; }
        [DataMember]
        public bool hasImage { get; set; }
        [DataMember]
        public bool resizable { get; set; }
        [DataMember]
        public bool connected { get; set; }
        [DataMember]
        public bool simulateEvent { get; set; }
        [DataMember]
        public bool writable { get; set; }
        [DataMember]
        public String LastMessage { get; set; }
        [DataMember]
        public int dataType { get; set; }
        [DataMember]
        public bool validated { get; set; }
        [DataMember]
        public bool userLocked { get; set; }
        [DataMember]
        public bool demoMode { get; set; }
    }
}
