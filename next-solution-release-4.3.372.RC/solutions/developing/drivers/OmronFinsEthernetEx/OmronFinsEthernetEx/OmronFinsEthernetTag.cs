using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;
using System.Runtime.InteropServices;
using UFUAModel.Extensions;

namespace OmronFinsEthernet
{


    public sealed class OmronFinsEthernetTag : Tag
    {
        #region Constructors

        public OmronFinsEthernetTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public OmronFinsEthernetTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool returnValue = OmronFinsEthernetDynSettings.TryParse(dynamicSettings);
            if(returnValue == true)
            {
                if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                {
                    OmronAddress addObj = new OmronAddress(OmronFinsEthernetDynSettings.Address);
                    if(addObj.StringLength > 0)
                    {
                        Size = (uint)addObj.StringLength;
                    }
                }
            }
            return (returnValue);
        }

        /// <summary>   The read value. </summary>
        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (Value.StatusCode.Code != StatusCodes.Good)
                    forceUpdate = true;

                bool bCopy = false;
                object val = new object();
                uint nType = (uint)TagNode.DataType.Identifier;
                object readValue = (forceUpdate ? null : GetValue());
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            byte[] tbuf;
                            uint iCheckCharacter = 0;
                            if (TagNode.ArrayDimension == 0)
                            {
                                for (iCheckCharacter = 0; iCheckCharacter < Size; iCheckCharacter++)
                                {
                                    if (buffer[(iCheckCharacter + index)] == 0x00)
                                    {
                                        break;
                                    }
                                }
                                tbuf = new byte[iCheckCharacter];
                                Array.Copy(buffer, index, tbuf, 0, iCheckCharacter);
                                return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                            }
                            else
                            {
                                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                                uint nOffset = 0;
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check);
                                int elemlen = (int)(Size / TagNode.ArrayDimension);

                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    nOffset = (uint)(i * elemlen);
                                    for (iCheckCharacter = 0; iCheckCharacter < elemlen; iCheckCharacter++)
                                    {
                                        if (buffer[(nOffset + iCheckCharacter + index)] == 0x00)
                                        {
                                            break;
                                        }
                                    }
                                    tbuf = new byte[iCheckCharacter];
                                    Array.Copy(buffer, index + nOffset, tbuf, 0, iCheckCharacter);
                                    a[i] = enc.GetString(tbuf);
                                    if (check && a[i] != (string)b.GetValue(i))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (readValue == null || bCopy)
                            {
                                SetValue(val);
                                return true;
                            }
                        }
                        break;

                    default:
                        return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                }
            }

            return (false);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)OmronFinsEthernetDynSettings; }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct byteUnion
        {

            [FieldOffset(0)]
            public byte BYTE;
            [FieldOffset(0)]
            public sbyte SBYTE;
            // Constructor:
            public byteUnion(byte val)
            {
                this.SBYTE = 0;
                this.BYTE = val;
            }
            public byteUnion(sbyte val)
            {
                this.BYTE = 0;
                this.SBYTE = val;
            }
        }
        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                object curVal = WriteVal; 
                uint size;
                uint nType = (uint)TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.SByte:
                        byteUnion val = new byteUnion(0);
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                val.SBYTE = (sbyte)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.SByte);
                                buffer[index] = val.BYTE;
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        val.SBYTE = (sbyte)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.SByte);
                                        buffer[index+i] = val.BYTE;
                                    }
                                }
                            }
                        }
                        return size;

                    case (uint)Opc.Ua.DataTypes.Byte:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                                buffer[0] = (byte)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Byte);
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        buffer[index + i] = (byte)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Byte);
                                    }
                                }
                            }
                        }
                        return size;
                }
                
            }
            return base.GetTagBuffer(ref buffer, read, index, elemsize , buffertype );
        }

        #endregion

        #region Properties

        readonly OmronFinsEthernetDynTagSettings _OmronFinsEthernetDynSettings = new OmronFinsEthernetDynTagSettings();
        public OmronFinsEthernetDynTagSettings OmronFinsEthernetDynSettings
        {
            get { return _OmronFinsEthernetDynSettings; }
        }

        #endregion        

    }
}
