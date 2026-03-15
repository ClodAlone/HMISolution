using System;
using System.Runtime.Serialization;

namespace UFWebClient.HTML5
{
    [DataContract]
    public class CommandData
    {
        [DataMember]
        public String url { get; set; }
        [DataMember]
        public bool isSynchro { get; set; }
        [DataMember]
        public double width { get; set; }
        [DataMember]
        public double height { get; set; }
        [DataMember]
        public bool closeCurrent { get; set; }
        [DataMember]
        public bool isError { get; set; }

    }
}
