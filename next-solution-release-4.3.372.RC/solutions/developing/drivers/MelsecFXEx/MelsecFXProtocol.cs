using DriverCodeBaseEx;
using Opc.Ua;

namespace MelsecFX
{
    public class MelsecFXProtocol
    {
        public const string TEST_COMM_DYNAMIC = "MelsecFX.Station={0}|LinkType=1|Addr=D0|Cpu=0";

        public static bool IsBitArrayTag(Tag defTag)
        {
            return (defTag != null && (defTag.TagNode.ArrayDimension != 0 && defTag.TagNode.DataType.IdType == IdType.Numeric && (uint)defTag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean));
        }
    }
}
