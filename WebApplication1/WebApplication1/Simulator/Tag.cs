using LiteDB;
using Opc.Ua;

namespace WebApplication1.Simulator
{
    public class Tag
    {
        [BsonId]
        public long id { get; set; }
        public DataValue Value { get; set; }

    }
}
