using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel.Extensions;

namespace PPI
{
    public sealed class PPITag : Tag
    {
        #region Constructors

        public PPITag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public PPITag(TagDefinition tag)
            : base(tag)
        {
            
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = PPIDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)PPIDynSettings.Length;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)PPIDynSettings; }
        }

        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            object curVal = Value.Value;
            uint size = 0;
            if (TagNode.DataType.IdType == IdType.Numeric &&
                (uint)TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean)
            {
                size = 1;
                if (buffer != null && buffer.Length >= size)
                {
                    if (TagNode.ArrayDimension == 0)
                        BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, index);
                    else
                    {
                        Array b = curVal as Array;

                        if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                        {
                            for (int i = 0; i < TagNode.ArrayDimension; i++)
                            {
                                if ((bool)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean))
                                {
                                    //buffer[k] += (byte)System.Math.Pow(2.0, j);
                                    buffer[index + i / 8] += (byte)(1 << (i % 8));
                                }
                            }
                        }

                        //if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                        //{
                        //    if (WriteVal == null)
                        //    {
                        //        WriteVal = b.Clone();
                        //        for (int i = 0; i < TagNode.ArrayDimension; i++)
                        //        {
                        //            (WriteVal as Array).SetValue(!(bool)b.GetValue(i), i);
                        //        }
                        //    }
                        //    byte mask = 0;
                        //    for (int i = 0; i < TagNode.ArrayDimension; i++)
                        //    {
                        //        //write one bit at a time
                        //        if ((bool)(b.GetValue(i)) != (bool)((WriteVal as Array).GetValue(i)))
                        //        {
                        //            buffer[index] = ((bool)(b.GetValue(i)) ? (byte)1 : (byte)0);
                        //            (WriteVal as Array).SetValue((bool)b.GetValue(i), i);
                        //            size = 1;
                        //            return size;
                        //        }
                        //    }
                        //}
                        size = TagNode.ArrayDimension ;
                    }
                }
            }
            else
            {
                size = base.GetTagBuffer(ref buffer, read, index, elemsize , buffertype);
                if (size == 1 && buffer[0] == 0 &&
                    (uint)TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.String )
                    size = 0;
            }

            return size;


        }
        #endregion

        #region Methods
        //public int GetBitArrayElementChanged()
        //{
        //    if (TagNode.DataType.IdType == IdType.Numeric &&
        //        (uint)TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean &&
        //        TagNode.ArrayDimension > 0)
        //    {
        //        Array b = Value.Value as Array;
        //        for (int i = 0; i < TagNode.ArrayDimension; i++)
        //        {
        //            //write one bit at a time
        //            if ((bool)(b.GetValue(i)) != (bool)((WriteVal as Array).GetValue(i)))
        //            {
        //                return i;
        //            }
        //        }
        //    }
        //    return -1;
        //}
        #endregion

        #region Properties

        private readonly PPIDynTagSettings _PPIDynSettings = new PPIDynTagSettings();
        public PPIDynTagSettings PPIDynSettings
        {
            get { return _PPIDynSettings; }
        }

        #endregion      
    }
}
