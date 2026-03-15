using LiteDB;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeData
{
    public class Tag
    {
        public Tag()
        {
        }

        public Tag(Tag tag)
        {
            id = tag.id;
            DataValue = new DataValue(tag.DataValue);
        }

        [BsonId]
        public String id { get; set; }
        public String idTagDefinition { get; set; }
        public DataValue DataValue { get; set; }
    }
}
