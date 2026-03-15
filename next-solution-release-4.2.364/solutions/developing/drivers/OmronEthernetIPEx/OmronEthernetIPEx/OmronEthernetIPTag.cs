using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
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

        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (Value.StatusCode.Code != StatusCodes.Good)
                    forceUpdate = true;

                bool bCopy = false;
                object val = new object();
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                    ReadValue = null;

                switch(nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            byte[] tbuf;
                            if (TagNode.ArrayDimension == 0)
                            {
                                if(buffer.Length > 0)
                                {
                                    tbuf = new byte[buffer.Length];
                                    Array.Copy(buffer, index, tbuf, 0, buffer.Length);
                                    return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                                }
                                // Empty string is an acceptable case
                                else
                                {
                                    val = String.Empty;
                                    // Check if the update of the tag value must be forced 
                                    if ((ReadValue != null) && ((ReadValue as String) != String.Empty))
                                    {
                                        bCopy = true;
                                    }
                                }
                                if (ReadValue == null || bCopy)
                                {
                                    ReadValue = val;
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = val;
                                    return true;
                                }
                            }
                            else
                            {
                                return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                            }
                        }
                        break;

                    default:
                        return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                }
            }

            return (false);
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
