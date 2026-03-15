using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using Opc.Ua;

namespace MelsecFX
{
    public sealed class MelsecFXTag : Tag
    {
        #region Constructors

        public MelsecFXTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public MelsecFXTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions
        //public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0)
        //{
        //    if (TagNode.DataType.IdType == IdType.Numeric && ((uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.Boolean))
        //    {
        //        object curVal = Value.Value;
        //        uint size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension / 8 + (TagNode.ArrayDimension % 8 > 0 ? 1 : 0));
        //        if (buffer != null && buffer.Length >= size)
        //        {
        //            if (TagNode.ArrayDimension == 0)
        //            {
        //                BitConverter.GetBytes((bool)(curVal)).CopyTo(buffer, index);
        //            }
        //            else
        //            {
        //                Array b = curVal as Array;
        //                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
        //                {
        //                    int j = 0;
        //                    int k = 0;
        //                    for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                    {
        //                        if ((bool)b.GetValue(i))
        //                        {
        //                            //buffer[k] += (byte)System.Math.Pow(2.0, j);
        //                            buffer[k] += (byte)(1 << j);
        //                        }
        //                        if (++j > 7)
        //                        {
        //                            j = 0;
        //                            k++;
        //                        }
        //                    }
        //                }
        //            }
        //            return size;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    else if(TagNode.DataType.IdType == IdType.Numeric && ((uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.Byte))
        //    {
        //        object curVal = Value.Value;
        //        uint size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
        //        if (buffer != null && buffer.Length >= size)
        //        {
        //            if (TagNode.ArrayDimension == 0)
        //            {
        //                //BitConverter.GetBytes((byte)(curVal)).CopyTo(buffer, index);
        //                buffer[index] = (byte)curVal;
        //            }
        //            else
        //            {
        //                Array b = curVal as Array;
        //                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
        //                {
        //                    for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                    {
        //                        //BitConverter.GetBytes((byte)(b.GetValue(i))).CopyTo(buffer, index + i);
        //                        buffer[index + i] = (byte)(b.GetValue(i));
        //                    }
        //                }
        //            }
        //            return size;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        return base.GetTagBuffer(ref buffer, read, index);
        //    }
        //}

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return MelsecFXDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)MelsecFXDynSettings; }
        }

        #endregion

        #region Properties

        readonly MelsecFXDynTagSettings _MelsecFXDynSettings = new MelsecFXDynTagSettings();
        public MelsecFXDynTagSettings MelsecFXDynSettings
        {
            get { return _MelsecFXDynSettings; }
        }

        #endregion        
    }
}
