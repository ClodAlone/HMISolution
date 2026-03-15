using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;

namespace MelsecFXTCP
{
    public sealed class MelsecFXTCPTag : Tag
    {
        public byte[] MemRWTag;
        #region Constructors

        public MelsecFXTCPTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
   
        }

        public MelsecFXTCPTag(TagDefinition tag)
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
        //                BitConverter.GetBytes((bool)(curVal)).CopyTo(buffer, index);
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
        //                            buffer[k] += (byte)System.Math.Pow(2.0, j);
        //                        if (++j > 7)
        //                        {
        //                            j = 0;
        //                            k++;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return size;
        //    }
        //    else
        //        return base.GetTagBuffer(ref buffer, read, index);
        //}

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return MelsecFXTCPDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)MelsecFXTCPDynSettings; }
        }

        #endregion

        #region Properties

        readonly MelsecFXTCPDynTagSettings _MelsecFXTCPDynSettings = new MelsecFXTCPDynTagSettings();
        public MelsecFXTCPDynTagSettings MelsecFXTCPDynSettings
        {
            get { return _MelsecFXTCPDynSettings; }
        }

        #endregion 
       
        # region Mothod
        public bool setMemRWTag(byte[] buffer, int index, int Length)
        {
            if (MemRWTag == null)
                MemRWTag = new byte[Length];
            if (Length > MemRWTag.Length)
                return false;
            Array.Copy(buffer, index, MemRWTag, 0, Length);
            return true;
        }
        #endregion
    }
}
