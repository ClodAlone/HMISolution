using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UFWebClient.HTML5
{
    [DataContract]
    public class DataInfo
    {
        [DataMember]
        public String value { get; set; }
        [DataMember]
        public double minValue { get; set; }
        [DataMember]
        public double maxValue { get; set; }
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        [DataMember]
        public List<String> selection { get; set; }
    }
}
