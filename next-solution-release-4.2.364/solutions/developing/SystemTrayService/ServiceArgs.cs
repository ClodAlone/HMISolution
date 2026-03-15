using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayService
{
    public class ServiceArgs : EventArgs
    {
        public String HostName;
        public String InstanceId;
        public String SchemaType;
        public Type ServiceType;
    }
}
