using LiteDB;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeData
{
    public class TagDefinition
    {
        [BsonId]
        public String id { get; set; }
        public String idTagUnit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsRetentive { get; set; }

        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }

        public long ReadableAccessMask { get; set; }
        public long WritableAccessMask { get; set; }
        public long AccessLevel { get; set; }

        public TypeInfo TypeInfo { get; set; }
        public DataValue InitialValue { get; set; }
        public Opc.Ua.Range RateOfChange { get; set; }
        public TagDefinition[] ObjectType { get; set; }
    }
}

