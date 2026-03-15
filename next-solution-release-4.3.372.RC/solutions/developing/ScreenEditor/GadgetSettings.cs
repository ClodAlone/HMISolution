using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager
{
    [DataContract(Name = "GadgetSettings")]
    public class GadgetSettings
    {
        [DataMember]
        public double left;
        [DataMember]
        public double top;
        [DataMember]
        public bool visible = true;
    }
}
