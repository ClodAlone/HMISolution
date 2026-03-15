using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
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
                Size = (uint)SNMPDynSettings.snmpDataSize;
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
                object curVal = Value.Value; 
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
                }
                
            }
            return base.GetTagBuffer(ref buffer, read, index, elemsize , buffertype );
        }

        #endregion

        #region Methods

        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (Value.StatusCode.Code != StatusCodes.Good)
                    forceUpdate = true;

                bool bCopy = false;
                object val = new object();
                uint size = 0;
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                {
                    SetReadValue(null);
                }
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            System.Text.UTF8Encoding enc = new UTF8Encoding();
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (Size > buffer.Length)
                                    size = (uint)buffer.Length;
                                else
                                    size = Size;
                                byte[] tbuf = new byte[size];
                                Array.Copy(buffer, index, tbuf, 0, size);
                                val = enc.GetString(tbuf);
                                bCopy = (GetReadValue() == null || (string)GetReadValue() != (string)val || (LastValue != null && (string)LastValue != (string)val));
                            }
                            else
                            {
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = GetReadValue() as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                Array bL = LastValue as Array;
                                bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check || !checkL);
                                int elemlen = (int)(Size / TagNode.ArrayDimension);
                                byte[] tbuf = new byte[elemlen];
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    Array.Copy(buffer, index + (i * elemlen), tbuf, 0, elemlen);
                                    a[i] = enc.GetString(tbuf);
                                    if (check && a[i] != (string)b.GetValue(i) || (checkL && a[i] != (string)bL.GetValue(i)))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (GetReadValue() == null || bCopy)
                            {
                                SetReadValue(val);
                                WriteVal = val;
                                Value.Value = val;
                                LastValue = Value.Value;
                                return true;
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= (index + size))
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                val = BitConverter.ToBoolean(buffer, index);
                                bCopy = (GetReadValue() == null || (bool)GetReadValue() != (bool)val);
                            }
                            else
                            {
                                bool[] a = new bool[TagNode.ArrayDimension];
                                Array b = GetReadValue()/*Value.Value*/ as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = !check;
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    a[i] = BitConverter.ToBoolean(buffer, index + i);
                                    if (check && a[i] != (bool)b.GetValue(i))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (GetReadValue() == null || bCopy)
                            {
                                SetReadValue(val);
                                WriteVal = val;
                                Value.Value = val;
                                LastValue = Value.Value;
                                return true;
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.SByte:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length > index)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                val = (sbyte)buffer[index];
                                bCopy = (GetReadValue() == null || (sbyte)GetReadValue() != (sbyte)val);
                            }
                            else
                            {
                                uint dataDimension = TagNode.ArrayDimension;
                                if (dataDimension > (buffer.Length - index))
                                {
                                    dataDimension = (uint)(buffer.Length - index);
                                }

                                sbyte[] a = new sbyte[TagNode.ArrayDimension];
                                Array b = GetReadValue()/*Value.Value*/ as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = !check;
                                for (int i = 0; i < dataDimension; i++)
                                {
                                    a[i] = (sbyte)buffer[index + i];
                                    if (check && a[i] != (sbyte)b.GetValue(i))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (GetReadValue() == null || bCopy)
                            {
                                SetReadValue(val);
                                WriteVal = val;
                                Value.Value = val;
                                LastValue = Value.Value;
                                return true;
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Byte:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length > index)
                        {
                            uint dataDimension = TagNode.ArrayDimension;
                            if (dataDimension > (buffer.Length - index))
                            {
                                dataDimension = (uint)(buffer.Length - index);
                            }

                            if (TagNode.ArrayDimension == 0)
                            {
                                val = (byte)buffer[index];
                                bCopy = (GetReadValue() == null || (byte)GetReadValue() != (byte)val);
                            }
                            else
                            {
                                byte[] a = new byte[TagNode.ArrayDimension];
                                Array b = GetReadValue()/*Value.Value*/ as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = !check;
                                for (int i = 0; i < dataDimension; i++)
                                {
                                    a[i] = (byte)buffer[index + i];
                                    if (check && a[i] != (byte)b.GetValue(i))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (GetReadValue() == null || bCopy)
                            {
                                SetReadValue(val);
                                WriteVal = val;
                                Value.Value = val;
                                LastValue = Value.Value;
                                return true;
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt16:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 2, elemsize);
                                    }
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToUInt16(nbuf, index);
                                    //bCopy = (Value.Value == null || (UInt16)Value.Value != (UInt16)val);
                                    bCopy = (GetReadValue() == null || (UInt16)GetReadValue() != (UInt16)val);
                                }
                                else
                                {
                                    UInt16[] a = new UInt16[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToUInt16(nbuf, index + 2 * i);
                                        if (check && a[i] != (UInt16)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                //if (Value.Value == null || bCopy)
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Double:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
                                    }
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    if (elemsize == 1)
                                        val = (Double)nbuf[index];
                                    else if (elemsize == 2)
                                        val = (Double)BitConverter.ToInt16(nbuf, index);
                                    else if (elemsize == 4)
                                        val = (Double)BitConverter.ToInt32(nbuf, index);
                                    else if (elemsize == 8)
                                        val = (Double)BitConverter.ToInt64(nbuf, index);
                                    else
                                        val = BitConverter.ToDouble(nbuf, index);
                                    bCopy = (GetReadValue() == null || (double)GetReadValue() != (double)val);
                                }
                                else
                                {
                                    double[] a = new double[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        if (elemsize == 1)
                                            a[i] = (Double)nbuf[index + 8 * i];
                                        else if (elemsize == 2)
                                            a[i] = (Double)BitConverter.ToInt16(nbuf, (int)(index + 8 * i));
                                        else if (elemsize == 4)
                                            a[i] = (Double)BitConverter.ToInt32(nbuf, (int)(index + 8 * i));
                                        else if (elemsize == 8)
                                            a[i] = (Double)BitConverter.ToInt64(nbuf, (int)(index + 8 * i));
                                        else
                                            a[i] = BitConverter.ToDouble(nbuf, index + 8 * i);
                                        if (check && a[i] != (double)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int16:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 2, elemsize);
                                    }
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToInt16(nbuf, index);
                                    bCopy = (GetReadValue() == null || (Int16)GetReadValue() != (Int16)val);
                                }
                                else
                                {
                                    Int16[] a = new Int16[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToInt16(nbuf, index + 2 * i);
                                        if (check && a[i] != (Int16)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int32:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
                                    }
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToInt32(nbuf, index);
                                    bCopy = (GetReadValue() == null || (Int32)GetReadValue() != (Int32)val);
                                }
                                else
                                {
                                    Int32[] a = new Int32[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToInt32(nbuf, index + 4 * i);
                                        if (check && a[i] != (Int32)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int64:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
                                    }
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToInt64(nbuf, index);
                                    bCopy = (GetReadValue() == null || (Int64)GetReadValue() != (Int64)val);
                                }
                                else
                                {
                                    Int64[] a = new Int64[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToInt64(nbuf, index + 8 * i);
                                        if (check && a[i] != (Int64)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Float:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
                                    }
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    if (elemsize == 1)
                                        val = (Single)nbuf[index];
                                    else if (elemsize == 2)
                                        val = (Single)BitConverter.ToInt16(nbuf, index);
                                    else if (elemsize == 4)
                                        val = (Single)BitConverter.ToInt32(nbuf, index);
                                    else if (elemsize == 8)
                                        val = (Single)BitConverter.ToInt64(nbuf, index);
                                    else
                                        val = BitConverter.ToSingle(nbuf, index);
                                    bCopy = (GetReadValue() == null || (float)GetReadValue() != (float)val);
                                }
                                else
                                {
                                    float[] a = new float[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        if (elemsize == 1)
                                            a[i] = (Single)nbuf[(int)(index + 4 * i)];
                                        else if (elemsize == 2)
                                            a[i] = (Single)BitConverter.ToInt16(nbuf, (int)(index + 4 * i));
                                        else if (elemsize == 4)
                                            a[i] = (Single)BitConverter.ToInt32(nbuf, (int)(index + 4 * i));
                                        else if (elemsize == 8)
                                            a[i] = (Single)BitConverter.ToInt64(nbuf, (int)(index + 4 * i));
                                        else
                                            a[i] = BitConverter.ToSingle(nbuf, index + 4 * i);
                                        if (check && a[i] != (float)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt32:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);

                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
                                    }
                                }

                            }


                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToUInt32(nbuf, index);
                                    bCopy = (GetReadValue() == null || (UInt32)GetReadValue() != (UInt32)val);
                                }
                                else
                                {
                                    UInt32[] a = new UInt32[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToUInt32(nbuf, index + 4 * i);
                                        if (check && a[i] != (UInt32)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt64:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {

                                    buffer.CopyTo(nbuf, 0);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
                                    }
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToUInt64(nbuf, index);
                                    bCopy = (GetReadValue() == null || (UInt64)GetReadValue() != (UInt64)val);
                                }
                                else
                                {
                                    UInt64[] a = new UInt64[TagNode.ArrayDimension];
                                    Array b = GetReadValue()/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToUInt64(nbuf, index + 8 * i);
                                        if (check && a[i] != (UInt64)b.GetValue(i))
                                            bCopy = true;
                                    }
                                    val = a;
                                }
                                if (GetReadValue() == null || bCopy)
                                {
                                    SetReadValue(val);
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                            }
                        }
                        break;
                }
            }
            return false;
        }
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
