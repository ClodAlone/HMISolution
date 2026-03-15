using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services.DeviceMetricsSvc
{
    [ServiceContract]
    public interface IDeviceMetrics
    {
        [OperationContract]
        DeviceScreenMetrics GetDeviceScreenMetrics();
    }

    [DataContract]
    public class DeviceScreenMetrics
    {
        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public int BitsPerPixel { get; set; }
    }
}
