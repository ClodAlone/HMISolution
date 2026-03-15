using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel.Extensions;

namespace TwinCAT
{
    public sealed class TwinCATTag : Tag
    {
        #region Constructors

        public TwinCATTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public TwinCATTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = TwinCATDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                if (TwinCATDynSettings.Length == 0)
                    Size = TwinCATProtocol.DEFAULT_STRING_SIZE;
                else
                    Size = (uint)TwinCATDynSettings.Length;
            }

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)TwinCATDynSettings; }
        }

        #endregion

        #region Properties

        private readonly TwinCATDynTagSettings _TwinCATDynSettings = new TwinCATDynTagSettings();
        public TwinCATDynTagSettings TwinCATDynSettings
        {
            get { return _TwinCATDynSettings; }
        }

        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            object curVal = Value.Value;
            uint size = 0;
            //The arrays of Boolean are not possible on Bit-Adress %MX8.0
            if ((TagNode.DataType.IdType == IdType.Numeric) &&
                ((uint)TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean) &&
                (TagNode.ArrayDimension > 0) )
            {
                if (buffer != null && buffer.Length >= TagNode.ArrayDimension)
                {
                    Array b = curVal as Array;
                    if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                    {
                        for (int i = 0; i < TagNode.ArrayDimension; i++)
                        {
                            buffer[index + i] = (byte)(DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Byte));
                        }
                        size = TagNode.ArrayDimension;
                    }
                }
            }
            else
            {
                if ((TagNode.DataType.IdType == IdType.Numeric) && ((uint)TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.String))
                {
                    // convert data from UTF8 string (Movicon format) to ASCII 8 bit
                    byte[] tmpData = Encoding.Default.GetBytes((string)curVal);
                    Array.Copy(tmpData, buffer, (tmpData.Length > buffer.Length-1 ? buffer.Length - 1 : tmpData.Length));
                    size = (uint)tmpData.Length;
                }
                else
                    size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
            }
            return (size);
        }
            #endregion
    }
}
