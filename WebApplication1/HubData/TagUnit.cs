using LiteDB;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeData
{
    public class TagUnit
    {
        [BsonId]
        public String id { get; set; }

        public Opc.Ua.Range Range { get; set; }
        public Opc.Ua.Range InstrumentRange { get; set; }
        public EUInformation EUInformation { get; set; }

    }
}
