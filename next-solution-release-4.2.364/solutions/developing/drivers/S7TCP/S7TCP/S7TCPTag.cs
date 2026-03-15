using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel.Extensions;
using System.Text;

namespace S7TCP
{
    public sealed class S7TCPTag : Tag
    {
        #region Constructors

        public S7TCPTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
            _IsMemberOfStruct = S7TCPDynSettings.IsMemberOfStruct;
        }

        public S7TCPTag(TagDefinition tag)
            : base(tag)
        {
            _IsMemberOfStruct = S7TCPDynSettings.IsMemberOfStruct;
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = S7TCPDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)S7TCPDynSettings.Length;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)S7TCPDynSettings; }
        }

        /// <summary>
        /// Check if current bit was the last changed starting from next
        /// </summary>
        /// <param name="b"></param>
        /// <param name="currentBitChanged"></param>
        /// <returns></returns>
        private bool IsCurrentBitLastChanged(Array b, int currentBitChanged)
        {            
            for (int i = currentBitChanged + 1 ; i < TagNode.ArrayDimension; i++)
            {                
                if ((bool)(DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean)) != (bool)((WriteVal as Array).GetValue(i)))
                    return false;
            }

            return true;
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
                            if (WriteVal == null)
                            {
                                WriteVal = b.Clone();
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    (WriteVal as Array).SetValue(!(bool)b.GetValue(i), i);
                                }
                            }

                            // force to check all bits of array
                            if (this.S7TCPArrayBoolWriteManualCheck)
                            {
                                S7TCPArrayBitWriteChangedElement++;
                                if (this.S7TCPArrayBitWriteChangedElement < TagNode.ArrayDimension)
                                {
                                    buffer[index] = ((bool)(b.GetValue(S7TCPArrayBitWriteChangedElement)) ? (byte)1 : (byte)0);
                                    this.S7TCPArrayBitWriteIsChangedElementLast = (this.S7TCPArrayBitWriteChangedElement == TagNode.ArrayDimension - 1);
                                    size = 1;
                                    return size;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    //write one bit at a time
                                    if ((bool)(DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean)) != (bool)((WriteVal as Array).GetValue(i)))
                                    {
                                        buffer[index] = ((bool)(b.GetValue(i)) ? (byte)1 : (byte)0);
                                        this.S7TCPArrayBitWriteChangedElement = i;
                                        // check and mark if this bit is last changed on array
                                        this.S7TCPArrayBitWriteIsChangedElementLast = IsCurrentBitLastChanged(b, i);
                                        size = 1;
                                        return size;
                                    }
                                }
                            }
                        }
                        size = unchecked((uint)(-1));
                    }
                }
            }
            else
            {
                switch ((uint)TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.UInt64:
                    case (uint)Opc.Ua.DataTypes.Int64:
                    case (uint)Opc.Ua.DataTypes.Double:
                        size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);

                        for (int i = 0; i < ((buffer.Length)/8); i++)
                            Array.Reverse(buffer, i * 8, 8);
                        break;
                    default:
                        size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
                        break;
                }
            }

            return size;

        }

        /// <summary>   The read value. </summary>
        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {                
                switch ((uint)TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.UInt64:
                    case (uint)Opc.Ua.DataTypes.Int64:
                    case (uint)Opc.Ua.DataTypes.Double:                        
                        for (int i = 0; i < (Size / 8); i++)
                            Array.Reverse(buffer, index + (i * 8), 8);
                        break;
                }

                return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);

            }
            return (false);
        }


        #endregion

        #region Methods
        public int GetBitArrayElementChanged()
        {
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean && TagNode.ArrayDimension > 0) {
                if (this.S7TCPArrayBitWriteChangedElement != -1)
                {
                    Array b = Value.Value as Array;
                    (WriteVal as Array).SetValue((bool)b.GetValue(this.S7TCPArrayBitWriteChangedElement), this.S7TCPArrayBitWriteChangedElement);
                    return this.S7TCPArrayBitWriteChangedElement;
                }
                //else
                //{
                //    Array b = Value.Value as Array;
                //    for (int i = 0; i < TagNode.ArrayDimension; i++)
                //    {
                //        //write one bit at a time
                //        if ((bool)(b.GetValue(i)) != (bool)((WriteVal as Array).GetValue(i)))
                //        {
                //            (WriteVal as Array).SetValue((bool)b.GetValue(i), i);
                //            return i;
                //        }
                //    }
                //}
            }
            return -1;
        }

        //public override bool SetTagValue(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
        //{
        //    if (TagNode.DataType.IdType == IdType.Numeric)
        //    {
        //        bool bCopy = false;
        //        object val = new object();
        //        uint size = 0;
        //        uint nType = (uint)TagNode.DataType.Identifier;
        //        if (forceUpdate)
        //            SetReadValue(null);
        //        switch (nType)
        //        {
        //            case (uint)Opc.Ua.DataTypes.String:
        //                {
        //                    System.Text.UTF8Encoding enc = new UTF8Encoding();
        //                    if (TagNode.ArrayDimension == 0)
        //                    {
        //                        if (Size > buffer.Length)
        //                            size = (uint)buffer.Length;
        //                        else
        //                            size = Size;
        //                        byte[] tbuf = new byte[size];
        //                        Array.Copy(buffer, index, tbuf, 0, size);
        //                        val = enc.GetString(tbuf);
        //                        bCopy = (GetReadValue() == null || (string)GetReadValue() != (string)val || (LastValue != null && (string)LastValue != (string)val));                                
        //                    }
        //                    else
        //                    {
        //                        string[] a = new string[TagNode.ArrayDimension];
        //                        Array b = GetReadValue() as Array;
        //                        bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                        Array bL = LastValue as Array;
        //                        bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                        bCopy = (!check || !checkL);
        //                        int elemlen = (int)(Size / TagNode.ArrayDimension);
        //                        byte[] tbuf = new byte[elemlen];
        //                        for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                        {
        //                            Array.Copy(buffer, index + (i * elemlen), tbuf, 0, elemlen);
        //                            a[i] = enc.GetString(tbuf);
        //                            if ((check && a[i] != (string)b.GetValue(i)) || (checkL && a[i] != (string)bL.GetValue(i)))
        //                                bCopy = true;
        //                        }
        //                        val = a;
        //                    }
        //                    if (GetReadValue() == null || bCopy)
        //                    {
        //                        SetReadValue(val);
        //                        WriteVal = val;
        //                        Value.Value = val;
        //                        LastValue = val;
        //                        return true;
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.Boolean:
        //                size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
        //                if (buffer != null && buffer.Length >= (index + size))
        //                {
        //                    if (TagNode.ArrayDimension == 0)
        //                    {
        //                        val = BitConverter.ToBoolean(buffer, index);
        //                        bCopy = (GetReadValue() == null || (bool)GetReadValue() != (bool)val || (LastValue != null && (bool)LastValue != (bool)val));
        //                    }
        //                    else
        //                    {
        //                        bool[] a = new bool[TagNode.ArrayDimension];
        //                        Array b = GetReadValue()/*Value.Value*/ as Array;
        //                        bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                        Array bL = LastValue/*Value.Value*/ as Array;
        //                        bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                        bCopy = (!check || !checkL);
        //                        for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                        {
        //                            a[i] = BitConverter.ToBoolean(buffer, index + i);
        //                            if ((check && a[i] != (bool)b.GetValue(i)) || (checkL && a[i] != (bool)bL.GetValue(i)))
        //                                bCopy = true;
        //                        }
        //                        val = a;
        //                    }
        //                    if (GetReadValue() == null || bCopy)
        //                    {
        //                        SetReadValue(val);
        //                        WriteVal = val;
        //                        Value.Value = val;
        //                        LastValue = val;
        //                        return true;
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.SByte:
        //                size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
        //                if (buffer != null && buffer.Length >= (index + size))
        //                {
        //                    if (TagNode.ArrayDimension == 0)
        //                    {
        //                        val = (sbyte)buffer[index];
        //                        bCopy = (GetReadValue() == null || (sbyte)GetReadValue() != (sbyte)val || (LastValue != null && (sbyte)LastValue != (sbyte)val));
        //                    }
        //                    else
        //                    {
        //                        sbyte[] a = new sbyte[TagNode.ArrayDimension];
        //                        Array b = GetReadValue()/*Value.Value*/ as Array;
        //                        bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                        Array bL = LastValue/*Value.Value*/ as Array;
        //                        bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                        bCopy = (!check || !checkL);
        //                        for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                        {
        //                            a[i] = (sbyte)buffer[index + i];
        //                            if ((check && a[i] != (sbyte)b.GetValue(i)) || (checkL && a[i] != (sbyte)bL.GetValue(i)))
        //                                bCopy = true;
        //                        }
        //                        val = a;
        //                    }
        //                    if (GetReadValue() == null || bCopy)
        //                    {
        //                        SetReadValue(val);
        //                        WriteVal = val;
        //                        Value.Value = val;
        //                        LastValue = val;
        //                        return true;
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.Byte:
        //                size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
        //                if (buffer != null && buffer.Length >= (index + size))
        //                {
        //                    if (TagNode.ArrayDimension == 0)
        //                    {
        //                        val = (byte)buffer[index];
        //                        bCopy = (GetReadValue() == null || (byte)GetReadValue() != (byte)val || (LastValue != null && (byte)LastValue != (byte)val));
        //                    }
        //                    else
        //                    {
        //                        byte[] a = new byte[TagNode.ArrayDimension];
        //                        Array b = GetReadValue()/*Value.Value*/ as Array;
        //                        bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                        Array bL = LastValue/*Value.Value*/ as Array;
        //                        bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                        bCopy = (!check || !checkL);
        //                        for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                        {
        //                            a[i] = (byte)buffer[index + i];
        //                            if ((check && a[i] != (byte)b.GetValue(i)) || (checkL && a[i] != (byte)bL.GetValue(i)))
        //                                bCopy = true;
        //                        }
        //                        val = a;
        //                    }
        //                    if (GetReadValue() == null || bCopy)
        //                    {
        //                        SetReadValue(val);
        //                        WriteVal = val;
        //                        Value.Value = val;
        //                        LastValue = val;
        //                        return true;
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.UInt16:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);
        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 2, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }
        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            val = BitConverter.ToUInt16(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (UInt16)GetReadValue() != (UInt16)val || (LastValue != null && (UInt16)LastValue != (UInt16)val));
        //                        }
        //                        else
        //                        {
        //                            UInt16[] a = new UInt16[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = (!check || !checkL);
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                a[i] = BitConverter.ToUInt16(nbuf, index + 2 * i);
        //                                if ((check && a[i] != (UInt16)b.GetValue(i)) || (checkL && a[i] != (UInt16)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        //if (Value.Value == null || bCopy)
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.Double:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);
        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }
        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            if (elemsize == 1)
        //                                val = (Double)nbuf[index];
        //                            else if (elemsize == 2)
        //                                val = (Double)BitConverter.ToInt16(nbuf, index);
        //                            else if (elemsize == 4)
        //                                val = (Double)BitConverter.ToInt32(nbuf, index);
        //                            else if (elemsize == 8)
        //                                val = (Double)BitConverter.ToInt64(nbuf, index);
        //                            else
        //                                val = BitConverter.ToDouble(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (double)GetReadValue() != (double)val) || (LastValue != null && (double)LastValue != (double)val);                                    
        //                        }
        //                        else
        //                        {
        //                            double[] a = new double[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = (!check || !checkL);
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                if (elemsize == 1)
        //                                    a[i] = (Double)nbuf[index + 8 * i];
        //                                else if (elemsize == 2)
        //                                    a[i] = (Double)BitConverter.ToInt16(nbuf, (int)(index + 8 * i));
        //                                else if (elemsize == 4)
        //                                    a[i] = (Double)BitConverter.ToInt32(nbuf, (int)(index + 8 * i));
        //                                else if (elemsize == 8)
        //                                    a[i] = (Double)BitConverter.ToInt64(nbuf, (int)(index + 8 * i));
        //                                else
        //                                    a[i] = BitConverter.ToDouble(nbuf, index + 8 * i);
        //                                if ((check && a[i] != (double)b.GetValue(i)) || (checkL && a[i] != (double)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.Int16:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);

        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 2, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }
        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            val = BitConverter.ToInt16(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (Int16)GetReadValue() != (Int16)val || (LastValue != null && (Int16)LastValue != (Int16)val));                                    
        //                        }
        //                        else
        //                        {
        //                            Int16[] a = new Int16[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = (!check || !checkL);
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                a[i] = BitConverter.ToInt16(nbuf, index + 2 * i);
        //                                if ((check && a[i] != (Int16)b.GetValue(i)) || (checkL && a[i] != (Int16)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.Int32:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);
        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);
        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }
        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            //val = BitConverter.ToInt32(nbuf, index);
        //                            if (elemsize == 1)
        //                                val = (Int32)nbuf[index];
        //                            else if (elemsize == 2)
        //                                val = (Int32)BitConverter.ToInt16(nbuf, index);
        //                            else
        //                                val = BitConverter.ToInt32(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (Int32)GetReadValue() != (Int32)val || (LastValue != null && (Int32)LastValue != (Int32)val));                                    
        //                        }
        //                        else
        //                        {
        //                            Int32[] a = new Int32[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = (!check || !checkL);
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                a[i] = BitConverter.ToInt32(nbuf, index + 4 * i);
        //                                if ((check && a[i] != (Int32)b.GetValue(i)) || (checkL && a[i] != (Int32)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.Int64:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);
        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }
        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            //val = BitConverter.ToInt64(nbuf, index);
        //                            if (elemsize == 1)
        //                                val = (Int64)nbuf[index];
        //                            else if (elemsize == 2)
        //                                val = (Int64)BitConverter.ToInt16(nbuf, index);
        //                            else if (elemsize == 4)
        //                                val = (Int64)BitConverter.ToInt32(nbuf, index);
        //                            else
        //                                val = BitConverter.ToInt64(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (Int64)GetReadValue() != (Int64)val || (LastValue != null && (Int64)LastValue != (Int64)val));                                    
        //                        }
        //                        else
        //                        {
        //                            Int64[] a = new Int64[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = (!check || !checkL);
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                a[i] = BitConverter.ToInt64(nbuf, index + 8 * i);
        //                                if ((check && a[i] != (Int64)b.GetValue(i)) || (checkL && a[i] != (Int64)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.Float:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);
        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);
        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }
        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            if (elemsize == 1)
        //                                val = (Single)nbuf[index];
        //                            else if (elemsize == 2)
        //                                val = (Single)BitConverter.ToInt16(nbuf, index);
        //                            else if (elemsize == 4)
        //                                val = (Single)BitConverter.ToInt32(nbuf, index);
        //                            else if (elemsize == 8)
        //                                val = (Single)BitConverter.ToInt64(nbuf, index);
        //                            else
        //                                val = BitConverter.ToSingle(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (float)GetReadValue() != (float)val || (LastValue != null && (float)LastValue != (float)val));                                    
        //                        }
        //                        else
        //                        {
        //                            float[] a = new float[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);                                                                        
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = (!check || !checkL);
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                if (elemsize == 1)
        //                                    a[i] = (Single)nbuf[(int)(index + 4 * i)];
        //                                else if (elemsize == 2)
        //                                    a[i] = (Single)BitConverter.ToInt16(nbuf, (int)(index + 4 * i));
        //                                else if (elemsize == 4)
        //                                    a[i] = (Single)BitConverter.ToInt32(nbuf, (int)(index + 4 * i));
        //                                else if (elemsize == 8)
        //                                    a[i] = (Single)BitConverter.ToInt64(nbuf, (int)(index + 4 * i));
        //                                else
        //                                    a[i] = BitConverter.ToSingle(nbuf, index + 4 * i);
        //                                if ((check && a[i] != (float)b.GetValue(i)) || (checkL && a[i] != (float)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.UInt32:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);

        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);
        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }


        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            val = BitConverter.ToUInt32(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (UInt32)GetReadValue() != (UInt32)val || (LastValue != null && (UInt32)LastValue != (UInt32)val));                                    
        //                        }
        //                        else
        //                        {
        //                            UInt32[] a = new UInt32[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = (!check || !checkL);
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                a[i] = BitConverter.ToUInt32(nbuf, index + 4 * i);
        //                                if ((check && a[i] != (UInt32)b.GetValue(i)) || (checkL && a[i] != (UInt32)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //            case (uint)Opc.Ua.DataTypes.UInt64:
        //                {
        //                    size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
        //                    byte[] nbuf = null;
        //                    if (elemsize == 0)
        //                    {
        //                        nbuf = new byte[buffer.Length];
        //                        buffer.CopyTo(nbuf, 0);
        //                    }
        //                    else
        //                    {
        //                        uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
        //                        nbuf = new byte[size];
        //                        if (buffer == null || buffer.Length < (index + newsize))
        //                            break;
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            Array.Copy(buffer, index, nbuf, 0, elemsize);
        //                            index = 0;
        //                        }
        //                        else
        //                        {
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
        //                            }

        //                            index = 0;
        //                        }

        //                    }
        //                    if (nbuf != null && nbuf.Length >= (index + size))
        //                    {
        //                        if (TagNode.ArrayDimension == 0)
        //                        {
        //                            val = BitConverter.ToUInt64(nbuf, index);
        //                            bCopy = (GetReadValue() == null || (UInt64)GetReadValue() != (UInt64)val || (LastValue != null && (UInt64)LastValue != (UInt64)val));                                    
        //                        }
        //                        else
        //                        {
        //                            UInt64[] a = new UInt64[TagNode.ArrayDimension];
        //                            Array b = GetReadValue()/*Value.Value*/ as Array;
        //                            bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
        //                            Array bL = LastValue/*Value.Value*/ as Array;
        //                            bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
        //                            bCopy = !check;
        //                            for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                            {
        //                                a[i] = BitConverter.ToUInt64(nbuf, index + 8 * i);
        //                                if ((check && a[i] != (UInt64)b.GetValue(i)) || (checkL && a[i] != (UInt64)bL.GetValue(i)))
        //                                    bCopy = true;
        //                            }
        //                            val = a;
        //                        }
        //                        if (GetReadValue() == null || bCopy)
        //                        {
        //                            SetReadValue(val);
        //                            WriteVal = val;
        //                            Value.Value = val;
        //                            LastValue = val;
        //                            return true;
        //                        }
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    return false;
        //}

        public void S7TCPInitArrayBoolWriteManualCheck()
        {
            _S7TCPArrayBoolWriteManualCheck = true;
            _S7TCPArrayBitWriteChangedElement = -1;
            _S7TCPArrayBitWriteIsChangedElementLast = false;
        }

        public void S7TCPResetArrayBoolWriteManualCheck()
        {
            S7TCPResetArrayBoolWriteManualCheck(true);
        }

        public void S7TCPResetArrayBoolWriteManualCheck(bool fullReset)
        {
            _S7TCPArrayBoolWriteManualCheck = false;
            if (fullReset)
            {                
                _S7TCPArrayBitWriteChangedElement = -1;                
                _S7TCPArrayBitWriteIsChangedElementLast = false;
            }
        }
        #endregion

        #region Properties

        private readonly S7TCPDynTagSettings _S7TCPDynSettings = new S7TCPDynTagSettings();
        public S7TCPDynTagSettings S7TCPDynSettings
        {
            get { return _S7TCPDynSettings; }
        }

        private bool _S7TCPArrayBoolWriteManualCheck = false;
        public bool S7TCPArrayBoolWriteManualCheck
        {
            get { return _S7TCPArrayBoolWriteManualCheck; }
            set { _S7TCPArrayBoolWriteManualCheck = value; }
        }

        private int _S7TCPArrayBitWriteChangedElement = -1;
        public int S7TCPArrayBitWriteChangedElement
        {
            get { return _S7TCPArrayBitWriteChangedElement; }
            set { _S7TCPArrayBitWriteChangedElement = value; }
        }
        private bool _S7TCPArrayBitWriteIsChangedElementLast = false;
        public bool S7TCPArrayBitWriteIsChangedElementLast
        {
            get { return _S7TCPArrayBitWriteIsChangedElementLast; }
            set { _S7TCPArrayBitWriteIsChangedElementLast = value; }
        }

        private bool _IsMemberOfStruct;
        public bool IsMemberOfStruct
        {
            get { return _IsMemberOfStruct; }
        }
        #endregion      
    }
}
