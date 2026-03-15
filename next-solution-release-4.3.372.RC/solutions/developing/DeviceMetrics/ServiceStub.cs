using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services.DeviceMetricsSvc
{
    public class ServiceStub : IDeviceMetrics
    {
        #region IDeviceMetrics Members

        public DeviceScreenMetrics GetDeviceScreenMetrics()
        {
            DeviceScreenMetrics dsm = new DeviceScreenMetrics();
            dsm.Width = 320;
            dsm.Height = 320;
            dsm.BitsPerPixel = 320;

            return dsm;
        }
        #endregion
    }
}
