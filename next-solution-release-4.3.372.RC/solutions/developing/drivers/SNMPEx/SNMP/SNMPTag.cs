using System;
using System.Text;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using System.Runtime.InteropServices;

namespace SNMP
{
    public sealed class SNMPTag : Tag
    {
        #region Constructors

        public SNMPTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public SNMPTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool returnedValue = SNMPDynSettings.TryParse(dynamicSettings);

            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                // (uint)SNMPDynSettings.snmpDataSize;
                if (SNMPDynSettings.snmpDataType == SNMPDATATYPE.IpAddress)
                    Size = SNMPProtocol.SNMP_IPADDRESS_DEFAULT_SIZE;
                else
                    Size = SNMPProtocol.STRING_DEFAULT_SIZE;
            }

            return (returnedValue); 
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)SNMPDynSettings; }
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
                                val.SBYTE = (sbyte)curVal;
                                buffer[index] = val.BYTE;
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        val.SBYTE = (sbyte)(b.GetValue(i));
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
                                buffer[0] = (byte)curVal;
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        buffer[index+i] = (byte)(b.GetValue(i));
                                    }
                                }
                            }
                        }
                        return size;

                    case (uint)Opc.Ua.DataTypes.String:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= size)
                        {
                            System.Text.UTF8Encoding enc = new UTF8Encoding();
                            if (TagNode.ArrayDimension == 0)
                            {
                                string sVal = curVal as string;
                                buffer = enc.GetBytes(sVal);
                                size = (uint)buffer.Length;
                            }
                        }
                        return size;
                }
                
            }
            return base.GetTagBuffer(ref buffer, read, index, elemsize , buffertype );
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        readonly SNMPDynTagSettings _SNMPDynSettings = new SNMPDynTagSettings();
        public SNMPDynTagSettings SNMPDynSettings
        {
            get { return _SNMPDynSettings; }
        }
        #endregion        
    }
}
