using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using Opc.Ua;
using System.Runtime.InteropServices;
using UFUAModel.Extensions;


namespace OmronEthernetIP
{


    public sealed class OmronEthernetIPTag : Tag
    {
        #region Constructors

        public OmronEthernetIPTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public OmronEthernetIPTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = OmronEthernetIPDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)OmronEthernetIPDynSettings.Length;

            return res;
        }
        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            uint nType = (uint)TagNode.DataType.Identifier;
            if (nType != (uint)Opc.Ua.DataTypes.Boolean)
            {
                return (base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype));
            }

            object curVal = Value.Value;
            uint size = 0;
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                if (buffer != null && buffer.Length >= size)
                {
                    if (TagNode.ArrayDimension == 0)
                    {
                        if (curVal != null)
                        {
                            BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, index);
                        }
                    }
                    else
                    {
                        Array b = curVal as Array;
                        if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                        {
                            for (int i = 0; i < TagNode.ArrayDimension; i++)
                            {
                                if ((bool)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean))
                                {
                                    buffer[index + i] += 1;
                                }
                                else
                                {
                                    buffer[index + i] += 0;
                                }
                            }
                        }
                    }
                }
            }

            return size;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)OmronEthernetIPDynSettings; }
        }


        #endregion

        #region Properties

        readonly OmronEthernetIPDynTagSettings _OmronEthernetIPDynSettings = new OmronEthernetIPDynTagSettings();
        public OmronEthernetIPDynTagSettings OmronEthernetIPDynSettings
        {
            get { return _OmronEthernetIPDynSettings; }
        }

        #endregion

    }
}
